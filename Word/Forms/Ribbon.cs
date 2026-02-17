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
            Instance = this;

            spotlightForm = new SpotlightForm();
            conversionForm = new WordPrepForm();
            fontsForm = new FontsReportForm();
            shortcutsForm = new ShortcutsForm();
            subsForm = new SubtitleToolsForm();
            fontChangerForm = new FontChangerForm();
            aboutForm = new AboutForm();
            colorPickerForm = new ColorPickerForm();
        }

        public static T SpawnForm<T>(ref T form) where T : Form, new()
        {
            var app = Globals.ThisAddIn.Application;
            if (app?.ActiveDocument == null)
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
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            // ask for confirmation before extracting all images
            if (MessageBox.Show(
                    "Extract all images?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                ) == DialogResult.No) return;

            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            var docxPath = doc.FullName;

            var outputFolder = Path.Combine(
            Path.GetDirectoryName(docxPath) ?? throw new FileNotFoundException(),
            Path.GetFileNameWithoutExtension(docxPath) + "_images");

            try
            {
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

                // Save and close the document
                doc.Save();
                app.ActiveDocument.Close();

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
                // Reopen the document
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
