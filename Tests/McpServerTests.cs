using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Word.Mcp;

namespace Tests
{
    [TestClass]
    public class McpServerTests
    {
        private const string Token = "test-token";

        private static ToolRegistry Fake()
        {
            var r = new ToolRegistry();
            r.Add("echo", "Echo text.", J.Schema(new[] { "text" }, ("text", "string", "Text.")),
                a => new JsonObject { ["echo"] = J.Str(a, "text", true) });
            r.Add("boom", "Always fails.", J.Schema(null), a => throw new InvalidOperationException("kaboom"));
            r.Add("prompt", "Records a UI message.", J.Schema(null), a =>
            {
                McpCallContext.Record("hello from ui");
                return new JsonObject { ["ok"] = true };
            });
            r.Add("slow", "Sleeps.", J.Schema(null), a => { System.Threading.Thread.Sleep(600); return new JsonObject(); });
            return r;
        }

        private static McpServer Server(int timeoutMs = 120000) => new McpServer(Fake(), null, Token, timeoutMs);

        private static McpResponse Post(McpServer s, string body, string auth = "Bearer " + Token, string origin = null)
            => s.Handle("POST", auth, origin, body);

        private static JsonObject Json(McpResponse r) => (JsonObject)JsonNode.Parse(r.Body);

        [TestMethod]
        public void Auth_MissingOrWrongToken_Is401()
        {
            var s = Server();
            const string body = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"ping\"}";
            Assert.AreEqual(401, Post(s, body, auth: null).Status);
            Assert.AreEqual(401, Post(s, body, auth: "Bearer wrong").Status);
            Assert.AreEqual(401, Post(s, body, auth: "Bearer test-toke").Status);
            Assert.AreEqual(401, Post(s, body, auth: "Basic " + Token).Status);
            Assert.AreEqual(200, Post(s, body).Status);
        }

        [TestMethod]
        public void Origin_NonLoopback_Is403_LoopbackAndNullAllowed()
        {
            var s = Server();
            const string body = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"ping\"}";
            Assert.AreEqual(403, Post(s, body, origin: "https://evil.example").Status);
            Assert.AreEqual(403, Post(s, body, origin: "http://localhost.evil.example").Status);
            Assert.AreEqual(403, Post(s, body, origin: "garbage").Status);
            Assert.AreEqual(200, Post(s, body, origin: "http://localhost:3000").Status);
            Assert.AreEqual(200, Post(s, body, origin: "http://127.0.0.1:5173").Status);
            Assert.AreEqual(200, Post(s, body, origin: "null").Status);
        }

        [TestMethod]
        public void Get_Is405()
        {
            var r = Server().Handle("GET", "Bearer " + Token, null, "");
            Assert.AreEqual(405, r.Status);
            Assert.AreEqual("POST", r.Allow);
        }

        [TestMethod]
        public void Initialize_NegotiatesProtocolAndServerInfo()
        {
            var s = Server();
            var r = Json(Post(s, "{\"jsonrpc\":\"2.0\",\"id\":7,\"method\":\"initialize\",\"params\":{\"protocolVersion\":\"2025-03-26\"}}"));
            Assert.AreEqual(7, (int)r["id"]);
            Assert.AreEqual("2025-03-26", (string)r["result"]["protocolVersion"]);
            Assert.AreEqual("spaghetti-word", (string)r["result"]["serverInfo"]["name"]);
            Assert.IsNotNull(r["result"]["capabilities"]["tools"]);

            r = Json(Post(s, "{\"jsonrpc\":\"2.0\",\"id\":\"a\",\"method\":\"initialize\",\"params\":{\"protocolVersion\":\"1999-01-01\"}}"));
            Assert.AreEqual("a", (string)r["id"]);
            Assert.AreEqual("2025-06-18", (string)r["result"]["protocolVersion"]);
        }

        [TestMethod]
        public void Notification_Is202_NoBody()
        {
            var r = Post(Server(), "{\"jsonrpc\":\"2.0\",\"method\":\"notifications/initialized\"}");
            Assert.AreEqual(202, r.Status);
            Assert.AreEqual("", r.Body);
        }

        [TestMethod]
        public void ParseError_AndInvalidRequest()
        {
            var s = Server();
            var r = Post(s, "{not json");
            Assert.AreEqual(400, r.Status);
            Assert.AreEqual(-32700, (int)Json(r)["error"]["code"]);

            r = Post(s, "[]");
            Assert.AreEqual(400, r.Status);
            Assert.AreEqual(-32600, (int)Json(r)["error"]["code"]);
        }

        [TestMethod]
        public void UnknownMethod_IsMinus32601()
        {
            var r = Json(Post(Server(), "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"nope\"}"));
            Assert.AreEqual(-32601, (int)r["error"]["code"]);
        }

        [TestMethod]
        public void Ping_ReturnsEmptyResult()
        {
            var r = Json(Post(Server(), "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"ping\"}"));
            Assert.AreEqual(0, ((JsonObject)r["result"]).Count);
        }

        [TestMethod]
        public void Batch_ReturnsArrayOfResponses_SkippingNotifications()
        {
            var r = Post(Server(), "[{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"ping\"},{\"jsonrpc\":\"2.0\",\"method\":\"notifications/initialized\"},{\"jsonrpc\":\"2.0\",\"id\":2,\"method\":\"ping\"}]");
            Assert.AreEqual(200, r.Status);
            var arr = (JsonArray)JsonNode.Parse(r.Body);
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(1, (int)arr[0]["id"]);
            Assert.AreEqual(2, (int)arr[1]["id"]);
        }

        [TestMethod]
        public void ToolsList_Shape()
        {
            var r = Json(Post(Server(), "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/list\"}"));
            var tools = (JsonArray)r["result"]["tools"];
            Assert.AreEqual(4, tools.Count);
            foreach (var t in tools)
            {
                Assert.IsFalse(string.IsNullOrEmpty((string)t["name"]));
                Assert.IsFalse(string.IsNullOrEmpty((string)t["description"]));
                Assert.AreEqual("object", (string)t["inputSchema"]["type"]);
                Assert.IsNotNull(t["inputSchema"]["properties"]);
            }
            Assert.AreEqual("echo", (string)tools[0]["name"]);
            Assert.AreEqual("text", (string)tools[0]["inputSchema"]["required"][0]);
        }

        [TestMethod]
        public void ToolsCall_Success_WrapsJsonAsTextContent()
        {
            var r = Json(Post(Server(), "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/call\",\"params\":{\"name\":\"echo\",\"arguments\":{\"text\":\"hi\"}}}"));
            Assert.IsFalse((bool)r["result"]["isError"]);
            Assert.AreEqual("text", (string)r["result"]["content"][0]["type"]);
            var inner = JsonNode.Parse((string)r["result"]["content"][0]["text"]);
            Assert.AreEqual("hi", (string)inner["echo"]);
        }

        [TestMethod]
        public void ToolsCall_ThrowingTool_IsErrorResult_NotCrash()
        {
            var r = Json(Post(Server(), "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/call\",\"params\":{\"name\":\"boom\"}}"));
            Assert.IsTrue((bool)r["result"]["isError"]);
            StringAssert.Contains((string)r["result"]["content"][0]["text"], "kaboom");
        }

        [TestMethod]
        public void ToolsCall_MissingArgument_IsErrorResult()
        {
            var r = Json(Post(Server(), "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/call\",\"params\":{\"name\":\"echo\",\"arguments\":{}}}"));
            Assert.IsTrue((bool)r["result"]["isError"]);
            StringAssert.Contains((string)r["result"]["content"][0]["text"], "text");
        }

        [TestMethod]
        public void ToolsCall_UnknownTool_IsInvalidParams()
        {
            var r = Json(Post(Server(), "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/call\",\"params\":{\"name\":\"rm_rf\"}}"));
            Assert.AreEqual(-32602, (int)r["error"]["code"]);
        }

        [TestMethod]
        public void ToolsCall_UiMessages_AreReturnedInResult()
        {
            var r = Json(Post(Server(), "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/call\",\"params\":{\"name\":\"prompt\"}}"));
            var inner = JsonNode.Parse((string)r["result"]["content"][0]["text"]);
            Assert.AreEqual("hello from ui", (string)inner["uiMessages"][0]);
            Assert.IsFalse(McpCallContext.Active, "context must be closed after the call");
        }

        [TestMethod]
        public void ToolsCall_Timeout_IsJsonRpcError()
        {
            var r = Json(Post(Server(timeoutMs: 100), "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"tools/call\",\"params\":{\"name\":\"slow\"}}"));
            Assert.AreEqual(-32000, (int)r["error"]["code"]);
        }

        [TestMethod]
        public void Token_IsUniqueBase64Url_32Bytes()
        {
            var a = McpServer.NewToken();
            var b = McpServer.NewToken();
            Assert.AreNotEqual(a, b);
            Assert.AreEqual(43, a.Length); // 32 bytes, base64 without padding
            Assert.IsFalse(a.Contains("+") || a.Contains("/") || a.Contains("="));
        }

        [TestMethod]
        public void RealHttp_RoundTrip_AndFallbackPort()
        {
            using (var s1 = new McpServer(Fake(), null, Token))
            using (var s2 = new McpServer(Fake(), null, Token))
            {
                s1.Start(39300);
                s2.Start(39300); // taken: must fall forward
                Assert.AreNotEqual(s1.Port, s2.Port);

                using (var http = new HttpClient())
                {
                    var req = new HttpRequestMessage(HttpMethod.Post, s1.Url)
                    {
                        Content = new StringContent("{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"ping\"}", Encoding.UTF8, "application/json")
                    };
                    Assert.AreEqual(401, (int)http.SendAsync(req).Result.StatusCode);

                    req = new HttpRequestMessage(HttpMethod.Post, s1.Url)
                    {
                        Content = new StringContent("{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"ping\"}", Encoding.UTF8, "application/json")
                    };
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    var resp = http.SendAsync(req).Result;
                    Assert.AreEqual(200, (int)resp.StatusCode);
                    StringAssert.Contains(resp.Content.ReadAsStringAsync().Result, "\"result\"");

                    var get = new HttpRequestMessage(HttpMethod.Get, s1.Url);
                    get.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    Assert.AreEqual(405, (int)http.SendAsync(get).Result.StatusCode);

                    var evil = new HttpRequestMessage(HttpMethod.Post, s1.Url)
                    {
                        Content = new StringContent("{}", Encoding.UTF8, "application/json")
                    };
                    evil.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    evil.Headers.Add("Origin", "https://evil.example");
                    Assert.AreEqual(403, (int)http.SendAsync(evil).Result.StatusCode);
                }
            }
        }
    }
}
