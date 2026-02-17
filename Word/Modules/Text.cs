using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Document = Microsoft.Office.Interop.Word.Document;
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;

namespace Word.Modules
{
    internal static class Text
    {
        /// <summary>
        /// Converts the document text by applying various formatting and cleanup operations.
        /// </summary>
        internal static void Prepare(
            bool doRemoveColumnBreaks,
            bool doRemoveSectionBreaks,
            bool doSetSingleColumn,

            bool doTrimSpaces,
            bool doFixFontProperties,
            bool doSetLinespacingSingle,

            bool doRemoveSpacingBeforeAfter,
            bool doRemoveLeftIndent,
            bool doRemoveRightIndent,
            bool doRemoveSpecianIndent,

            bool doRemoveTabstops,
            bool doRemoveTabs,
            bool doLockImageAspectRatio,
            
            bool doAutofitTextboxes,
            bool doRemoveOptionalHyphens,
            bool doDisableAutomaticHyphenation,
            
            bool doRemoveSoftbreaks
        )
        {
            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            Shared.EnsurePrintViewAndCloseSplit(app);

            var undo = app.UndoRecord;

            try
            {
                app.ScreenUpdating = false;

                if (doRemoveSectionBreaks && doc.Sections.Count > 1) RemoveSectionBreaks(doc);

                // avoid find/replace crash
                if (doRemoveColumnBreaks) RemoveColumnBreaks(doc);

                if (!undo.IsRecordingCustomRecord)
                    undo.StartCustomRecord("Convert");
                
                if (doSetSingleColumn) SetSingleColumn(doc);

                if (doDisableAutomaticHyphenation) DisableAutomaticHyphenation();

                Shared.ApplyGlobally(doc, range =>
                {
                    if (doFixFontProperties) FixFontProperties(range);
                    if (doSetLinespacingSingle) SetLinespacingSingle(range);

                    if (doRemoveLeftIndent) RemoveLeftIndent(range);
                    if (doRemoveRightIndent) RemoveRightIndent(range);
                    if (doRemoveSpecianIndent) RemoveSpecialIndent(range);
                    if (doRemoveSpacingBeforeAfter) RemoveBeforeAfterSpacing(range);

                    if (doRemoveTabstops) RemoveTabstops(range);
                    if (doLockImageAspectRatio) Images.LockImageAspectRatio(range);

                    if (doAutofitTextboxes) AutofitTextboxes(range);
                    if (doRemoveOptionalHyphens) RemoveOptionalHyphens(range);

                    if (doRemoveSoftbreaks) RemoveSoftbreaks(range);
                });
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

                // avoid find/replace crash
                Shared.ApplyGlobally(doc, range =>
                {
                    if (doTrimSpaces) TrimSpaces(range);
                    if (doRemoveTabs) RemoveTabs(range);
                });

                GC.WaitForPendingFinalizers();
                GC.Collect();

                app.ScreenUpdating = true;
            }
        }

        /// <summary>
        /// Removes all soft line breaks (manual line breaks) from the specified range and replaces them with spaces.
        /// </summary>
        private static void RemoveSoftbreaks(Range range)
        {
            var find = range.Find;
            find.ClearFormatting();
            find.Replacement.ClearFormatting();
            find.Text = "^l"; // Soft line break
            find.Replacement.Text = "^p"; // Replace with paragraph break
            find.Forward = true;
            find.Wrap = WdFindWrap.wdFindContinue;
            find.Format = false;
            find.MatchCase = false;
            find.MatchWholeWord = false;
            find.MatchWildcards = false;
            find.Execute(Replace: WdReplace.wdReplaceAll);
        }

        /// <summary>
        /// Removes all tab characters from the specified range.
        /// </summary>
        private static void RemoveTabs(Range range)
        {
            // Remove all tab characters from the range
            range.Find.ClearFormatting();
            range.Find.Replacement.ClearFormatting();
            range.Find.Text = "\t"; // Tab character
            range.Find.Replacement.Text = " "; // Replace with nothing
            range.Find.Forward = true;
            range.Find.Wrap = WdFindWrap.wdFindContinue;
            range.Find.Format = false;
            range.Find.MatchCase = false;
            range.Find.MatchWholeWord = false;
            range.Find.MatchWildcards = false;
            // Execute the find and replace operation
            range.Find.Execute(Replace: WdReplace.wdReplaceAll);
        }

        /// <summary>
        /// Autofits all textboxes in the given range by setting their AutoSize property to true if they contain text.
        /// Handles both inline and floating textboxes.
        /// </summary>
        internal static void AutofitTextboxes(Range range)
        {
            var textBoxTypes = new HashSet<MsoShapeType> { MsoShapeType.msoTextBox };

            Shared.TraverseShapes(range, textBoxTypes, shape =>
            {
                if (shape.TextFrame.HasText != 0)
                {
                    shape.TextFrame.AutoSize = (int)MsoTriState.msoTrue;
                }
            });
        }

        /// <summary>
        /// Removes optional hyphens from the specified range in the document.
        /// </summary>
        private static void RemoveOptionalHyphens(Range range)
        {
            // pray this this would not crash because find.Execute is fucking garbage
            var find = range.Find;
            find.ClearFormatting();
            find.Text = "^-"; // Word code for optional hyphen
            find.Replacement.ClearFormatting();
            find.Replacement.Text = "";
            find.Forward = true;
            find.Wrap = WdFindWrap.wdFindContinue;
            find.Format = false;
            find.MatchWildcards = false;

            find.Execute(Replace: WdReplace.wdReplaceAll);
        }

        /// <summary>
        /// Disable automatic hyphenation in active document.
        /// </summary>
        private static void DisableAutomaticHyphenation()
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            var doc = Globals.ThisAddIn.Application.ActiveDocument;
            doc.AutoHyphenation = false;
        }

        /// <summary>
        /// Removes spacing before and after paragraphs in the specified range.
        /// </summary>
        private static void RemoveBeforeAfterSpacing(Range range)
        {
            foreach (Paragraph para in range.Paragraphs)
            {
                para.SpaceBefore = 0;
                para.SpaceAfter = 0;
            }
        }

        /// <summary>
        /// Removes all tab stops from the paragraphs in the specified range.
        /// </summary>
        private static void RemoveTabstops(Range range)
        {
            foreach (Paragraph para in range.Paragraphs)
            {
                para.TabStops.ClearAll();
            }
        }

        /// <summary>
        /// Removes all column breaks from the document.
        /// </summary>
        private static void RemoveColumnBreaks(Document doc)
        {
            var find = doc.Content.Find;
            find.ClearFormatting();
            find.Replacement.ClearFormatting();

            find.Text = "^n";
            find.Replacement.Text = "";

            find.Forward = true;
            find.Wrap = WdFindWrap.wdFindContinue;
            find.Format = false;
            find.MatchCase = false;
            find.MatchWholeWord = false;
            find.MatchWildcards = false;

            find.Execute(Replace: WdReplace.wdReplaceAll);
        }

        /// <summary>
        /// Removes left indent from the specified range.
        /// </summary>
        private static void RemoveLeftIndent(Range range)
        {
            range.ParagraphFormat.LeftIndent = 0f;
            range.ParagraphFormat.CharacterUnitLeftIndent = 0;
        }

        /// <summary>
        /// Removes right indent from the specified range.
        /// </summary>
        private static void RemoveRightIndent(Range range)
        {
            range.ParagraphFormat.RightIndent = 0f;
            range.ParagraphFormat.CharacterUnitRightIndent = 0;
        }

        /// <summary>
        /// Remove special indent (first line and hanging) from the specified range.
        /// </summary>
        private static void RemoveSpecialIndent(Range range)
        {
            range.ParagraphFormat.FirstLineIndent = 0f;
            range.ParagraphFormat.CharacterUnitFirstLineIndent = 0;
            range.ParagraphFormat.CharacterUnitLeftIndent = 0;
        }

        /// <summary>
        /// Replace continuous sections breaks with paragraph breaks and new page section breaks with page breaks.
        /// </summary>
        private static void RemoveSectionBreaks(Document doc)
        {
            var range = doc.Content;
            var find = range.Find;

            find.ClearFormatting();
            find.Replacement.ClearFormatting();
            find.Text = "^b";
            find.Forward = true;
            find.Wrap = WdFindWrap.wdFindStop;

            while (find.Execute())
            {
                var start = range.Start;
                var end = range.End;

                // Determine the section at the current position
                var currentSection = doc.Range(0, start).Sections.Last;

                // Delete the section break
                var sectionBreakRange = doc.Range(start, end);
                sectionBreakRange.Delete();

                // Insert appropriate break
                var insertRange = doc.Range(start, start);
                insertRange.InsertBreak(currentSection.PageSetup.SectionStart == WdSectionStart.wdSectionContinuous
                    ? WdBreakType.wdLineBreak
                    : WdBreakType.wdPageBreak); // use line break as approximation for paragraph

                // Reset range and Find object for next loop
                range = doc.Range(start + 1, doc.Content.End);
                find = range.Find;
                find.ClearFormatting();
                find.Replacement.ClearFormatting();
                find.Text = "^b";
                find.Forward = true;
                find.Wrap = WdFindWrap.wdFindStop;
            }
        }

        /// <summary>
        ///  Set single column layout for all sections in the document.
        /// </summary>
        private static void SetSingleColumn(Document doc)
        {
            foreach (Section section in doc.Sections)
            {
                section.PageSetup.TextColumns.SetCount(1);
            }
        }

        /// <summary>
        /// Trims redundant spaces in the specified range.
        /// </summary>
        internal static void TrimSpaces(Range range)
        {
            var pairs = new (string findText, string replaceText, bool wildcards)[]
            {
                (" .", ".", false),
                (" ,", ",", false),
                ("( ", "(", false),
                (" )", ")", false),
                ("  ", " ", false),
                (" ;", ";", false),
            };

            foreach (var (findText, replaceText, wildcards) in pairs)
            {
                while(true) {
                    var find = range.Find;
                    find.ClearFormatting();
                    find.Replacement.ClearFormatting();

                    find.Text = findText;
                    find.Replacement.Text = replaceText;

                    find.Forward = true;
                    find.Wrap = WdFindWrap.wdFindContinue;
                    find.Format = false;
                    find.MatchCase = false;
                    find.MatchWholeWord = false;
                    find.MatchWildcards = wildcards;

                    if (!find.Execute(Replace: WdReplace.wdReplaceAll))
                        break;
                }
            }
        }

        /// <summary>
        /// Fixes font scaling, spacing, kerning, and position for the specified range.
        /// </summary>
        private static void FixFontProperties(Range range)
        {
            range.Font.Scaling = 100;
            range.Font.Spacing = 0;
            range.Font.Kerning = 0;
            range.Font.Position = 0;

            // Apply to lists as well (they are not covered by range.Font because Word is broken)
            var template = range.ListFormat.ListTemplate;

            if (template == null)
                return;

            for (var level = 1; level <= template.ListLevels.Count; level++)
            {
                var font = template.ListLevels[level].Font;

                font.Scaling = 100;
                font.Spacing = 0;
                font.Kerning = 0;
                font.Position = 0;
            }
        }

        /// <summary>
        /// Sets the line spacing of the specified range to single spacing.
        /// </summary>
        private static void SetLinespacingSingle(Range range)
        {
            range.Paragraphs.LineSpacingRule = WdLineSpacing.wdLineSpaceSingle;
        }

        internal static void GlobalUnhide()
        {
            var app = Globals.ThisAddIn.Application;
            var doc = Globals.ThisAddIn.Application.ActiveDocument;

            Shared.RunWithUndo("Global Unhide", "Error", () =>
            {
                if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;



                Shared.ApplyGlobally(doc, range => {
                    range.Font.Hidden = 0;
                });

                app.Activate();
            });
        }

        internal static void GlobalHide()
        {
            var app = Globals.ThisAddIn.Application;
            var doc = Globals.ThisAddIn.Application.ActiveDocument;

            Shared.RunWithUndo("Global Unhide", "Error", () =>
            {
                if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;



                Shared.ApplyGlobally(doc, range => {
                    range.Font.Hidden = 1;
                });

                app.Activate();
            });
        }

        // TODO: Review and test this
        internal static void AutoDtp()
        {
            var doc = Globals.ThisAddIn.Application.ActiveDocument;

            var letterNumLiteral = "[A-Za-zА-Яа-яÀ-ÿĀ-ſƀ-ɏΑ-ώЀ-ӿİŞĞÜÇÖışğüçö0-9]";
            var letterLiteral = "[A-Za-zА-Яа-яÀ-ÿĀ-ſƀ-ɏΑ-ώЀ-ӿİŞĞÜÇÖışğüçö]";

            // REMEMBER: This bullshit crashes when run inside an undo custom record, so we have to run it outside of that context.
            // i fucking hate word interop
            //  alawys use find/replace crap outside of undo trash

            Shared.ApplyGlobally(doc, range =>
            {
                // 1. Fix " a " -> " a^s"
                var find = range.Find;
                find.ClearFormatting();
                find.Replacement.ClearFormatting();
                find.Text = $" ({letterNumLiteral}) ";
                find.Replacement.Text = " \\1^s";
                find.Forward = true;
                find.Wrap = WdFindWrap.wdFindContinue;
                find.Format = false;
                find.MatchWildcards = true;
                find.Execute(Replace: WdReplace.wdReplaceAll);

                // 2. Fix ". ab " -> ". ab^s"
                find = range.Find;
                find.ClearFormatting();
                find.Replacement.ClearFormatting();
                find.Text = $". ({letterLiteral}{letterLiteral}) ";
                find.Replacement.Text = ". \\1^s";
                find.Forward = true;
                find.Wrap = WdFindWrap.wdFindContinue;
                find.Format = false;
                find.MatchWildcards = true;
                find.Execute(Replace: WdReplace.wdReplaceAll);

                // 3. Fix ", ab " -> ", ab^s"
                find = range.Find;
                find.ClearFormatting();
                find.Replacement.ClearFormatting();
                find.Text = $", ({letterLiteral}{letterLiteral}) ";
                find.Replacement.Text = ", \\1^s";
                find.Forward = true;
                find.Wrap = WdFindWrap.wdFindContinue;
                find.Format = false;
                find.MatchWildcards = true;
                find.Execute(Replace: WdReplace.wdReplaceAll);
            });
        }
    }
}
