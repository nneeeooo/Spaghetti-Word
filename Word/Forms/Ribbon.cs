using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Word.Forms;
using Word.Helpers;
using Word.Mcp;
using Word.Modules;

namespace Word
{
    /// <summary>
    /// Ribbon tab and context menus, defined in Forms/Ribbon.xml (embedded resource).
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class Ribbon : IRibbonExtensibility
    {
        private SpotlightForm spotlightForm;
        private WordPrepForm conversionForm;
        private FontsReportForm fontsForm;
        private ShortcutsForm shortcutsForm;
        private SubtitleToolsForm subsForm;
        private FontChangerForm fontChangerForm;
        private AboutForm aboutForm;
        private ColorPickerForm colorPickerForm;

        private IRibbonUI ribbonUi;

        public static Ribbon Instance { get; private set; }

        public string GetCustomUI(string ribbonID)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Word.Ribbon.xml"))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        public void Ribbon_Load(IRibbonUI ribbon)
        {
            // Forms are created on first use by SpawnForm. Creating them here runs at Word startup,
            // when no document may be open yet (ActiveDocument throws then).
            ribbonUi = ribbon;
            Instance = this;
            RegisterMcpForms();
            SyncMcpToggle();
        }

        // ---- images: Ribbon.xml tag = resource name in Properties.Resources ----

        public stdole.IPictureDisp GetImage(IRibbonControl control)
        {
            Bitmap bmp;
            switch (control.Tag)
            {
                case "Etherbrian_Webuosities_Crossing1": bmp = Properties.Resources.Etherbrian_Webuosities_Crossing1; break;
                case "Folder_Transfer_32x32": bmp = Properties.Resources.Folder_Transfer_32x32; break;
                case "video__1": bmp = Properties.Resources.video__1; break;
                case "_32_x_32___BMP__4_bit_": bmp = Properties.Resources._32_x_32___BMP__4_bit_; break;
                case "font_tt_0": bmp = Properties.Resources.font_tt_0; break;
                case "Font_slide_32x32": bmp = Properties.Resources.Font_slide_32x32; break;
                case "Lamp_32x321": bmp = Properties.Resources.Lamp_32x321; break;
                case "abc1_BMP__8_bit___3__32x32": bmp = Properties.Resources.abc1_BMP__8_bit___3__32x32; break;
                case "Regedit_32x32_0": bmp = Properties.Resources.Regedit_32x32_0; break;
                default: return null;
            }
            return PictureConverter.ToPictureDisp(bmp);
        }

        private sealed class PictureConverter : AxHost
        {
            private PictureConverter() : base(string.Empty) { }

            internal static stdole.IPictureDisp ToPictureDisp(Image image) => (stdole.IPictureDisp)GetIPictureDispFromPicture(image);
        }

        // ---- MCP: fixed name -> form map (see Word/Mcp/FormTools.cs) ----

        private void RegisterMcpForms()
        {
            var r = FormRegistry.Instance;
            r.Register("spotlight", open => open ? SpawnForm(ref spotlightForm) : spotlightForm);
            r.Register("wordprep", open => open ? SpawnForm(ref conversionForm) : conversionForm);
            r.Register("fontsreport", open => open ? SpawnForm(ref fontsForm) : fontsForm);
            r.Register("fontchanger", open => open ? SpawnForm(ref fontChangerForm) : fontChangerForm);
            r.Register("subtitles", open => open ? SpawnForm(ref subsForm) : subsForm);
            r.Register("shortcuts", open => open ? SpawnForm(ref shortcutsForm) : shortcutsForm);
            r.Register("colorpicker", open => open ? SpawnForm(ref colorPickerForm) : colorPickerForm);
            r.Register("about", open => open ? SpawnForm(ref aboutForm) : aboutForm);
        }

        /// <summary>Refreshes the MCP toggle so it shows McpHost.Running.</summary>
        internal void SyncMcpToggle()
        {
            ribbonUi?.InvalidateControl("toggleButton_Mcp");
        }

        public bool Mcp_GetPressed(IRibbonControl control) => McpHost.Running;

        public void toggleButton_Mcp_Click(IRibbonControl control, bool pressed)
        {
            try
            {
                Globals.ThisAddIn.SetMcp(pressed);
            }
            catch (Exception ex)
            {
                UiPrompt.Show("MCP server: " + ex.Message, "MCP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SyncMcpToggle();
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

        // ---- context menus ----

        public void FixTable_Click(IRibbonControl control)
        {
            try
            {
                if (!Shared.HasActiveDocument()) return;
                var table = Tables.GetSelectionOuterTable(Globals.ThisAddIn.Application.Selection);
                if (table == null) return;
                TablePrepSettings.Load().Apply(onlyTable: table, undoName: "Fix table");
            }
            catch (Exception ex)
            {
                UiPrompt.Show(ex.Message, "Fix table", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool PageBreakBefore_GetPressed(IRibbonControl control)
        {
            try { return ParagraphOps.AllSelectedHavePageBreakBefore(); }
            catch { return false; }
        }

        public void PageBreakBefore_Click(IRibbonControl control, bool pressed)
        {
            ParagraphOps.TogglePageBreakBefore();
        }

        // ---- ribbon buttons ----

        public void buttonHLForm_Click(IRibbonControl control)
        {
            SpawnForm(ref spotlightForm);
        }

        public void buttonConversionForm_Click(IRibbonControl control)
        {
            SpawnForm(ref conversionForm);
        }

        public void button_FontChecker_Click(IRibbonControl control)
        {
            SpawnForm(ref fontsForm);
        }

        public void button_ExtractAllImages_Click(IRibbonControl control)
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

        public void button_Shortcuts_Click(IRibbonControl control)
        {
            SpawnForm(ref shortcutsForm);
        }

        public void button_SubtitleTools_Click(IRibbonControl control)
        {
            SpawnForm(ref subsForm);
        }

        public void button_FontChanger_Click(IRibbonControl control)
        {
            SpawnForm(ref fontChangerForm);
        }

        public void button2_Click(IRibbonControl control)
        {
            SpawnForm(ref aboutForm);
        }

        public void button_ColorPicker_Click(IRibbonControl control)
        {
            SpawnForm(ref colorPickerForm);
        }
    }
}
