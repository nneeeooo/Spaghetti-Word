using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using Word.Helpers;

namespace Word.Mcp
{
    /// <summary>Builds the full tool set (Word, forms, actions).</summary>
    internal static class McpToolSet
    {
        internal static ToolRegistry Create(FormRegistry forms)
        {
            var r = new ToolRegistry();
            WordTools.Register(r);
            FormTools.Register(r, forms);
            ActionTools.Register(r);
            return r;
        }
    }

    /// <summary>Owns the server lifetime: start/stop, mcp.json, registry flag. Off by default.</summary>
    internal static class McpHost
    {
        internal const string RegistryPath = @"Software\Macaroni\Mcp";
        internal const int DefaultPort = 39181;

        private static McpServer _server;

        internal static bool Running => _server != null;
        internal static string Url => _server?.Url;

        internal static string InfoPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Spaghetti", "mcp.json");

        internal static bool EnabledInRegistry
        {
            get => RegistryHelper.LoadValue(RegistryPath, "Enabled", false);
            set => RegistryHelper.SaveValue(RegistryPath, "Enabled", value);
        }

        /// <summary>Starts the server; must be called on the UI thread (captures its synchronization context).</summary>
        internal static void Start(SynchronizationContext ui)
        {
            if (_server != null) return;
            var s = new McpServer(McpToolSet.Create(FormRegistry.Instance), ui);
            s.Start(DefaultPort);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(InfoPath));
                var info = new JsonObject
                {
                    ["url"] = s.Url,
                    ["pid"] = System.Diagnostics.Process.GetCurrentProcess().Id
                };
                File.WriteAllText(InfoPath, info.ToJsonString());
            }
            catch
            {
                s.Stop();
                throw;
            }
            _server = s;
        }

        internal static void Stop()
        {
            var s = _server;
            _server = null;
            s?.Stop();
            try { if (File.Exists(InfoPath)) File.Delete(InfoPath); } catch { }
        }
    }
}
