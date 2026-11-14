using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Word.Modules;

using Word.Helpers;
namespace Word.Forms
{
    public partial class SubtitleToolsForm : Form
    {
        public SubtitleToolsForm()
        {
            InitializeComponent();
        }

        private void button_RemoveLinebreaks_Click(object sender, EventArgs e)
        {
            button_HideTS_Click(sender, e);
            button_ShowTS_Click(sender, e);
            button_HideTS_Click(sender, e);

            Shared.RunWithUndo("Remove sentence linebreaks", "Error removing sentence linebreaks", () =>
            {
                var app = Globals.ThisAddIn.Application;
                var doc = app.ActiveDocument;
                var find = doc.Content.Find;
                
                Subtitles.EncodeParagraphBreaks(doc);

                find.ClearFormatting();
                find.Replacement.ClearFormatting();

                find.Text = "([!.?])⸻";
                find.Replacement.Text = "\\1"; // keep the punctuation, remove ⸻

                find.Forward = true;
                find.Wrap = WdFindWrap.wdFindContinue;
                find.Format = false;
                find.MatchWildcards = true; // enable regex-like matching

                find.Execute(Replace: WdReplace.wdReplaceAll);

                // normalize encoded paragraph breaks

                find.ClearFormatting();
                find.Replacement.ClearFormatting();

                find.Text = "⸻⸻";
                find.Replacement.Text = "⸻";
                find.Forward = true;
                find.Wrap = WdFindWrap.wdFindContinue;
                find.Format = false;
                find.MatchWildcards = false;
                find.Execute(Replace: WdReplace.wdReplaceAll);

                find.Text = "⸻";
                find.Replacement.Text = "⸻⸻";
                find.Execute(Replace: WdReplace.wdReplaceAll);

                Subtitles.DecodeParagraphBreaks(doc);
            });
        }

        // Word wildcards: "???" stands for "-->" because ">" is a wildcard token. Cue number is optional (VTT).
        private static string[] CuePatterns(string br)
        {
            const string time = "[0-9:.,]{9,12}";
            var cue = time + " ??? " + time + br;
            return new[] { "[0-9]{1,}" + br + cue, cue };
        }

        private void button_HideTS_Click(object sender, EventArgs e)
        {
            Modules.Text.GlobalUnhide();

            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            app.ScreenUpdating = false;
            Enabled = false;


            Shared.RunWithUndo("Hide subtitle timestamps", "Error hiding subtitle timestamps", () =>
            {
                Subtitles.EncodeParagraphBreaks(doc); // encode paragraph breaks before hiding timestamps

                var findPatterns = CuePatterns("⸻");

                foreach (var pattern in findPatterns)
                {
                    var rng = doc.Content.Duplicate;
                    var find = rng.Find;
                    find.ClearFormatting();
                    find.Text = pattern;
                    find.MatchWildcards = true;
                    find.Wrap = WdFindWrap.wdFindStop;
                    find.Forward = true;
                    find.Format = false;
                    find.Replacement.ClearFormatting();

                    while (find.Execute())
                    {
                        rng.Font.Hidden = 1;
                        rng.Font.Color = WdColor.wdColorDarkBlue;
                        rng.Collapse(WdCollapseDirection.wdCollapseEnd);
                    }
                }

                Subtitles.DecodeParagraphBreaks(doc); // decode paragraph breaks after hiding timestamps
                // Subtitles.removeExcessiveParagraphs(app);
            });


            app.ScreenUpdating = true;
            Enabled = true;
        }

        private void button_ShowTS_Click(object sender, EventArgs e)
        {
            Shared.RunWithUndo("Show subtitle timestamps", "Error showing subtitle timestamps", () => {
                // Reset only what Hide marked (hidden + dark blue), so the user's own colors stay.
                var find = Globals.ThisAddIn.Application.ActiveDocument.Content.Find;
                find.ClearFormatting();
                find.Replacement.ClearFormatting();
                find.Text = "";
                find.Replacement.Text = "";
                find.Font.Color = WdColor.wdColorDarkBlue;
                find.Font.Hidden = 1;
                find.Replacement.Font.Color = WdColor.wdColorAutomatic;
                find.Replacement.Font.Hidden = 0;
                find.Format = true;
                find.MatchWildcards = false;
                find.Wrap = WdFindWrap.wdFindContinue;
                find.Execute(Replace: WdReplace.wdReplaceAll);
            });
        }

        private void button_RemoveTS_Click(object sender, EventArgs e)
        {
            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            button_ShowTS_Click(sender, e); // first show all timestamps

            Shared.RunWithUndo("Remove subtitle timestamps", "Error removing subtitle timestamps", () => {
                var findPatterns = CuePatterns("^13");

                foreach (var pattern in findPatterns)
                {
                    var find = doc.Content.Find;
                    find.ClearFormatting();
                    find.Text = pattern;
                    find.MatchWildcards = true;
                    find.Wrap = WdFindWrap.wdFindContinue;
                    find.Format = false;

                    find.Replacement.ClearFormatting();
                    find.Replacement.Text = "";

                    find.Execute(
                        FindText: pattern,
                        MatchWildcards: true,
                        Wrap: WdFindWrap.wdFindContinue,
                        ReplaceWith: "",
                        Replace: WdReplace.wdReplaceAll
                    );
                }
            });

            Subtitles.RemoveExcessiveParagraphs(app.ActiveDocument);
        }

        private void button_ApplyStyle_Click(object sender, EventArgs e)
        {
            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            Shared.RunWithUndo("Apply subtitle style", "Error applying style", () =>
            {
                Shared.ApplyGlobally(doc, range =>
                {
                    range.Font.Name = "Arial";
                    range.Font.Size = 10;

                    range.ParagraphFormat.SpaceBefore = 0;
                    range.ParagraphFormat.SpaceAfter = 0;
                    range.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
                });
            });
        }

        private void ConvertAndSave(string targetExtension)
        {
            var inputPath = "";
            var closed = false;

            try
            {
                if (!Shared.HasActiveDocument())
                {
                    UiPrompt.Show("No active document found.", $"Save {targetExtension.ToUpper()}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var doc = Globals.ThisAddIn.Application.ActiveDocument;
                var folder = doc.Path;
                if (string.IsNullOrEmpty(folder))
                {
                    UiPrompt.Show("Please save the document first before running this action.", $"Save {targetExtension.ToUpper()}", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Modules.Text.GlobalUnhide();

                var fileName = Path.GetFileNameWithoutExtension(doc.Name);
                inputPath = doc.FullName;
                var outputPath = Path.Combine(folder, $"{fileName}.{targetExtension}");

                // Convert before closing: if the text is not valid subtitles this throws,
                // nothing is written, and the document stays open.
                var content = doc.Content.Text;
                var converted = Subtitles.ConvertFormat(content, targetExtension, inputPath);

                doc.Close(false);
                closed = true;

                File.WriteAllText(outputPath, converted, new UTF8Encoding(false));

                Globals.ThisAddIn.Application.Documents.Open(outputPath);

                Clipboard.SetText(outputPath);
                UiPrompt.Show($"{targetExtension.ToUpper()} saved to:\n{outputPath}",
                                $"Save {targetExtension.ToUpper()}\n\nPath is copied to clipboard!",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                UiPrompt.Show($"Error saving {targetExtension.ToUpper()}:\n{ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Reopen the original source document if we closed it
                if (closed && File.Exists(inputPath))
                    Globals.ThisAddIn.Application.Documents.Open(inputPath);
            }

            Close();
        }

        private void button_SaveSRT_Click(object sender, EventArgs e)
        {            
            ConvertAndSave("srt");
        }

        private void button_SaveVTT_Click(object sender, EventArgs e)
        {            
            ConvertAndSave("vtt");
        }

        private void button_SaveDocx_Click(object sender, EventArgs e)
        {
            try
            {
                var doc = Globals.ThisAddIn.Application.ActiveDocument;
                if (doc == null)
                {
                    UiPrompt.Show("No open document found.", "Save DOCX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var folder = doc.Path;
                if (string.IsNullOrEmpty(folder))
                {
                    UiPrompt.Show("Please save the document manually first before running this action.", "Document Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var fileName = Path.GetFileNameWithoutExtension(doc.Name);
                if (string.IsNullOrEmpty(fileName))
                {
                    UiPrompt.Show("Could not determine the file name.\nSave the document and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var newPath = Path.Combine(folder, fileName + ".docx");

                doc.SaveAs2(
                    FileName: newPath,
                    FileFormat: WdSaveFormat.wdFormatXMLDocument,
                    LockComments: false,
                    Password: "",
                    AddToRecentFiles: true,
                    WritePassword: "",
                    ReadOnlyRecommended: false,
                    EmbedTrueTypeFonts: false,
                    SaveNativePictureFormat: false,
                    SaveFormsData: false,
                    SaveAsAOCELetter: false,
                    CompatibilityMode: 15
                );

                UiPrompt.Show($"DOCX saved to:\n{newPath}", "Save DOCX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clipboard.SetText(newPath);

            }
            catch (Exception ex)
            {
                UiPrompt.Show("Error saving  DOCX:\n" + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Close();
        }
    }
}
