using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Word.Mcp
{
    /// <summary>Error whose message is safe to show to the AI client as-is (bad arguments, wrong state).</summary>
    internal sealed class ToolException : Exception
    {
        public ToolException(string message) : base(message) { }
    }

    /// <summary>
    /// Per-call scope set by the dispatcher while an MCP tool runs on the UI thread.
    /// UiPrompt records messages here instead of showing a modal MessageBox.
    /// </summary>
    internal static class McpCallContext
    {
        [ThreadStatic] private static List<string> _messages;

        internal static bool Active => _messages != null;

        internal static void Begin() => _messages = new List<string>();

        internal static List<string> End()
        {
            var m = _messages;
            _messages = null;
            return m ?? new List<string>();
        }

        internal static void Record(string message)
        {
            if (_messages != null && _messages.Count < 50) _messages.Add(message);
        }
    }

    internal sealed class ToolDef
    {
        public string Name;
        public string Description;
        public JsonObject InputSchema;
        public Func<JsonObject, JsonNode> Handler;
    }

    /// <summary>Explicit allow-list of tools. Nothing is resolved by reflection or by name over arbitrary types.</summary>
    internal sealed class ToolRegistry
    {
        private readonly Dictionary<string, ToolDef> _tools = new Dictionary<string, ToolDef>(StringComparer.Ordinal);
        private readonly List<string> _order = new List<string>();

        public IEnumerable<ToolDef> Tools => _order.Select(n => _tools[n]);

        public void Add(string name, string description, JsonObject schema, Func<JsonObject, JsonNode> handler)
        {
            _tools[name] = new ToolDef { Name = name, Description = description, InputSchema = schema, Handler = handler };
            if (!_order.Contains(name)) _order.Add(name);
        }

        public bool TryGet(string name, out ToolDef def) => _tools.TryGetValue(name ?? "", out def);
    }

    /// <summary>Tiny argument and JSON-schema helpers.</summary>
    internal static class J
    {
        public static JsonObject Schema(string[] required, params (string name, string type, string desc)[] props)
        {
            var p = new JsonObject();
            foreach (var (name, type, desc) in props)
                p[name] = new JsonObject { ["type"] = type, ["description"] = desc };
            var s = new JsonObject { ["type"] = "object", ["properties"] = p };
            if (required != null && required.Length > 0)
                s["required"] = new JsonArray(required.Select(r => (JsonNode)JsonValue.Create(r)).ToArray());
            return s;
        }

        public static JsonNode Get(JsonObject a, string key) => a != null && a.TryGetPropertyValue(key, out var v) ? v : null;

        public static string Str(JsonObject a, string key, bool required = false, string def = null)
        {
            var v = Get(a, key);
            if (v == null)
            {
                if (required) throw new ToolException($"Missing required argument '{key}'.");
                return def;
            }
            try { return v.GetValue<string>(); }
            catch { throw new ToolException($"Argument '{key}' must be a string."); }
        }

        public static bool Bool(JsonObject a, string key, bool def = false)
        {
            var v = Get(a, key);
            if (v == null) return def;
            try { return v.GetValue<bool>(); }
            catch { throw new ToolException($"Argument '{key}' must be a boolean."); }
        }

        public static int Int(JsonObject a, string key, int? def = null)
        {
            var v = Get(a, key);
            if (v == null)
            {
                if (def.HasValue) return def.Value;
                throw new ToolException($"Missing required argument '{key}'.");
            }
            try { return v.GetValue<int>(); }
            catch { throw new ToolException($"Argument '{key}' must be an integer."); }
        }

        public static int? IntOpt(JsonObject a, string key) => Get(a, key) == null ? (int?)null : Int(a, key);

        public static float Float(JsonObject a, string key, float def = 0f)
        {
            var v = Get(a, key);
            if (v == null) return def;
            try { return v.GetValue<float>(); }
            catch { throw new ToolException($"Argument '{key}' must be a number."); }
        }

        /// <summary>Boolean flag by case-insensitive name, with or without the 'do' prefix (trimSpaces == doTrimSpaces).</summary>
        public static bool Flag(JsonObject a, string name, bool def = false)
        {
            if (a == null) return def;
            foreach (var kv in a)
                if (string.Equals(kv.Key, name, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(kv.Key, "do" + name, StringComparison.OrdinalIgnoreCase))
                    return Bool(a, kv.Key, def);
            return def;
        }
    }
}
