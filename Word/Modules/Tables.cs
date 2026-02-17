using System;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;

namespace Word.Modules
{
    public static class Tables
    {
        /// <summary>
        /// Converts all or selected tables in active document by applying various formatting and cleanup options.
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
            float customMarginRight = 0f
        )
        {
            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            Shared.EnsurePrintViewAndCloseSplit(app);

            var undo = app.UndoRecord;
            if (!undo.IsRecordingCustomRecord)
                undo.StartCustomRecord("Table convert");

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

                if (convertSelectionOnly)
                {
                    foreach (Table table in app.Selection.Range.Tables)
                        ProcessTable(table);
                }
                else
                {
                    Shared.ApplyGlobally(doc, range =>
                    {
                        foreach (Table table in range.Tables)
                            ProcessTable(table);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
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
            foreach (Row row in table.Rows)
            {
                row.AllowBreakAcrossPages = 0;
            }
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
            foreach (Cell cell in table.Range.Cells)
            {
                foreach (Paragraph para in cell.Range.Paragraphs)
                {
                    para.Format.SpaceBefore = 0f;
                    para.Format.SpaceAfter = 0f;
                }
            }
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
            foreach (Cell cell in table.Range.Cells)
            {
                foreach (Paragraph para in cell.Range.Paragraphs)
                {
                    para.Format.KeepWithNext = 0;       // Disable Keep with next
                    para.Format.KeepTogether = 0;       // Disable Keep lines together
                    para.Format.PageBreakBefore = 0;    // Disable Page break before
                }
            }
        }

        /// <summary>
        /// Remove table borders.
        /// </summary>
        private static void RemoveBorders(Table table)
        {
            table.Borders.Enable = 0;
        }

        /// <summary>
        /// Copies the width of the selected table cells to the clipboard in mm
        /// </summary>
        internal static void CopyCellWidthToClipboard()
        {
            var app = Globals.ThisAddIn.Application;
            var sel = app.Selection;

            if (sel.Cells.Count == 0) return;

            var firstCell = sel.Cells[1];
            var table = firstCell.Range.Tables[1];

            var minCol = int.MaxValue;
            var maxCol = int.MinValue;

            // Find min/max column index in selection
            foreach (Cell cell in sel.Cells)
            {
                if (cell.ColumnIndex < minCol) minCol = cell.ColumnIndex;
                if (cell.ColumnIndex > maxCol) maxCol = cell.ColumnIndex;
            }

            var totalWidthPt = 0f;
            // Use Columns collection to get reliable widths
            for (var i = minCol; i <= maxCol && i <= table.Columns.Count; i++)
            {
                totalWidthPt += table.Columns[i].Width;
            }

            var totalWidthMm = totalWidthPt * 0.3528f;
            Clipboard.SetText($"{Math.Round(totalWidthMm, 1)} mm");
        }
    }
}
