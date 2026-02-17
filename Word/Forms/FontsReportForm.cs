using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Word.Helpers;
using Word.Modules;
using Word.Types;

namespace Word.Forms
{
    public partial class FontsReportForm : Form
    {
        public FontsReportForm()
        {
            InitializeComponent();
        }

        private void LoadFontsData()
        {
            var documentFonts = Fonts.GetDocumentFonts();

            var rows = documentFonts
                .Where(font => !font.StartsWith("@")) // exclude verticals
                .Select(font =>
                {
                    var path = Fonts.GetFontPathFromRegistry(font);

                    return new FontInfo
                    {
                        Name = font,
                        Installed = !string.IsNullOrEmpty(path),
                        System = Fonts.WindowsDefaultFonts.Contains(font),
                        Type = string.IsNullOrEmpty(path) ?
                        "Unknown" :
                        Path.GetExtension(path).ToUpper().TrimStart('.')
                    };
                })
                .OrderBy(f => f.Installed)
                .ThenBy(f => f.Name)
                .ToList();

            dataGridView_FontTable.DataSource = rows;

            // Disable column sorting by user
            foreach (DataGridViewColumn column in dataGridView_FontTable.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }


            dataGridView_FontTable.Columns[0].FillWeight = 100;                                         // fills leftover space
            dataGridView_FontTable.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;   // fits content
            dataGridView_FontTable.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;   // fits content
            dataGridView_FontTable.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;   // fits content
        }

        private void FontsForm_Load(object sender, EventArgs e)
        {
            LoadFontsData();

            // load the checkbox state from the registry
            const string fontCheckerPath = @"Software\Macaroni\FontChecker";
            checkBox_CollectDefaultFonts.Checked = RegistryHelper.LoadValue(fontCheckerPath, "CollectDefaultFonts", false);
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            var doc = Globals.ThisAddIn.Application.ActiveDocument;
            doc.Save();
            LoadFontsData();
        }

        private void button_CollectFonts_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            var fontInfos = dataGridView_FontTable.Rows
                .OfType<DataGridViewRow>()
                .Select(row => row.DataBoundItem as FontInfo)
                .Where(fontInfo => fontInfo != null)
                .ToList();

            var anyMissing = fontInfos.Any(f => !f.Installed);
            var anyPresent = fontInfos.Any(f => f.Installed);

            if (!anyPresent)
            {
                MessageBox.Show(
                    "All fonts are missing, nothing will be collected.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (anyMissing)
            {
                MessageBox.Show(
                    "Some fonts are missing, they will not be collected.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

            Fonts.CollectFonts(checkBox_CollectDefaultFonts.Checked);
        }

        private void dataGridView_FontTable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                var selectedRows = dataGridView_FontTable.SelectedRows
                    .OfType<DataGridViewRow>()
                    .OrderBy(r => r.Index);

                var lines = selectedRows
                    .Select(r => r.Cells[0].Value?.ToString())
                    .Where(v => !string.IsNullOrEmpty(v));

                Clipboard.SetText(string.Join(Environment.NewLine, lines));

                e.Handled = true;
            }
        }

        private void FontsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string fontCheckerPath = @"Software\Macaroni\FontChecker";

            // save the checkbox state to the registry
            RegistryHelper.SaveValue(fontCheckerPath, "CollectDefaultFonts", checkBox_CollectDefaultFonts.Checked);
        }
    }
}
