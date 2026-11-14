using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;

using Word.Helpers;
namespace Word.Modules
{
    /// <summary>
    /// Font management utilities for Microsoft Word documents.
    /// </summary>
    internal static partial class Fonts
    {
        /// <summary>
        /// Windows default fonts that are typically present on all installations.
        /// </summary>
        internal static readonly string[] WindowsDefaultFonts = new[]
        {
            "Arial",
            "Arial Black",
            "Bahnschrift",
            "Calibri",
            "Cambria",
            "Cambria Math",
            "Candara",
            "Comic Sans MS",
            "Consolas",
            "Constantia",
            "Corbel",
            "Courier New",
            "Ebrima",
            "Franklin Gothic Medium",
            "Gabriola",
            "Georgia",
            "Impact",
            "Ink Free",
            "Javanese Text",
            "Leelawadee UI",
            "Lucida Console",
            "Lucida Sans Unicode",
            "Malgun Gothic",
            "Marlett",
            "Microsoft Himalaya",
            "Microsoft JhengHei",
            "Microsoft New Tai Lue",
            "Microsoft PhagsPa",
            "Microsoft Sans Serif",
            "Microsoft Tai Le",
            "Microsoft YaHei",
            "Microsoft Yi Baiti",
            "MingLiU-ExtB",
            "Mongolian Baiti",
            "MS Gothic",
            "MS PGothic",
            "MS UI Gothic",
            "MV Boli",
            "Nirmala UI",
            "Palatino Linotype",
            "Segoe MDL2 Assets",
            "Segoe Print",
            "Segoe Script",
            "Segoe UI",
            "Segoe UI Historic",
            "Segoe UI Emoji",
            "Segoe UI Symbol",
            "SimSun",
            "Sitka",
            "Sylfaen",
            "Symbol",
            "Tahoma",
            "Times New Roman",
            "Trebuchet MS",
            "Verdana",
            "Webdings",
            "Wingdings",
            "Wingdings 2",
            "Wingdings 3",
            "Yu Gothic"
        };

        /// <summary>
        /// Applies the specified font globally to the content of the active document, using a custom action for each
        /// range.
        /// </summary>
        /// <param name="title">The title of the operation, used to identify the action in the undo history.</param>
        /// <param name="fontName">The name of the font to apply to the document's content.</param>
        /// <param name="action">An action delegate that receives a Range object, allowing custom operations to be performed on each range of
        /// the document.</param>
        private static void ApplyGlobalFont(string title, string fontName, Action<Range> action)
        {
            var app = Globals.ThisAddIn.Application;

            if (app.Documents.Count == 0) return;

            var doc = app.ActiveDocument;
            var undo = app.UndoRecord;
            var startedUndo = false;

            if (!undo.IsRecordingCustomRecord)
            {
                undo.StartCustomRecord($"{title} → {fontName}");
                startedUndo = true;
            }

            try
            {
                Shared.ApplyGlobally(doc, action);
            }
            finally
            {
                if (startedUndo && undo.IsRecordingCustomRecord)
                    undo.EndCustomRecord();
            }
        }

        internal static void SetGlobalCyrillicOtherFont(string font)
        {
            ApplyGlobalFont("Set ComplexScripts Font", font, range =>
            {
                range.Font.NameOther = font;
            });
        }

        internal static void SetGlobalRtlThaiFont(string font)
        {
            ApplyGlobalFont("Set RTL/Thai Font", font, range =>
            {
                range.Font.NameBi = font;
            });
        }

        internal static void SetGlobalAsianFont(string font)
        {
            ApplyGlobalFont("Set Asian Font", font, range =>
            {
                range.Font.NameFarEast = font;
            });
        }

        internal static void SetGlobalLatinFont(string font)
        {
            ApplyGlobalFont("Set Latin Font", font, range =>
            {
                range.Font.Name = font;
            });
        }

        internal static void ReplaceDocumentFont(string sourceFont, string targetFont)
        {
            if (!Shared.HasActiveDocument()) return;

            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            var source = sourceFont.TrimStart('@');
            var target = targetFont.TrimStart('@');

            if (source == "" || target == "") return;

            var undo = app.UndoRecord;
            if (!undo.IsRecordingCustomRecord)
                undo.StartCustomRecord($"Replace Font: {source} → {target}");

            try
            {
                Shared.ApplyGlobally(doc, range =>
                {
                    // Replace Western font
                    var find = range.Find;
                    find.ClearFormatting();
                    find.Font.Name = source;
                    find.Replacement.ClearFormatting();
                    find.Replacement.Font.Name = target;
                    find.Execute(Replace: WdReplace.wdReplaceAll);

                    // Replace Far East font (e.g., Japanese, Chinese)
                    try
                    {
                        find = range.Find;
                        find.ClearFormatting();
                        find.Font.NameFarEast = source;
                        find.Replacement.ClearFormatting();
                        find.Replacement.Font.NameFarEast = target;
                        find.Execute(Replace: WdReplace.wdReplaceAll);
                    }
                    catch (System.Runtime.InteropServices.COMException)
                    {
                        // East Asian editing is off in Word: skip the Far East pass
                    }
                });
            }
            finally
            {
                if (undo.IsRecordingCustomRecord)
                    undo.EndCustomRecord();
            }
        }

        internal static List<string> GetDocumentRealFonts()
        {
            var fonts = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

            if (!Shared.HasActiveDocument()) return new List<string>();
            var doc = Globals.ThisAddIn.Application.ActiveDocument;

            Shared.ApplyGlobally(doc, range =>
            {
                foreach (Paragraph para in range.Paragraphs)
                    CollectFontNames(para.Range, fonts, 0);
            });

            return fonts.OrderBy(f => f, StringComparer.CurrentCultureIgnoreCase).ToList();
        }

        // Reads the fonts of a range; if it is mixed (empty name), descends paragraph -> words -> characters.
        private static void CollectFontNames(Range r, HashSet<string> fonts, int level)
        {
            var latin = r.Font.Name?.TrimStart('@');
            var asian = r.Font.NameFarEast?.TrimStart('@');

            var mixed = string.IsNullOrEmpty(latin) || string.IsNullOrEmpty(asian);
            if (!string.IsNullOrEmpty(latin)) fonts.Add(latin);
            if (!string.IsNullOrEmpty(asian)) fonts.Add(asian);
            if (!mixed || level >= 2) return;

            if (level == 0)
            {
                foreach (Range word in r.Words)
                    CollectFontNames(word, fonts, 1);
            }
            else
            {
                foreach (Range ch in r.Characters)
                    CollectFontNames(ch, fonts, 2);
            }
        }

        /// <summary>
        /// Retrieves a list of the names of all installed font families on the system.
        /// </summary>
        /// <returns>A list of strings containing the names of the installed font families, sorted in a case-insensitive manner.</returns>
        internal static List<string> GetInstalledFonts()
        {
            var installedFonts = new InstalledFontCollection();

            return installedFonts.Families
                .Select(f => f.Name)
                .OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }

        /// <summary>
        /// Retrieves a list of unique font names used in the active Word document.
        /// </summary>
        /// <returns>A list of strings containing the names of fonts found in the active document. The list is empty if no fonts
        /// are found or if the document is not accessible.</returns>
        internal static List<string> GetDocumentFonts()
        {
            if (!Shared.HasActiveDocument())
                return new List<string>();
            var doc = Globals.ThisAddIn.Application.ActiveDocument;

            try
            {
                var xdoc = XDocument.Parse(doc.Content.XML);

                return xdoc.Descendants()
                    .Where(n => n.Name.LocalName == "font")
                    .Select(n => n.Attribute(n.Name.Namespace + "name")?.Value
                              ?? n.Attributes().FirstOrDefault(a => a.Name.LocalName == "name")?.Value)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct()
                    .ToList();
            }
            catch
            {
                UiPrompt.Show(
                    "Error loading font table.\nFile might be corrupt.",
                    "Error Reading Font Table",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return new List<string>();
            }
        }

        /// <summary>
        /// Copies the font files used in the active Word document to a _fonts subfolder next to the document.
        /// </summary>
        internal static void CollectFonts(bool collectDefaultFonts)
        {
            if (!Shared.HasActiveDocument()) return;

            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            // Ensure the document is saved: an unsaved one has no folder (FullName is just "Document1"),
            // and the _fonts folder would land in Word's current directory.
            if (string.IsNullOrEmpty(doc.Path))
            {
                UiPrompt.Show(
                    "Please save the document before collecting fonts.",
                    "Document Not Saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Get the list of fonts used in the document
            var usedFonts = GetDocumentFonts();
            if (usedFonts == null || !usedFonts.Any()) return;

            // Prepare the output folder based on the document's path
            var outputFolder = Path.Combine(
                doc.Path,
                Path.GetFileNameWithoutExtension(doc.FullName) + "_fonts"
            );

            // Find font paths from the registry
            var fontFilesToCopy = new List<string>();
            var fontIndex = BuildFontPathIndex();

            foreach (var fontName in usedFonts)
            {
                //also check if the font file is windows default, skip those
                if (!collectDefaultFonts && WindowsDefaultFonts.Contains(fontName))
                {
                    continue;
                }

                fontIndex.TryGetValue(fontName, out var fontFilePath);

                if (!string.IsNullOrEmpty(fontFilePath) && File.Exists(fontFilePath))
                {
                    fontFilesToCopy.Add(fontFilePath);
                }
            }

            if (fontFilesToCopy.Count == 0)
            {
                // Show warning if no fonts were found (no custom font if collectDefaultFonts is false)
                if (collectDefaultFonts) { 
                    UiPrompt.Show(
                        "No fonts found in the document!",
                        "No Fonts Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    UiPrompt.Show(
                        "No custom fonts found in the document.\nOnly Windows default fonts are used.",
                        "No Custom Fonts Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }

                return;
            }

            // Only a folder created by this run may be removed if the first copy fails.
            var createdFolder = !Directory.Exists(outputFolder);
            if (createdFolder)
                Directory.CreateDirectory(outputFolder);

            // Copy font files to the _fonts subfolder
            var firstCopyAttempted = true;

            foreach (var fontPath in fontFilesToCopy)
            {
                try
                {
                    var destPath = Path.Combine(outputFolder, Path.GetFileName(fontPath));

                    File.Copy(fontPath, destPath, true);

                    firstCopyAttempted = false;
                }
                catch (Exception ex)
                {
                    // Show error and clean up if this is the first copy attempt

                    if (createdFolder && firstCopyAttempted && Directory.Exists(outputFolder))
                    {
                        try
                        {
                            Directory.Delete(outputFolder, true);
                        }
                        catch { }
                    }

                    UiPrompt.Show(
                        $"Error copying font '{Path.GetFileName(fontPath)}': {ex.Message}",
                        "Copy Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return;
                }
            }

            // Show success message
            UiPrompt.Show(
                $"{fontFilesToCopy.Count} {(fontFilesToCopy.Count == 1 ? "font" : "fonts")} collected successfully!",
                "Font Collection Report",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            Process.Start(new ProcessStartInfo
            {
                FileName = outputFolder,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
