using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Word.Helpers;
using Word.Modules;
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;
using Tables = Word.Modules.Tables;

namespace Word.Forms
{
    // TODO: Review, test, add comments, and refactor

    internal partial class ShortcutsForm : Form
    {
        private readonly string registryPath = @"Software\Macaroni\QuickActions";

        public ShortcutsForm()
        {
            InitializeComponent();
        }

        private void QuickActionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save position
            RegistryHelper.SaveValue(registryPath, "PositionLeft", Left);
            RegistryHelper.SaveValue(registryPath, "PositionTop", Top);
            RegistryHelper.SaveValue(registryPath, "PositionWidth", Width);
            RegistryHelper.SaveValue(registryPath, "PositionHeight", Height);
        }

        private void button_GlobalUnhide_Click(object sender, EventArgs e)
        {
            Modules.Text.GlobalUnhide();
        }

        private void button_KeepWithNext_Click(object sender, EventArgs e)
        {
            Shared.RunWithUndo("Toggle Keep With Next", "Error", () =>
            {
                if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

                var app = Globals.ThisAddIn.Application;

                var sel = Globals.ThisAddIn.Application.Selection;
                var paragraphs = sel.Paragraphs;

                var shouldEnable = paragraphs.Cast<Paragraph>().Any(p => p.Format.KeepWithNext != -1);

                // Determine if we should enable it (if any paragraph doesn't have it)

                // Apply setting
                foreach (Paragraph p in paragraphs)
                {
                    p.Format.KeepWithNext = shouldEnable ? -1 : 0;
                    p.Format.KeepTogether = shouldEnable ? -1 : 0;
                }

                app.Activate();
            });
        }

        private void button_PageBreakBefore_Click(object sender, EventArgs e)
        {
            Shared.RunWithUndo("Toggle Page Break Before", "Error", () =>
            {
                if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

                var app = Globals.ThisAddIn.Application;

                var sel = Globals.ThisAddIn.Application.Selection;
                var paragraphs = sel.Paragraphs;

                var shouldEnable = false;

                // Determine if we should enable it (if any paragraph doesn't have it)
                foreach (Paragraph p in paragraphs)
                {
                    if (p.Format.PageBreakBefore != -1)
                    {
                        shouldEnable = true;
                        break;
                    }
                }

                // Apply setting
                foreach (Paragraph p in paragraphs)
                {
                    p.Format.PageBreakBefore = shouldEnable ? -1 : 0;
                }

                app.Activate();
            });
        }

        private void QuickShortcutsForm_Load(object sender, EventArgs e)
        {
            // Load saved values or use current size/position
            var x = RegistryHelper.LoadValue(registryPath, "PositionLeft", Left);
            var y = RegistryHelper.LoadValue(registryPath, "PositionTop", Top);
            var width = RegistryHelper.LoadValue(registryPath, "PositionWidth", Width);
            var height = RegistryHelper.LoadValue(registryPath, "PositionHeight", Height);

            // Apply bounds
            SetBounds(x, y, width, height);
        }

        private void button_FontSpacing0_Click(object sender, EventArgs e)
        {
            Shared.RunWithUndo("Set Font Spacing to 0", "Error", () => {
                var app = Globals.ThisAddIn.Application;
                if (app == null || app.Selection == null) return;
                var sel = app.Selection;
                sel.Font.Spacing = 0f;
                app.Activate();
            });
        }

        private void button_FontSpacingDecrease_Click(object sender, EventArgs e)
        {
            Shared.RunWithUndo("Decrease Font Spacing", "Error", () => {
                var app = Globals.ThisAddIn.Application;
                if (app == null || app.Selection == null) return;

                var sel = app.Selection;
                sel.Font.Spacing -= 0.1f;

                app.Activate();
            });
        }

        private void button_FontSpacingIncrease_Click(object sender, EventArgs e)
        {
            Shared.RunWithUndo("Increase Font Spacing", "Error", () => {
                var app = Globals.ThisAddIn.Application;
                if (app == null || app.Selection == null) return;

                var sel = app.Selection;
                sel.Font.Spacing += 0.1f;

                app.Activate();
            });

        }

        private void button_NewTextbox_Click(object sender, EventArgs e)
        {
            Shared.RunWithUndo("Insert New Textbox", "Error", () =>
            {
                var app = Globals.ThisAddIn.Application;
                var sel = app.Selection;
                if (sel == null) return;

                var doc = app.ActiveDocument;

                var shapeWidth = 100f;
                var shapeHeight = 20f;

                var shape = doc.Shapes.AddTextbox(
                    MsoTextOrientation.msoTextOrientationHorizontal,
                    0, 0, shapeWidth, shapeHeight,
                    sel.Range);

                var frame = shape.TextFrame;
                var text = frame.TextRange;
                text.Text = "Textbox";

                // Font
                text.Font.Name = "Arial";
                text.Font.Size = 10;

                // Paragraph formatting
                text.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                text.ParagraphFormat.LineSpacing = 12f; // 1 line at 10pt
                text.ParagraphFormat.SpaceBefore = 0;
                text.ParagraphFormat.SpaceAfter = 0;
                text.ParagraphFormat.LeftIndent = 0;
                text.ParagraphFormat.RightIndent = 0;

                // Textbox appearance
                frame.AutoSize = (int)MsoTriState.msoTrue;
                frame.MarginLeft = 0;
                frame.MarginRight = 0;
                frame.MarginTop = 0;
                frame.MarginBottom = 0;
                shape.Line.Visible = MsoTriState.msoFalse;
                shape.Fill.Visible = MsoTriState.msoFalse;

                // Center on page
                shape.RelativeHorizontalPosition = WdRelativeHorizontalPosition.wdRelativeHorizontalPositionPage;
                shape.RelativeVerticalPosition = WdRelativeVerticalPosition.wdRelativeVerticalPositionPage;
                shape.Left = (float)WdShapePosition.wdShapeCenter;
                shape.Top = (float)WdShapePosition.wdShapeCenter;

                // Focus the textbox
                frame.TextRange.Select();
                app.Activate();
            });
        }

        private void button_MergeCells_Click(object sender, EventArgs e)
        {
            Shared.RunWithUndo("Merge Cells", "Error", () =>
            {
                var sel = Globals.ThisAddIn.Application.Selection;
                if (sel.Cells.Count > 1)
                {
                    sel.Cells.Merge();
                }
            });
        }
        private void button_SplitCellsV_Click(object sender, EventArgs e)
        {
            Shared.RunWithUndo("Split Cells Vertically", "Error", () =>
            {
                var sel = Globals.ThisAddIn.Application.Selection;

                // Copy cells to list to avoid modification issues
                var cells = sel.Cells.Cast<Cell>().ToList();

                foreach (var cell in cells)
                {
                    var row = cell.RowIndex;
                    var col = cell.ColumnIndex;

                    var table = cell.Range.Tables[1];
                    cell.Split(1, 2); // 1 row, 2 columns

                    var leftCell = table.Cell(row, col);
                    var rightCell = table.Cell(row, col + 1);

                    leftCell.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleNone;
                    rightCell.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleNone;
                }
            });
        }

        private void button_SplitCellsH_Click(object sender, EventArgs e)
        {
            Shared.RunWithUndo("Split Cells Horizontally", "Error", () =>
            {
                var sel = Globals.ThisAddIn.Application.Selection;

                var cells = sel.Cells.Cast<Cell>().ToList();

                foreach (var cell in cells)
                {
                    var row = cell.RowIndex;
                    var col = cell.ColumnIndex;

                    var table = cell.Range.Tables[1];
                    cell.Split(2, 1); // 2 rows, 1 column

                    var topCell = table.Cell(row, col);
                    var bottomCell = table.Cell(row + 1, col);

                    topCell.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleNone;
                    bottomCell.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;
                }
            });
        }
        
        private void button_GlobalAutofit_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            Shared.ApplyGlobally(doc, Modules.Text.AutofitTextboxes);
        }

        private void button_FixTable_Click(object sender, EventArgs e)
        {
            const string bakeTablesPath = @"Software\Macaroni\TableConverter";

            var setCustomMargins = RegistryHelper.LoadValue(bakeTablesPath, "DoSetCustomMargins", false);

            Tables.Prepare(
                doAutofitWindow: RegistryHelper.LoadValue(bakeTablesPath, "DoAutofitWindow", true),
                doAutofitRowHeight: RegistryHelper.LoadValue(bakeTablesPath, "DoAutofitRowHeight", true),
                doDoNotResizeToFitContents: RegistryHelper.LoadValue(bakeTablesPath, "DoDoNotResizeToFitContents", true),
                doRemoveSpacingBeforeAfter: RegistryHelper.LoadValue(bakeTablesPath, "DoRemoveSpacingBeforeAfter", true),
                doDoNotBreakAcrossPages: RegistryHelper.LoadValue(bakeTablesPath, "DoDoNotBreakAcrossPages", true),

                doSetZeroMargins: RegistryHelper.LoadValue(bakeTablesPath, "DoSetZeroMargins", false),
                doSetDefaultMargins: RegistryHelper.LoadValue(bakeTablesPath, "DoSetDefaultMargins", false),
                doResetPaginationSettings: RegistryHelper.LoadValue(bakeTablesPath, "DoResetPaginationSettings", true),
                doRemoveBorders: RegistryHelper.LoadValue(bakeTablesPath, "DoRemoveBorders", false),
                convertSelectionOnly: true,

                doSetCustomMargins: setCustomMargins,
                customMarginTop: RegistryHelper.LoadValue(bakeTablesPath, "MarginTop", 0f),
                customMarginBottom: RegistryHelper.LoadValue(bakeTablesPath, "MarginBottom", 0f),
                customMarginLeft: RegistryHelper.LoadValue(bakeTablesPath, "MarginLeft", 0f),
                customMarginRight: RegistryHelper.LoadValue(bakeTablesPath, "MarginRight", 0f));
        }

        private void button_TogglePreview_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            var app = Globals.ThisAddIn.Application;

            var view = app.ActiveWindow.View;

            if (view.TableGridlines == view.ShowAll && view.ShowAll == view.ShowCropMarks)
            {
                view.TableGridlines = !view.TableGridlines;
                view.ShowAll = !view.ShowAll;
                view.ShowCropMarks = !view.ShowCropMarks;
            }
            else
            {
                var state = view.ShowAll;
                view.TableGridlines = state;
                view.ShowAll = state;
                view.ShowCropMarks = state;
            }
        }

        private void button_SplitCell_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            var app = Globals.ThisAddIn.Application;

            app.Dialogs[WdWordDialog.wdDialogTableSplitCells].Show();
        }

        private void button_GlobalHide_Click(object sender, EventArgs e)
        {
            Modules.Text.GlobalHide();
        }

        private void button_SelectionPane_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            Globals.ThisAddIn.Application.CommandBars.ExecuteMso("SelectionPane");
        }

        private void button_FormatPane_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            Globals.ThisAddIn.Application.CommandBars.ExecuteMso("ObjectFormatDialog");
        }

        private void button_InsertTable_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            Globals.ThisAddIn.Application.Dialogs[WdWordDialog.wdDialogTableInsertTable].Show();
        }

        private void button_TableProperties_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            Globals.ThisAddIn.Application.Dialogs[WdWordDialog.wdDialogTableProperties].Show();

        }

        private void button_TableOptions_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            Globals.ThisAddIn.Application.Dialogs[WdWordDialog.wdDialogTableTableOptions].Show();
        }

        private void button_StylesPane_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            Globals.ThisAddIn.Application.CommandBars.ExecuteMso("StylesPane");
        }
    }
}
