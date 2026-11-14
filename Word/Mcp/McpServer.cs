using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;

namespace Word.Mcp
{
    internal struct McpResponse
    {
        public int Status;
        public string Body;
        public string Allow;
    }

    /// <summary>
    /// Hand-rolled MCP "Streamable HTTP" server (JSON responses only, stateless): POST JSON-RPC 2.0 to /mcp/.
    /// Loopback only. Tools run on the captured UI synchronization context.
    /// Free of Office interop so it can be unit tested without Word.
    /// </summary>
    internal sealed class McpServer : IDisposable
    {
        internal const string LatestProtocol = "2025-06-18";
        private static readonly string[] Protocols = { "2025-06-18", "2025-03-26" };

        private readonly ToolRegistry _registry;
        private readonly SynchronizationContext _ui;
        private readonly int _timeoutMs;
        private HttpListener _listener;
        private Thread _thread;

        public int Port { get; private set; }
        public string Url => $"http://127.0.0.1:{Port}/mcp/";

        public McpServer(ToolRegistry registry, SynchronizationContext ui, int timeoutMs = 120000)
        {
            _registry = registry;
            _ui = ui;
            _timeoutMs = timeoutMs;
        }

        /// <summary>Starts listening on the first free port from <paramref name="preferredPort"/>.</summary>
        public void Start(int preferredPort = 39181)
        {
            for (var p = preferredPort; p < preferredPort + 50; p++)
            {
                var l = new HttpListener();
                l.Prefixes.Add($"http://127.0.0.1:{p}/mcp/");
                try
                {
                    l.Start();
                    _listener = l;
                    Port = p;
                    break;
                }
                catch (Exception)
                {
                    try { l.Close(); } catch { }
                }
            }
            if (_listener == null) throw new InvalidOperationException("No free port for the MCP server.");

            _thread = new Thread(Loop) { IsBackground = true, Name = "MCP listener" };
            _thread.Start();
        }

        public void Stop()
        {
            var l = _listener;
            _listener = null;
            try { l?.Stop(); l?.Close(); } catch { }
        }

        public void Dispose() => Stop();

        private void Loop()
        {
            var l = _listener;
            while (l != null && l.IsListening)
            {
                HttpListenerContext ctx;
                try { ctx = l.GetContext(); }
                catch { break; }
                ThreadPool.QueueUserWorkItem(_ => Serve(ctx));
            }
        }

        private void Serve(HttpListenerContext ctx)
        {
            try
            {
                var req = ctx.Request;
                var body = "";
                if (req.HasEntityBody)
                {
                    if (req.ContentLength64 > 8 * 1024 * 1024)
                    {
                        Write(ctx, new McpResponse { Status = 413, Body = "" });
                        return;
                    }
                    using (var sr = new StreamReader(req.InputStream, Encoding.UTF8)) body = sr.ReadToEnd();
                }
                Write(ctx, Handle(req.HttpMethod, req.Headers["Origin"], body));
            }
            catch (Exception)
            {
                try { Write(ctx, new McpResponse { Status = 500, Body = "" }); } catch { }
            }
        }

        private static void Write(HttpListenerContext ctx, McpResponse r)
        {
            var resp = ctx.Response;
            resp.StatusCode = r.Status;
            if (r.Allow != null) resp.AddHeader("Allow", r.Allow);
            if (!string.IsNullOrEmpty(r.Body))
            {
                var bytes = Encoding.UTF8.GetBytes(r.Body);
                resp.ContentType = "application/json; charset=utf-8";
                resp.ContentLength64 = bytes.Length;
                resp.OutputStream.Write(bytes, 0, bytes.Length);
            }
            else resp.ContentLength64 = 0;
            resp.Close();
        }

        // ---- transport-independent core (unit tested directly) ----

        internal McpResponse Handle(string method, string origin, string body)
        {
            if (!OriginAllowed(origin)) return new McpResponse { Status = 403, Body = "" };
            if (!string.Equals(method, "POST", StringComparison.OrdinalIgnoreCase))
                return new McpResponse { Status = 405, Body = "", Allow = "POST" };

            JsonNode root;
            try { root = JsonNode.Parse(body ?? ""); }
            catch (Exception) { root = null; }
            if (root == null)
                return new McpResponse { Status = 400, Body = Error(null, -32700, "Parse error").ToJsonString() };

            if (root is JsonArray batch)
            {
                if (batch.Count == 0)
                    return new McpResponse { Status = 400, Body = Error(null, -32600, "Invalid Request: empty batch").ToJsonString() };
                var outArr = new JsonArray();
                foreach (var item in batch)
                {
                    var r = HandleMessage(item as JsonObject);
                    if (r != null) outArr.Add(r);
                }
                return outArr.Count == 0
                    ? new McpResponse { Status = 202, Body = "" }
                    : new McpResponse { Status = 200, Body = outArr.ToJsonString() };
            }

            var one = HandleMessage(root as JsonObject);
            return one == null
                ? new McpResponse { Status = 202, Body = "" }
                : new McpResponse { Status = 200, Body = one.ToJsonString() };
        }


        /// <summary>DNS-rebinding guard: an Origin header, when present, must be null or a loopback host.</summary>
        internal static bool OriginAllowed(string origin)
        {
            if (string.IsNullOrEmpty(origin) || origin == "null") return true;
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var u)) return false;
            var h = u.Host;
            return h == "localhost" || h == "127.0.0.1" || h == "[::1]" || h == "::1";
        }

        private JsonNode HandleMessage(JsonObject msg)
        {
            if (msg == null) return Error(null, -32600, "Invalid Request");
            var hasId = msg.TryGetPropertyValue("id", out var idNode) && idNode != null;
            var id = hasId ? idNode.DeepClone() : null;
            string method;
            try { method = J.Get(msg, "method")?.GetValue<string>(); }
            catch { return Error(id, -32600, "Invalid Request: method must be a string"); }

            if (method == null) return null; // a response from the client: nothing to do
            if (!hasId) return null;         // notification (notifications/initialized etc.)

            try
            {
                var p = J.Get(msg, "params") as JsonObject;
                switch (method)
                {
                    case "initialize":
                        {
                            var want = J.Str(p, "protocolVersion");
                            var ver = Array.IndexOf(Protocols, want) >= 0 ? want : LatestProtocol;
                            return Result(id, new JsonObject
                            {
                                ["protocolVersion"] = ver,
                                ["capabilities"] = new JsonObject { ["tools"] = new JsonObject() },
                                ["serverInfo"] = new JsonObject { ["name"] = "spaghetti-word", ["version"] = "1.0.0" }
                            });
                        }
                    case "ping":
                        return Result(id, new JsonObject());
                    case "tools/list":
                        {
                            var arr = new JsonArray();
                            foreach (var t in _registry.Tools)
                                arr.Add(new JsonObject
                                {
                                    ["name"] = t.Name,
                                    ["description"] = t.Description,
                                    ["inputSchema"] = t.InputSchema.DeepClone()
                                });
                            return Result(id, new JsonObject { ["tools"] = arr });
                        }
                    case "tools/call":
                        return CallTool(id, p);
                    default:
                        return Error(id, -32601, "Method not found: " + method);
                }
            }
            catch (Exception ex)
            {
                return Error(id, -32603, ex.Message);
            }
        }

        private JsonNode CallTool(JsonNode id, JsonObject p)
        {
            var name = J.Str(p, "name");
            if (name == null || !_registry.TryGet(name, out var tool))
                return Error(id, -32602, "Unknown tool: " + name);
            var args = J.Get(p, "arguments") as JsonObject ?? new JsonObject();

            JsonNode value = null;
            Exception error = null;
            var messages = new string[0];
            var cancelled = false;
            var done = new ManualResetEventSlim();

            void Work()
            {
                if (cancelled) { done.Set(); return; }
                McpCallContext.Begin();
                try { value = tool.Handler(args); }
                catch (Exception ex) { error = ex; }
                finally
                {
                    messages = McpCallContext.End().ToArray();
                    done.Set();
                }
            }

            if (_ui == null) ThreadPool.QueueUserWorkItem(_ => Work());
            else _ui.Post(_ => Work(), null);

            if (!done.Wait(_timeoutMs))
            {
                cancelled = true;
                return Error(id, -32000, $"Tool '{name}' timed out after {_timeoutMs / 1000}s.");
            }

            if (error != null)
            {
                var o = new JsonObject { ["error"] = error.Message };
                if (messages.Length > 0) o["uiMessages"] = ToArray(messages);
                return Result(id, ToolResult(o.ToJsonString(), true));
            }

            var payload = value ?? new JsonObject();
            if (messages.Length > 0)
            {
                var o = payload as JsonObject ?? new JsonObject { ["result"] = payload };
                o["uiMessages"] = ToArray(messages);
                payload = o;
            }
            return Result(id, ToolResult(payload.ToJsonString(), false));
        }

        private static JsonArray ToArray(string[] items)
        {
            var a = new JsonArray();
            foreach (var s in items) a.Add(s);
            return a;
        }

        private static JsonObject ToolResult(string text, bool isError) => new JsonObject
        {
            ["content"] = new JsonArray(new JsonObject { ["type"] = "text", ["text"] = text }),
            ["isError"] = isError
        };

        private static JsonObject Result(JsonNode id, JsonNode result) => new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id,
            ["result"] = result
        };

        private static JsonObject Error(JsonNode id, int code, string message) => new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id,
            ["error"] = new JsonObject { ["code"] = code, ["message"] = message }
        };
    }
}
