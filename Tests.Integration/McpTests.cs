using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Word.Helpers;
using Word.Mcp;

namespace Tests.Integration
{
    /// <summary>
    /// End-to-end: real HTTP server + real hidden Word (the shim Globals) + tools marshalled to a dedicated STA UI thread
    /// with a message loop, like Word's own UI thread.
    /// </summary>
    [TestClass]
    public class McpTests
    {
        private static Thread _uiThread;
        private static SynchronizationContext _ui;
        private static McpServer _server;
        private static HttpClient _http;
        private static FormRegistry _forms;
        private static int _nextId = 1;
        private string _dir;

        [ClassInitialize]
        public static void Start(TestContext _)
        {
            var ready = new ManualResetEventSlim();
            _uiThread = new Thread(() =>
            {
                var ctx = new WindowsFormsSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(ctx);
                _ui = ctx;
                ready.Set();
                System.Windows.Forms.Application.Run();
            }) { IsBackground = true, Name = "test UI" };
            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.Start();
            Assert.IsTrue(ready.Wait(10000));

            _forms = new FormRegistry();
            _forms.Register("fake", open => FakeForm.Get(open));
            _server = new McpServer(McpToolSet.Create(_forms), _ui, timeoutMs: 60000);
            _server.Start(39400);
            _http = new HttpClient { Timeout = TimeSpan.FromSeconds(90) };
        }

        [ClassCleanup]
        public static void Stop()
        {
            _server?.Stop();
            _http?.Dispose();
            try { _ui?.Send(_ => System.Windows.Forms.Application.ExitThread(), null); } catch { }
        }

        [TestInitialize]
        public void Setup()
        {
            _dir = Path.Combine(Path.GetTempPath(), "spaghetti-mcp-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
            CloseAllDocs();
        }

        [TestCleanup]
        public void Teardown()
        {
            CloseAllDocs();
            try { Directory.Delete(_dir, true); } catch { }
        }

        private static void CloseAllDocs()
        {
            // through the server itself: proves doc_close works and runs on the UI thread
            for (var i = 0; i < 20; i++)
            {
                var list = Tool("doc_list", null);
                var docs = (JsonArray)list["documents"];
                if (docs.Count == 0) return;
                Tool("doc_close", new JsonObject { ["name"] = (string)docs[0]["name"], ["save"] = false });
            }
        }

        private static JsonObject Rpc(string method, JsonObject p)
        {
            var body = new JsonObject { ["jsonrpc"] = "2.0", ["id"] = _nextId++, ["method"] = method };
            if (p != null) body["params"] = p;
            var req = new HttpRequestMessage(HttpMethod.Post, _server.Url)
            {
                Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _server.Token);
            var resp = _http.SendAsync(req).Result;
            Assert.AreEqual(200, (int)resp.StatusCode);
            return (JsonObject)JsonNode.Parse(resp.Content.ReadAsStringAsync().Result);
        }

        /// <summary>Calls a tool, asserts it is not an error, returns the parsed JSON payload.</summary>
        private static JsonObject Tool(string name, JsonObject args, bool expectError = false)
        {
            var r = Rpc("tools/call", new JsonObject { ["name"] = name, ["arguments"] = args ?? new JsonObject() });
            Assert.IsNull(r["error"], r["error"]?.ToJsonString());
            var res = r["result"];
            var text = (string)res["content"][0]["text"];
            Assert.AreEqual(expectError, (bool)res["isError"], text);
            return (JsonObject)JsonNode.Parse(text);
        }

        [TestMethod, TestCategory("Word")]
        public void Protocol_Initialize_And_ToolsList()
        {
            var init = Rpc("initialize", new JsonObject { ["protocolVersion"] = "2025-06-18" });
            Assert.AreEqual("spaghetti-word", (string)init["result"]["serverInfo"]["name"]);

            var tools = (JsonArray)Rpc("tools/list", null)["result"]["tools"];
            var names = new System.Collections.Generic.HashSet<string>();
            foreach (var t in tools) names.Add((string)t["name"]);
            foreach (var n in new[] { "word_status", "doc_list", "doc_open", "doc_new", "doc_activate", "doc_close", "doc_save",
                "doc_get_text", "doc_insert_text", "doc_select", "doc_find", "form_list", "form_open", "form_close",
                "form_controls", "form_get", "form_set", "form_click", "action_list", "action_run", "undo" })
                Assert.IsTrue(names.Contains(n), "missing tool " + n);
        }

        [TestMethod, TestCategory("Word")]
        public void Document_NewInsertGetCloseRoundTrip()
        {
            var status = Tool("word_status", null);
            Assert.AreEqual(0, (int)status["documents"]);

            var doc = Tool("doc_new", null);
            Assert.IsFalse(string.IsNullOrEmpty((string)doc["name"]));

            Tool("doc_insert_text", new JsonObject { ["text"] = "hello world", ["at"] = "end" });
            Tool("doc_insert_text", new JsonObject { ["text"] = ">> ", ["at"] = "start" });
            var text = Tool("doc_get_text", null);
            StringAssert.StartsWith((string)text["text"], ">> hello world");

            var find = Tool("doc_find", new JsonObject { ["text"] = "o" });
            Assert.AreEqual(2, ((JsonArray)find["matches"]).Count);

            Tool("doc_select", new JsonObject { ["start"] = 3, ["end"] = 8 });
            Assert.AreEqual(3, (int)Tool("word_status", null)["selectionStart"]);

            var part = Tool("doc_get_text", new JsonObject { ["start"] = 3, ["end"] = 8 });
            Assert.AreEqual("hello", (string)part["text"]);

            var limited = Tool("doc_get_text", new JsonObject { ["maxChars"] = 4 });
            Assert.IsTrue((bool)limited["truncated"]);

            Tool("doc_close", new JsonObject { ["save"] = false });
            Assert.AreEqual(0, ((JsonArray)Tool("doc_list", null)["documents"]).Count);
        }

        [TestMethod, TestCategory("Word")]
        public void Document_SaveAs_Open_ValidatesPaths()
        {
            Tool("doc_new", null);
            Tool("doc_insert_text", new JsonObject { ["text"] = "persisted", ["at"] = "end" });
            var path = Path.Combine(_dir, "out.docx");
            Tool("doc_save", new JsonObject { ["path"] = path });
            Assert.IsTrue(File.Exists(path));
            Tool("doc_close", new JsonObject { ["save"] = true });

            var opened = Tool("doc_open", new JsonObject { ["path"] = path, ["readOnly"] = true });
            Assert.IsTrue((bool)opened["readOnly"]);
            StringAssert.StartsWith((string)Tool("doc_get_text", null)["text"], "persisted");
            Tool("doc_close", new JsonObject { ["save"] = false });

            Tool("doc_open", new JsonObject { ["path"] = "relative.docx" }, expectError: true);
            Tool("doc_open", new JsonObject { ["path"] = Path.Combine(_dir, "missing.docx") }, expectError: true);
            File.WriteAllText(Path.Combine(_dir, "evil.exe"), "x");
            Tool("doc_open", new JsonObject { ["path"] = Path.Combine(_dir, "evil.exe") }, expectError: true);
        }

        [TestMethod, TestCategory("Word")]
        public void Tool_Errors_AreIsError_NotCrash()
        {
            Tool("doc_get_text", null, expectError: true);            // no document open
            Tool("doc_activate", new JsonObject { ["name"] = "nope.docx" }, expectError: true);
            Tool("action_run", new JsonObject { ["action"] = "format.c" }, expectError: true);
            Tool("doc_insert_text", new JsonObject { ["text"] = "x" }, expectError: true); // missing 'at'
        }

        [TestMethod, TestCategory("Word")]
        public void Actions_List_Run_And_Undo()
        {
            var list = Tool("action_list", null);
            StringAssert.Contains(list.ToJsonString(), "spotlight.clear");

            Tool("doc_new", null);
            Tool("doc_insert_text", new JsonObject { ["text"] = "a  b", ["at"] = "end" });
            Tool("action_run", new JsonObject { ["action"] = "text.prepare", ["args"] = new JsonObject { ["trimSpaces"] = true } });
            StringAssert.StartsWith((string)Tool("doc_get_text", null)["text"], "a b");
            Assert.IsTrue((bool)Tool("undo", null)["undone"]);
            StringAssert.StartsWith((string)Tool("doc_get_text", null)["text"], "a  b");

            Tool("action_run", new JsonObject { ["action"] = "spotlight.clear" });
            Tool("action_run", new JsonObject { ["action"] = "spotlight.run", ["args"] = new JsonObject { ["fastDtp"] = true } });
            Tool("action_run", new JsonObject { ["action"] = "text.global_hide" });
            Tool("action_run", new JsonObject { ["action"] = "text.global_unhide" });
            var fonts = Tool("action_run", new JsonObject { ["action"] = "fonts.document_fonts" });
            Assert.IsNotNull(fonts["fonts"]);
            Tool("action_run", new JsonObject { ["action"] = "spotlight.clear" });
            Tool("doc_close", new JsonObject { ["save"] = false });

            // needs a document
            Tool("action_run", new JsonObject { ["action"] = "spotlight.clear" }, expectError: true);
        }

        [TestMethod, TestCategory("Word")]
        public void Action_SubtitlesConvert_NoWord()
        {
            var src = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples", "test.srt");
            var dst = Path.Combine(_dir, "out.vtt");
            Tool("action_run", new JsonObject
            {
                ["action"] = "subtitles.convert",
                ["args"] = new JsonObject { ["inputPath"] = src, ["targetExtension"] = "vtt", ["outputPath"] = dst }
            });
            StringAssert.StartsWith(File.ReadAllText(dst), "WEBVTT");
        }

        [TestMethod, TestCategory("Word")]
        public void Forms_List_Open_Inspect_Set_Click_Close()
        {
            var list = Tool("form_list", null);
            Assert.AreEqual("fake", (string)list["forms"][0]["name"]);
            Assert.IsFalse((bool)list["forms"][0]["open"]);

            Tool("form_controls", new JsonObject { ["form"] = "fake" }, expectError: true); // not open yet

            Tool("form_open", new JsonObject { ["form"] = "fake" });
            Assert.IsTrue((bool)Tool("form_list", null)["forms"][0]["open"]);

            var controls = (JsonArray)Tool("form_controls", new JsonObject { ["form"] = "fake" })["controls"];
            var byName = new System.Collections.Generic.Dictionary<string, JsonNode>();
            foreach (var c in controls) byName[(string)c["name"]] = c;
            Assert.AreEqual("CheckBox", (string)byName["chk"]["type"]);
            Assert.AreEqual(5m, (decimal)byName["num"]["value"]);
            Assert.AreEqual("b", (string)byName["combo"]["items"][1]);
            Assert.AreEqual(2, ((JsonArray)byName["grid"]["rows"]).Count);
            Assert.AreEqual("r2c2", (string)byName["grid"]["rows"][1][1]);

            Tool("form_set", new JsonObject { ["form"] = "fake", ["control"] = "chk", ["value"] = true });
            Tool("form_set", new JsonObject { ["form"] = "fake", ["control"] = "num", ["value"] = 9 });
            Tool("form_set", new JsonObject { ["form"] = "fake", ["control"] = "txt", ["value"] = "typed" });
            Tool("form_set", new JsonObject { ["form"] = "fake", ["control"] = "combo", ["value"] = "b" });
            Assert.IsTrue((bool)Tool("form_get", new JsonObject { ["form"] = "fake", ["control"] = "chk" })["checked"]);
            Assert.AreEqual("b", (string)Tool("form_get", new JsonObject { ["form"] = "fake", ["control"] = "combo" })["selectedItem"]);
            Tool("form_set", new JsonObject { ["form"] = "fake", ["control"] = "combo", ["value"] = "zzz" }, expectError: true);
            Tool("form_set", new JsonObject { ["form"] = "fake", ["control"] = "chk", ["value"] = "notabool" }, expectError: true);
            Tool("form_get", new JsonObject { ["form"] = "fake", ["control"] = "missing" }, expectError: true);

            // Button click runs the handler; its MessageBox is captured, not shown
            var clicked = Tool("form_click", new JsonObject { ["form"] = "fake", ["control"] = "btn" });
            StringAssert.Contains(clicked["uiMessages"][0].ToString(), "typed/9");
            Assert.AreEqual("done", (string)Tool("form_get", new JsonObject { ["form"] = "fake", ["control"] = "lbl" })["text"]);

            Tool("form_close", new JsonObject { ["form"] = "fake" });
            Assert.IsFalse((bool)Tool("form_list", null)["forms"][0]["open"]);
            Tool("form_open", new JsonObject { ["form"] = "nonexistent" }, expectError: true);
        }

        [TestMethod]
        public void Auth_Enforced_OverRealHttp()
        {
            var req = new HttpRequestMessage(HttpMethod.Post, _server.Url)
            {
                Content = new StringContent("{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"ping\"}", Encoding.UTF8, "application/json")
            };
            Assert.AreEqual(401, (int)_http.SendAsync(req).Result.StatusCode);
        }

        /// <summary>Stand-in for an add-in form, created on the UI thread by the registry provider.</summary>
        private sealed class FakeForm : Form
        {
            private static FakeForm _instance;

            internal static Form Get(bool open)
            {
                if (open && (_instance == null || _instance.IsDisposed))
                    _instance = new FakeForm();
                if (open && !_instance.Visible) _instance.Show();
                return _instance;
            }

            private FakeForm()
            {
                Name = "FakeForm";
                Text = "Fake";
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.Manual;
                Location = new System.Drawing.Point(-2000, -2000);

                var panel = new Panel { Name = "panel" };
                var chk = new CheckBox { Name = "chk", Text = "Check" };
                var num = new NumericUpDown { Name = "num", Minimum = 0, Maximum = 100, Value = 5 };
                var txt = new TextBox { Name = "txt" };
                var combo = new ComboBox { Name = "combo", DropDownStyle = ComboBoxStyle.DropDownList };
                combo.Items.AddRange(new object[] { "a", "b", "c" });
                var lbl = new Label { Name = "lbl", Text = "idle" };
                var grid = new DataGridView { Name = "grid", AllowUserToAddRows = false };
                grid.Columns.Add("c1", "C1");
                grid.Columns.Add("c2", "C2");
                grid.Rows.Add("r1c1", "r1c2");
                grid.Rows.Add("r2c1", "r2c2");
                var btn = new Button { Name = "btn", Text = "Go" };
                btn.Click += (s, e) =>
                {
                    UiPrompt.Show(txt.Text + "/" + num.Value, "Fake", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lbl.Text = "done";
                };
                panel.Controls.AddRange(new Control[] { chk, num, txt, combo, lbl, grid });
                Controls.Add(panel);
                Controls.Add(btn);
            }
        }
    }
}
