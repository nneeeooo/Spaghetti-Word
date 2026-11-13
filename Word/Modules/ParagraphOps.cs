using System;
using System.Linq;
using Microsoft.Office.Interop.Word;
using Word.Helpers;
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;

namespace Word.Modules
{
    internal static class ParagraphOps
    {
        internal const string PageBreakBeforeUndoName = "Toggle Page Break Before";

        /// <summary>
        /// Turns a paragraph flag on for the selected paragraphs if any of them lacks it, otherwise off.
        /// </summary>
        internal static void ToggleParagraphFlag(string undoName, Func<ParagraphFormat, int> get, Action<ParagraphFormat, int> set)
        {
            Shared.RunWithUndo(undoName, "Error", () =>
            {
                var app = Globals.ThisAddIn.Application;
                var paragraphs = app.Selection.Paragraphs;

                var shouldEnable = paragraphs.Cast<Paragraph>().Any(p => get(p.Format) != -1);

                foreach (Paragraph p in paragraphs)
                    set(p.Format, shouldEnable ? -1 : 0);

                app.Activate();
            });
        }

        internal static void TogglePageBreakBefore() =>
            ToggleParagraphFlag(PageBreakBeforeUndoName, f => f.PageBreakBefore, (f, v) => f.PageBreakBefore = v);

        /// <summary>True when every selected paragraph has Page break before.</summary>
        internal static bool AllSelectedHavePageBreakBefore()
        {
            if (!Shared.HasActiveDocument()) return false;
            var paragraphs = Globals.ThisAddIn.Application.Selection.Paragraphs;
            return paragraphs.Count > 0 && paragraphs.Cast<Paragraph>().All(p => p.Format.PageBreakBefore == -1);
        }
    }
}
