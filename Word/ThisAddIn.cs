using System;
using System.Threading;
using System.Windows.Forms;
using Word.Mcp;

namespace Word
{
    public partial class ThisAddIn
    {
        // UI-thread context captured at startup; MCP tools are marshalled onto it.
        private SynchronizationContext uiContext;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
            uiContext = SynchronizationContext.Current;

            // MCP server is OFF unless the user turned it on from the ribbon.
            if (McpHost.EnabledInRegistry)
            {
                try
                {
                    McpHost.Start(uiContext);
                }
                catch (Exception ex)
                {
                    McpHost.EnabledInRegistry = false;
                    System.Diagnostics.Debug.WriteLine("MCP server did not start: " + ex.Message);
                }
                Ribbon.Instance?.SyncMcpToggle();
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            McpHost.Stop();
        }

        /// <summary>Starts or stops the MCP server and remembers the choice. Call on the UI thread.</summary>
        internal void SetMcp(bool enabled)
        {
            if (enabled)
            {
                McpHost.Start(uiContext ?? SynchronizationContext.Current);
                McpHost.EnabledInRegistry = true;
            }
            else
            {
                McpHost.Stop();
                McpHost.EnabledInRegistry = false;
            }
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        #endregion
    }
}
