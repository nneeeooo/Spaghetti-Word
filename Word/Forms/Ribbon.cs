using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;
using Word.Forms;
using Word.Modules;

namespace Word
{
    public partial class Ribbon
    {
        private SpotlightForm spotlightForm;
        private WordPrepForm conversionForm;
        private FontsReportForm fontsForm;
        private ShortcutsForm shortcutsForm;
        private SubtitleToolsForm subsForm;
        private FontChangerForm fontChangerForm;
        private AboutForm aboutForm;
        private ColorPickerForm colorPickerForm;

        public static Ribbon Instance { get; private set; }

        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            // Forms are created on first use by SpawnForm. Creating them here runs at Word startup,
            // when no document may be open yet (ActiveDocument throws then).
            Instance = this;
        }

        public static T SpawnForm<T>(ref T form) where T : Form, new()
        {
            if (!Shared.HasActiveDocument())
                return form;

            if (form == null || form.IsDisposed)
                form = new T();

            if (!form.Visible)
                form.Show();
            else
                form.Focus();

            return form;
        }

        private void buttonHLForm_Click(object sender, RibbonControlEventArgs e)
        {
            SpawnForm(ref spotlightForm);
        }

        private void buttonConversionForm_Click(object sender, RibbonControlEventArgs e)
        {
            SpawnForm(ref conversionForm);
        }

        private void button_FontChecker_Click(object sender, RibbonControlEventArgs e)
        {
            SpawnForm(ref fontsForm);
        }
        private void button_ExtractAllImages_Click(object sender, RibbonControlEventArgs e)
        {
            if (!Shared.HasActiveDocument()) return;

            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            // An unsaved document has no folder; FullName is just "Document1".
            if (string.IsNullOrEmpty(doc.Path))
            {
                MessageBox.Show(
                    "Please save the document before extracting images.",
                    "Extraction Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // ask for confirmation before extracting all images
            if (MessageBox.Show(
                    "Extract all images?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                ) == DialogResult.No) return;

            var docxPath = doc.FullName;
            var outputFolder = Path.Combine(doc.Path, Path.GetFileNameWithoutExtension(docxPath) + "_images");
            var closed = false;

            try
            {
                // Save and close the document
                doc.Save();
                doc.Close();
                closed = true;

                // Wait briefly to ensure file unlock
                System.Threading.Thread.Sleep(500);

                Images.ExtractImages(docxPath, outputFolder);

                MessageBox.Show("Images extracted succesfully!", "Extraction Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start(new ProcessStartInfo
                {
                    FileName = outputFolder,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting images:\n" + ex.Message, "Extraction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Reopen the document, only if we closed it
                if (closed)
                    app.Documents.Open(docxPath);
            }
        }

        private void button_Shortcuts_Click(object sender, RibbonControlEventArgs e)
        {
            SpawnForm(ref shortcutsForm);
        }

        private void button_SubtitleTools_Click(object sender, RibbonControlEventArgs e)
        {
            SpawnForm(ref subsForm);
        }

        private void button_FontChanger_Click(object sender, RibbonControlEventArgs e)
        {
            SpawnForm(ref fontChangerForm);
        }

        private void button2_Click(object sender, RibbonControlEventArgs e)
        {
            SpawnForm(ref aboutForm);
        }

        private void button_ColorPicker_Click(object sender, RibbonControlEventArgs e)
        {
            SpawnForm(ref colorPickerForm);
        }
    }
}
