using System.Windows.Forms;
using Word.Mcp;

namespace Word.Helpers
{
    /// <summary>
    /// Drop-in for MessageBox.Show. While an MCP tool call is executing the message is recorded into the
    /// tool result instead of blocking Word with a modal box. Normal UI use shows the real MessageBox.
    /// </summary>
    internal static class UiPrompt
    {
        internal static DialogResult Show(string text, string caption = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            if (!McpCallContext.Active)
                return MessageBox.Show(text, caption, buttons, icon);

            McpCallContext.Record(string.IsNullOrEmpty(caption) ? text : caption + ": " + text);

            // Never auto-confirm a question: answer the safe way.
            switch (buttons)
            {
                case MessageBoxButtons.YesNo: return DialogResult.No;
                case MessageBoxButtons.YesNoCancel:
                case MessageBoxButtons.OKCancel: return DialogResult.Cancel;
                case MessageBoxButtons.RetryCancel: return DialogResult.Cancel;
                case MessageBoxButtons.AbortRetryIgnore: return DialogResult.Abort;
                default: return DialogResult.OK;
            }
        }
    }
}
