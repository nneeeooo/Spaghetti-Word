using System;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;

using Word.Helpers;
namespace Word.Modules
{
    public static class Tables
    {
        /// <summary>
        /// Converts all, selected, or one given (onlyTable) tables in active document by applying various formatting and cleanup options.
        /// </summary>
        internal static void Prepare(
            bool doAutofitWindow,
            bool doAutofitRowHeight,
            bool doDoNotResizeToFitContents,
            bool doRemoveSpacingBeforeAfter,
            bool doDoNotBreakAcrossPages,

            bool doSetZeroMargins,
            bool doSetDefaultMargins,
            bool doSetCustomMargins,

            bool doResetPaginationSettings,
            bool doRemoveBorders,

            bool convertSelectionOnly = false,

            float customMarginTop = 0f,
            float customMarginBottom = 0f,
            float customMarginLeft = 0f,
            float customMarginRight = 0f,

            Table onlyTable = null,
            string undoName = null
        )
        {
            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            Shared.EnsurePrintViewAndCloseSplit(app);

            var undo = app.UndoRecord;
            if (!undo.IsRecordingCustomRecord)
                undo.StartCustomRecord(undoName ?? "Table convert");

            try
            {
                app.ScreenUpdating = false;

                // Локальна функція для обробки таблиці
                void ProcessTable(Table table)
                {
                    if (doAutofitWindow) AutofitWindow(table);
                    if (doAutofitRowHeight) AutofitRowHeight(table);
                    if (doDoNotResizeToFitContents) DoNotResizeToFitContents(table);
                    if (doRemoveSpacingBeforeAfter) RemoveSpacingBeforeAfter(table);
                    if (doDoNotBreakAcrossPages) DoNotBreakAcrossPages(table);

                    if (doSetDefaultMargins) SetDefaultMargins(table);
                    if (doSetZeroMargins) SetZeroMargins(table);
                    if (doSetCustomMargins) SetCustomMargins(table, customMarginTop, customMarginBottom, customMarginLeft, customMarginRight);

                    if (doResetPaginationSettings) ResetPaginationSettings(table);
                    if (doRemoveBorders) RemoveBorders(table);
                }

                if (onlyTable != null)
                {
                    ProcessTable(onlyTable);
                }
                else if (convertSelectionOnly)
                {
                    foreach (Table table in app.Selection.Range.Tables)
                        ProcessTable(table);
                }
                else
                {
                    Shared.ApplyGlobally(doc, range =>
                    {
                        // Index loop, Count re-read each pass: changing a table can make Word split it in two,
                        // and a foreach over the Tables snapshot would miss the new half.
                        for (var i = 1; i <= range.Tables.Count; i++)
                            ProcessTable(range.Tables[i]);
                    });
                }
            }
            catch (Exception ex)
            {
                UiPrompt.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (undo.IsRecordingCustomRecord)
                    undo.EndCustomRecord();

                GC.WaitForPendingFinalizers();
                GC.Collect();

                app.ScreenUpdating = true;
            }
        }

        /// <summary>
        /// The table the selection is in (first table of the selection). For a nested table, the outermost one.
        /// Null when the selection has no table.
        /// </summary>
        internal static Table GetSelectionOuterTable(Selection selection)
        {
            if (selection.Tables.Count == 0) return null;

            var table = selection.Tables[1];
            if (table.NestingLevel <= 1) return table;

            var start = table.Range.Start;
            var end = table.Range.End;
            foreach (Table top in selection.Range.Document.Tables)
            {
                if (top.NestingLevel == 1 && top.Range.StoryType == table.Range.StoryType && top.Range.Start <= start && top.Range.End >= end)
                    return top;
            }
            return table;
        }

        /// <summary>
        /// Sets the row height to at least 0 mm so it autofits the content.
        /// </summary>
        private static void AutofitRowHeight(Table table)
        {
            table.Rows.HeightRule = WdRowHeightRule.wdRowHeightAtLeast;
            table.Rows.Height = 0f;
        }

        /// <summary>
        /// Prevents all rows in the specified table from being split across pages when rendered or printed.
        /// </summary>
        private static void DoNotBreakAcrossPages(Table table)
        {
            table.Rows.AllowBreakAcrossPages = 0;
        }

        /// <summary>
        /// Sets the table to autofit to the window width.
        /// </summary>
        private static void AutofitWindow(Table table)
        {
            table.AutoFitBehavior(WdAutoFitBehavior.wdAutoFitWindow);
        }

        /// <summary>
        /// Disable "Resize to fit contents" for the table.
        /// </summary>
        private static void DoNotResizeToFitContents(Table table)
        {
            table.AllowAutoFit = false;
        }

        /// <summary>
        /// Remove spacing before and after paragraphs in all cells of the table.
        /// </summary>
        private static void RemoveSpacingBeforeAfter(Table table)
        {
            var format = table.Range.ParagraphFormat;
            format.SpaceBefore = 0f;
            format.SpaceAfter = 0f;
        }

        /// <summary>
        /// Sets all table cell margins to 0mm.
        /// </summary>
        private static void SetZeroMargins(Table table)
        {
            table.LeftPadding = 0f;
            table.RightPadding = 0f;
            table.TopPadding = 0f;
            table.BottomPadding = 0f;
        }

        /// <summary>
        /// Sets all table cell margins to custom values.
        /// </summary>
        private static void SetCustomMargins(Table table, float top, float bottom, float left, float right)
        {
            table.LeftPadding = left * 2.83465f;
            table.RightPadding = right * 2.83465f;
            table.TopPadding = top * 2.83465f;
            table.BottomPadding = bottom * 2.83465f;
        }

        /// <summary>
        /// Sets default table cell margins of 1.9mm left and right, and 0mm top and bottom.
        /// </summary>
        private static void SetDefaultMargins(Table table)
        {
            table.LeftPadding = 5.4f;
            table.RightPadding = 5.4f;
            table.TopPadding = 0f;
            table.BottomPadding = 0f;
        }

        /// <summary>
        /// Remove keep with next, keep lines together, and page break before settings from all paragraphs in the table.
        /// </summary>
        private static void ResetPaginationSettings(Table table)
        {
            var format = table.Range.ParagraphFormat;
            format.KeepWithNext = 0;       // Disable Keep with next
            format.KeepTogether = 0;       // Disable Keep lines together
            format.PageBreakBefore = 0;    // Disable Page break before
        }

        /// <summary>
        /// Remove table borders.
        /// </summary>
        private static void RemoveBorders(Table table)
        {
            table.Borders.Enable = 0;
        }
    }
}
