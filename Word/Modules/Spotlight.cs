using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;

namespace Word.Modules
{
    /// <summary>
    /// Provides methods for condition-based text highlighting in a Word document.
    /// </summary>
    internal static class Spotlight
    {
        /// <summary>
        /// Runs specifie highlight preset on the active document.
        /// </summary>
        internal static void Run(
            bool doFastDtp = false,
            bool doJustifiedText = false,
            bool doUndelimitedText = false,

            bool doDecimalDot = false,
            bool doDecimalComma = false,

            WdColorIndex color = WdColorIndex.wdYellow
        )
        {
            // TODO: duplication, refactor with a wrapper later
            if (!Shared.HasActiveDocument()) return;

            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            app.ScreenUpdating = false;

            var undo = app.UndoRecord;

            if (!undo.IsRecordingCustomRecord)
                undo.StartCustomRecord("Highlight");
            try
            {
                Shared.ApplyGlobally(doc, range =>
                {
                    if (doFastDtp)
                    {
                        // character-based parallel processing is expensive
                        // but during the testing i managed to shorten time spent from 20s into 15s (even 13s later on)
                        // 83 pages document with a 483 ranges???
                        // jesus christ

                        HangingLetters(range, color);
                        Hyphens(range, color);
                        Digits(range, color);
                    }

                    // some parallel stuff
                    if (doJustifiedText) JustifiedText(range, color);
                    if (doUndelimitedText) UndelimitedText(range, color);
                });

            }
            catch (Exception ex)
            {
               MessageBox.Show(
                    ex.Message,
                    "Highlight Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (undo.IsRecordingCustomRecord)
                    undo.EndCustomRecord();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                app.ScreenUpdating = true;
            }
        }

        /// <summary>
        /// Clear all highlights in the active document. Warning: this will remove all highlights, not just those added by this add-in.
        /// </summary>
        internal static void Clear()
        {
            if (!Shared.HasActiveDocument()) return;

            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            app.ScreenUpdating = false;
            var undo = app.UndoRecord;

            if (!undo.IsRecordingCustomRecord)
                undo.StartCustomRecord("Clear Highlight");

            try
            {
                Shared.ApplyGlobally(doc, range =>
                {
                    range.HighlightColorIndex = WdColorIndex.wdNoHighlight;
                });
            }
            catch (Exception ex) {
                MessageBox.Show(
                     ex.Message,
                     "Error",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Error
                 );
            }
            finally             {
                if (undo.IsRecordingCustomRecord)
                    undo.EndCustomRecord();

                GC.WaitForPendingFinalizers();
                GC.Collect();
                app.ScreenUpdating = true;
            }
        }

        /// <summary>
        /// Highlights hyphens in the specified range.
        /// </summary>
        private static void Hyphens(Range range, WdColorIndex color)
        {
            HighlightFound(range, "[‒–—]", color);
        }

        /// <summary>
        /// Highlights digits in the specified range.
        /// </summary>
        private static void Digits(Range range, WdColorIndex color)
        {
            HighlightFound(range, "[0-9]", color);
        }

        /// <summary>
        /// Highlights every wildcard match of the pattern inside the range.
        /// </summary>
        private static void HighlightFound(Range range, string pattern, WdColorIndex color)
        {
            // Search on a copy: Find moves the range it runs on.
            var search = range.Duplicate;
            var end = range.End;
            var find = search.Find;
            find.ClearFormatting();
            find.Text = pattern;
            find.MatchWildcards = true;
            find.Forward = true;
            find.Wrap = WdFindWrap.wdFindStop;

            while (find.Execute() && search.Start < end)
            {
                search.HighlightColorIndex = color;
                search.Collapse(WdCollapseDirection.wdCollapseEnd);
            }
        }

        /// <summary>
        /// Highlights paragraphs that do not end with a delimiter (., !, ?, ;, or ,).
        /// </summary>
        private static void UndelimitedText(Range range, WdColorIndex color)
        {
            foreach (Range sentence in range.Sentences)
            {
                var text = sentence.Text.TrimEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    var lastChar = text[text.Length - 1];
                    if (!".!?…;,".Contains(lastChar))
                        sentence.HighlightColorIndex = color;
                }
            }
        }

        /// <summary>
        /// Highlights justified paragraphs in the specified range.
        /// </summary>
        private static void JustifiedText(Range range, WdColorIndex color)
        {
            foreach (Paragraph paragraph in range.Paragraphs)
            {
                if (paragraph.Alignment == WdParagraphAlignment.wdAlignParagraphJustify)
                    paragraph.Range.HighlightColorIndex = color;
            }
        }

        /// <summary>
        /// Hightlights all single letters and digits that are hanging between 2 spaces in the specified range.
        /// </summary>
        private static void HangingLetters(Range range, WdColorIndex color)
        {
            // Search on a copy: Find moves the range it runs on, and the caller passes
            // the same range on to Hyphens() and Digits() afterwards.
            foreach (var pattern in new[] { " [! ] ", " [! ]^13" })
            {
            var search = range.Duplicate;
            var find = search.Find;
            find.ClearFormatting();
            // this works well enough to avoid very heavy forloops inside find/replace caused by huge wildcard range operators
            find.Text = pattern;
            find.MatchWildcards = true;

            while (find.Execute())
            {
                var highlightStart = search.Start + 1;
                var highlightEnd = highlightStart + 1;

                // Duplicate keeps the story (header, footnote, textbox); doc.Range() would point into the main text.
                var charRange = search.Duplicate;
                charRange.SetRange(highlightStart, highlightEnd);
                var c = charRange.Text[0];
                // this is fine solution to avoid long wildcard ranges
                if (char.IsLetterOrDigit(c))
                {
                    charRange.HighlightColorIndex = color;
                }

                // Continue from the trailing space, not after it: in "і в" that space
                // is also the leading space of the next single letter.
                search.SetRange(highlightEnd, highlightEnd);
            }
            }
        }
    }
}
