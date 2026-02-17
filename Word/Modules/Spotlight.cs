using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            if (doc == null) return;

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
            var app = Globals.ThisAddIn.Application;
            var doc = app.ActiveDocument;

            if (doc == null) return;

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
            var chars = range.Characters.Cast<Range>().ToArray();
            var indices = new List<int>();

            Parallel.For(0, chars.Length, i =>
            {
                if ("‒–—".Contains(chars[i].Text))
                {
                    lock (indices)
                        indices.Add(i);
                }
            });

            foreach (var i in indices)
            {
                chars[i].HighlightColorIndex = color;
            }
        }

        /// <summary>
        /// Highlights digits in the specified range.
        /// </summary>
        private static void Digits(Range range, WdColorIndex color)
        {
            var chars = range.Characters.Cast<Range>().ToArray();
            var indices = new List<int>();

            Parallel.ForEach(chars.Select((c, i) => (c, i)), item =>
            {
                var c = item.c.Text[0];
                if (char.IsDigit(c))
                {
                    lock (indices)
                        indices.Add(item.i);
                }
            });

            foreach (var i in indices)
            {
                chars[i].HighlightColorIndex = color;
            }
        }

        /// <summary>
        /// Highlights paragraphs that do not end with a delimiter (., !, ?, ;, or ,).
        /// </summary>
        private static void UndelimitedText(Range range, WdColorIndex color)
        {
            var matches = new ConcurrentBag<Range>();
            var sentences = range.Sentences.Cast<Range>().ToList();

            Parallel.ForEach(sentences, sentence =>
            {
                var text = sentence.Text.TrimEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    var lastChar = text.Length > 0 ? text[text.Length - 1] : '\0'; // Replace index operator with Length-based access  
                    if (!".!?…;,".Contains(lastChar))
                    {
                        matches.Add(sentence.Duplicate);
                    }
                }
            });

            foreach (var match in matches)
            {
                match.HighlightColorIndex = color;
            }
        }

        /// <summary>
        /// Highlights justified paragraphs in the specified range.
        /// </summary>
        private static void JustifiedText(Range range, WdColorIndex color)
        {
            var paragraphs = range.Paragraphs.Cast<Paragraph>().ToArray();

            var justifiedIndices = new List<int>();

            Parallel.ForEach(paragraphs.Select((p, i) => (p, i)), item =>
            {
                if (item.p.Alignment == WdParagraphAlignment.wdAlignParagraphJustify)
                {
                    lock (justifiedIndices)
                        justifiedIndices.Add(item.i);
                }
            });

            foreach (var i in justifiedIndices)
            {
                paragraphs[i].Range.HighlightColorIndex = color;
            }
        }

        /// <summary>
        /// Hightlights all single letters and digits that are hanging between 2 spaces in the specified range.
        /// </summary>
        private static void HangingLetters(Range range, WdColorIndex color)
        {
            var find = range.Find;
            find.ClearFormatting();
            // this works well enough to avoid very heavy forloops inside find/replace caused by huge wildcard range operators
            find.Text = " [! ] ";
            find.MatchWildcards = true;

            while (find.Execute())
            {
                int highlightStart = find.Parent.Start + 1;
                var highlightEnd = highlightStart + 1;

                var doc = range.Document;
                var charRange = doc.Range(highlightStart, highlightEnd);
                var c = charRange.Text[0];
                // this is fine solution to avoid long wildcard ranges
                if (char.IsLetterOrDigit(c))
                {
                    charRange.HighlightColorIndex = color;
                }

                find.Parent.Collapse(WdCollapseDirection.wdCollapseEnd);
            }
        } 
    }
}
