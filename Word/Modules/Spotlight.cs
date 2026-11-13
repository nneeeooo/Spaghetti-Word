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
            Shared.RunWithUndo("Highlight", "Highlight Error", () =>
            {
                Shared.ApplyGlobally(Globals.ThisAddIn.Application.ActiveDocument, range =>
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
            }, freezeScreen: true);
        }

        /// <summary>
        /// Clear all highlights in the active document. Warning: this will remove all highlights, not just those added by this add-in.
        /// </summary>
        internal static void Clear()
        {
            Shared.RunWithUndo("Clear Highlight", "Error", () =>
            {
                Shared.ApplyGlobally(Globals.ThisAddIn.Application.ActiveDocument, range =>
                {
                    range.HighlightColorIndex = WdColorIndex.wdNoHighlight;
                });
            }, freezeScreen: true);
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
            HighlightReplaceAll(range, color, find =>
            {
                find.Text = pattern;
                find.MatchWildcards = true;
            });
        }

        /// <summary>
        /// One Find/ReplaceAll with Replacement.Highlight instead of a COM call per match.
        /// "^&amp;" keeps the matched text, so only the highlight changes.
        /// </summary>
        private static void HighlightReplaceAll(Range range, WdColorIndex color, Action<Find> configure)
        {
            var options = Globals.ThisAddIn.Application.Options;
            var previous = options.DefaultHighlightColorIndex;
            try
            {
                options.DefaultHighlightColorIndex = color;

                // Search on a copy: Find moves the range it runs on, and the caller reuses the range.
                var search = range.Duplicate;
                var find = search.Find;
                find.ClearFormatting();
                find.Replacement.ClearFormatting();
                configure(find);
                find.Replacement.Text = "^&";
                find.Replacement.Highlight = 1;
                find.Format = true;
                find.Forward = true;
                find.Wrap = WdFindWrap.wdFindStop;
                find.Execute(Replace: WdReplace.wdReplaceAll);
            }
            finally
            {
                options.DefaultHighlightColorIndex = previous;
            }
        }

        /// <summary>
        /// Highlights paragraphs that do not end with a delimiter (., !, ?, ;, or ,).
        /// </summary>
        private static void UndelimitedText(Range range, WdColorIndex color)
        {
            // Not batched: Word's own sentence splitting (table cells, empty paragraphs, quotes after a period)
            // cannot be reproduced from the text, and the time goes into the per-hit highlight calls anyway.
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
        /// Asks Word about the alignment of whole blocks of paragraphs (undefined = mixed) and bisects only the mixed
        /// blocks, instead of reading the alignment of every paragraph. (A document-wide Find/ReplaceAll with a
        /// format replacement inside the undo record crashes Word on large documents, and Find with a paragraph
        /// format skips some paragraphs.)
        /// </summary>
        private static void JustifiedText(Range range, WdColorIndex color)
        {
            if (!TryJustifiedText(range, color))
                JustifiedTextSlow(range, color);
        }

        private static bool TryJustifiedText(Range range, WdColorIndex color)
        {
            if (!TryReadText(range, out var text, out var pos)) return false;

            // paragraph k spans text indexes [starts[k], starts[k + 1])
            var starts = new List<int>();
            var from = 0;
            while (from < text.Length)
            {
                starts.Add(from);
                var nl = text.IndexOf('\r', from);
                var end = nl < 0 ? text.Length : nl + 1;
                if (end < text.Length && text[end] == '\a') end++;
                from = end;
            }
            if (starts.Count == 0) return true;
            starts.Add(text.Length);

            var target = range.Duplicate;
            Action<int, int> visit = null;
            visit = (lo, hi) => // paragraphs [lo, hi)
            {
                target.SetRange(pos[starts[lo]], pos[starts[hi] - 1] + 1);
                var alignment = (int)target.ParagraphFormat.Alignment;
                if (alignment == (int)WdParagraphAlignment.wdAlignParagraphJustify)
                    target.HighlightColorIndex = color;
                else if (alignment == 9999999 && hi - lo > 1)
                {
                    var mid = (lo + hi) / 2;
                    visit(lo, mid);
                    visit(mid, hi);
                }
            };
            visit(0, starts.Count - 1);
            return true;
        }

        private static void JustifiedTextSlow(Range range, WdColorIndex color)
        {
            foreach (Paragraph paragraph in range.Paragraphs)
            {
                if (paragraph.Alignment == WdParagraphAlignment.wdAlignParagraphJustify)
                    paragraph.Range.HighlightColorIndex = color;
            }
        }

        /// <summary>
        /// Reads the range text once and maps every text index to a document position.
        /// Range.Text leaves out field codes, but they take positions: the begin mark, the code and the separator
        /// before a field result, and the end mark after it. They are skipped using the field ranges. A table cell /
        /// row end mark is "\r\a" in Text but one position. False when the skipped positions plus the text do not
        /// add up to the range length (hidden text, odd fields): offsets cannot be trusted then.
        /// </summary>
        private static bool TryReadText(Range range, out string text, out int[] pos)
        {
            text = range.Text ?? "";
            pos = null;
            var start = range.Start;
            var length = range.End - start;

            var skip = new bool[length];
            var skipped = 0;
            void Skip(int from, int to) // positions [from, to)
            {
                for (var i = Math.Max(from, start); i < Math.Min(to, start + length); i++)
                {
                    if (!skip[i - start]) { skip[i - start] = true; skipped++; }
                }
            }

            var fields = range.Fields;
            var count = fields.Count;
            for (var k = 1; k <= count; k++)
            {
                var field = fields[k];
                var code = field.Code;
                var result = field.Result;
                Skip(code.Start - 1, result.Start); // begin mark, code, separator
                Skip(result.End, result.End + 1);   // end mark
            }

            var positions = new int[text.Length + 1];
            var p = 0;
            for (var i = 0; i < text.Length; i++)
            {
                if (i > 0 && text[i] == '\a' && text[i - 1] == '\r') { positions[i] = positions[i - 1]; continue; }
                while (p < length && skip[p]) p++;
                if (p >= length) return false;
                positions[i] = start + p++;
            }
            while (p < length && skip[p]) p++;
            positions[text.Length] = start + p;
            if (p != length) return false;

            pos = positions;
            return true;
        }

        /// <summary>
        /// Hightlights all single letters and digits that are hanging between 2 spaces in the specified range.
        /// </summary>
        private static void HangingLetters(Range range, WdColorIndex color)
        {
            if (!TryHangingLetters(range, color))
                HangingLettersSlow(range, color);
        }

        private static bool TryHangingLetters(Range range, WdColorIndex color)
        {
            if (!TryReadText(range, out var text, out var pos)) return false;

            // " x " or " x" + paragraph mark, x = any letter or digit. Overlaps allowed ("і в": shared space).
            var hits = new List<int>();
            for (var i = 1; i + 1 < text.Length; i++)
            {
                if (text[i - 1] == ' ' && char.IsLetterOrDigit(text[i]) && (text[i + 1] == ' ' || text[i + 1] == '\r'))
                    hits.Add(i);
            }
            if (hits.Count == 0) return true;

            // Drift guard: the first and last hit must read back as the expected char.
            var target = range.Duplicate;
            foreach (var i in new[] { hits[0], hits[hits.Count - 1] })
            {
                target.SetRange(pos[i], pos[i] + 1);
                if (target.Text != text[i].ToString()) return false;
            }

            foreach (var i in hits)
            {
                target.SetRange(pos[i], pos[i] + 1);
                target.HighlightColorIndex = color;
            }
            return true;
        }

        private static void HangingLettersSlow(Range range, WdColorIndex color)
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
