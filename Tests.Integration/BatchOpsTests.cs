using System;
using System.IO;
using System.Linq;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Word.Modules;

namespace Tests.Integration
{
    [TestClass]
    public class BatchOpsTests
    {
        private static Microsoft.Office.Interop.Word.Application App => Word.Globals.ThisAddIn.Application;
        private string _dir;

        [TestInitialize]
        public void Setup()
        {
            _dir = Path.Combine(Path.GetTempPath(), "spaghetti-it-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
            CloseAll();
        }

        [TestCleanup]
        public void Teardown()
        {
            if (App != null) CloseAll();
            try { Directory.Delete(_dir, true); } catch { }
        }

        private static void CloseAll()
        {
            for (var i = App.Documents.Count; i >= 1; i--)
                App.Documents[i].Close(WdSaveOptions.wdDoNotSaveChanges);
        }

        private static Document NewDoc(string text)
        {
            var doc = App.Documents.Add();
            doc.Content.InsertAfter(text);
            return doc;
        }

        private static int Custom(Paragraph p) => p.TabStops.Cast<TabStop>().Count(t => t.CustomTab);

        private static void PrepareText(bool spacing = false, bool tabstops = false)
        {
            Word.Modules.Text.Prepare(false, false, false, false, false, false, spacing, false, false, false,
                tabstops, false, false, false, false, false, false);
        }

        [TestMethod, TestCategory("Word")]
        public void Spacing_RemovedForAllParagraphs_AndUndoRestores()
        {
            var doc = NewDoc("one\rtwo\rthree");
            foreach (Paragraph p in doc.Paragraphs) { p.SpaceBefore = 12; p.SpaceAfter = 18; }
            doc.Paragraphs[2].SpaceBefore = 6;

            PrepareText(spacing: true);

            foreach (Paragraph p in doc.Paragraphs)
            {
                Assert.AreEqual(0f, p.SpaceBefore, 0.01f);
                Assert.AreEqual(0f, p.SpaceAfter, 0.01f);
            }

            doc.Undo(1);
            Assert.AreEqual(12f, doc.Paragraphs[1].SpaceBefore, 0.01f);
            Assert.AreEqual(6f, doc.Paragraphs[2].SpaceBefore, 0.01f);
            Assert.AreEqual(18f, doc.Paragraphs[3].SpaceAfter, 0.01f);
        }

        [TestMethod, TestCategory("Word")]
        public void Spacing_AlsoAppliesToHeader()
        {
            var doc = NewDoc("body");
            var header = doc.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
            header.Text = "head";
            header.ParagraphFormat.SpaceAfter = 20;

            PrepareText(spacing: true);

            Assert.AreEqual(0f,
                doc.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.ParagraphFormat.SpaceAfter, 0.01f);
        }

        [TestMethod, TestCategory("Word")]
        public void Tabstops_ClearedPerParagraph_WithDifferentStops()
        {
            var doc = NewDoc("a\tb\rc\td\re");
            doc.Paragraphs[1].TabStops.Add(50f);
            doc.Paragraphs[1].TabStops.Add(120f);
            doc.Paragraphs[2].TabStops.Add(80f, WdTabAlignment.wdAlignTabCenter);
            // paragraph 3 keeps none
            Assert.IsTrue(Custom(doc.Paragraphs[1]) == 2);
            Assert.IsTrue(Custom(doc.Paragraphs[2]) == 1);

            PrepareText(tabstops: true);

            foreach (Paragraph p in doc.Paragraphs)
                Assert.AreEqual(0, Custom(p));
        }

        [TestMethod, TestCategory("Word")]
        public void Tables_SpacingPaginationAndRowBreak()
        {
            var src = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples", "test_tables.docx");
            var copy = Path.Combine(_dir, "test_tables.docx");
            File.Copy(src, copy);
            var doc = App.Documents.Open(copy, ReadOnly: false);
            Assert.IsTrue(doc.Tables.Count > 0);
            foreach (Table t in doc.Tables)
            {
                foreach (Paragraph p in t.Range.Paragraphs)
                {
                    p.SpaceBefore = 7; p.SpaceAfter = 9;
                    p.KeepWithNext = -1; p.KeepTogether = -1; p.PageBreakBefore = -1;
                }
                t.Rows.AllowBreakAcrossPages = -1; Assert.AreEqual(-1, t.Rows.AllowBreakAcrossPages);
            }

            Word.Modules.Tables.Prepare(false, false, false, doRemoveSpacingBeforeAfter: true, doDoNotBreakAcrossPages: true,
                doSetZeroMargins: false, doSetDefaultMargins: false, doSetCustomMargins: false,
                doResetPaginationSettings: true, doRemoveBorders: false);

            foreach (Table t in doc.Tables)
            {
                foreach (Paragraph p in t.Range.Paragraphs)
                {
                    Assert.AreEqual(0f, p.SpaceBefore, 0.01f);
                    Assert.AreEqual(0f, p.SpaceAfter, 0.01f);
                    Assert.AreEqual(0, p.KeepWithNext);
                    Assert.AreEqual(0, p.KeepTogether);
                    Assert.AreEqual(0, p.PageBreakBefore);
                }
                Assert.AreEqual(0, t.Rows.AllowBreakAcrossPages);
            }
        }

        // ---- highlight ----

        private static bool IsHanging(string s, int i) =>
            char.IsLetterOrDigit(s[i]) && i > 0 && s[i - 1] == ' ' && (i + 1 == s.Length || s[i + 1] == ' ' || s[i + 1] == '\r');

        private static void AssertHighlight(Range doc, string s, Func<int, bool> expected, WdColorIndex color)
        {
            for (var i = 0; i < s.Length; i++)
            {
                var hl = doc.Document.Range(doc.Start + i, doc.Start + i + 1).HighlightColorIndex;
                Assert.AreEqual(expected(i) ? color : WdColorIndex.wdNoHighlight, hl, $"char {i} '{s[i]}'");
            }
        }

        [TestMethod, TestCategory("Word")]
        public void FastDtp_DigitsDashesAndHangingLetters()
        {
            const string s = "він і в місті а б 12 – x—y 7 z\r5";
            var doc = NewDoc(s);
            var before = doc.Content.Text;
            var count = doc.Characters.Count;

            Spotlight.Run(doFastDtp: true);

            AssertHighlight(doc.Content, s, i =>
                char.IsDigit(s[i]) || "–—‒".IndexOf(s[i]) >= 0 || IsHanging(s, i), WdColorIndex.wdYellow);
            Assert.AreEqual(before, doc.Content.Text);
            Assert.AreEqual(count, doc.Characters.Count);
        }

        [TestMethod, TestCategory("Word")]
        public void Highlight_UsesRequestedColor_RestoresDefaultColor_UndoRemoves()
        {
            var doc = NewDoc("a 1 b");
            App.Options.DefaultHighlightColorIndex = WdColorIndex.wdTurquoise;

            Spotlight.Run(doFastDtp: true, color: WdColorIndex.wdBrightGreen);

            Assert.AreEqual(WdColorIndex.wdTurquoise, App.Options.DefaultHighlightColorIndex);
            Assert.AreEqual(WdColorIndex.wdBrightGreen, doc.Range(2, 3).HighlightColorIndex);
            Assert.AreEqual("a 1 b", doc.Content.Text.TrimEnd('\r'));

            doc.Undo(1);
            Assert.AreEqual(WdColorIndex.wdNoHighlight, doc.Range(2, 3).HighlightColorIndex);
            App.Options.DefaultHighlightColorIndex = WdColorIndex.wdYellow;
        }

        [TestMethod, TestCategory("Word")]
        public void Highlight_FootnoteAndHeader()
        {
            var doc = NewDoc("body text");
            doc.Footnotes.Add(doc.Range(0, 4), Text: "note і 2");
            var header = doc.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
            header.Text = "head 3 – q";

            Spotlight.Run(doFastDtp: true);

            var note = doc.Footnotes[1].Range;
            var nt = note.Text;
            for (var i = 0; i < nt.Length; i++)
            {
                var exp = char.IsDigit(nt[i]) || IsHanging(nt, i);
                Assert.AreEqual(exp ? WdColorIndex.wdYellow : WdColorIndex.wdNoHighlight,
                    note.Document.Range().Footnotes[1].Range.Characters[i + 1].HighlightColorIndex, $"footnote char {i}");
            }
            var h = doc.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
            var ht = h.Text;
            for (var i = 0; i < ht.Length; i++)
            {
                var exp = char.IsDigit(ht[i]) || ht[i] == '–' || IsHanging(ht, i);
                Assert.AreEqual(exp ? WdColorIndex.wdYellow : WdColorIndex.wdNoHighlight,
                    h.Characters[i + 1].HighlightColorIndex, $"header char {i} '{ht[i]}'");
            }
        }

        [TestMethod, TestCategory("Word")]
        public void Justified_HighlightsWholeJustifiedParagraphs()
        {
            var doc = NewDoc("left one\rjust two\rjust three\rleft four");
            doc.Paragraphs[1].Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            doc.Paragraphs[2].Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[3].Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            doc.Paragraphs[4].Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            var header = doc.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
            header.Text = "justified header";
            header.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
            var before = doc.Content.Text;

            Spotlight.Run(doJustifiedText: true, color: WdColorIndex.wdPink);

            Assert.AreEqual(WdColorIndex.wdNoHighlight, doc.Paragraphs[1].Range.HighlightColorIndex);
            Assert.AreEqual(WdColorIndex.wdPink, doc.Paragraphs[2].Range.HighlightColorIndex, "whole paragraph incl. mark");
            Assert.AreEqual(WdColorIndex.wdPink, doc.Paragraphs[3].Range.HighlightColorIndex);
            Assert.AreEqual(WdColorIndex.wdNoHighlight, doc.Paragraphs[4].Range.HighlightColorIndex);
            Assert.AreEqual(WdColorIndex.wdPink,
                doc.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Characters[1].HighlightColorIndex);
            Assert.AreEqual(before, doc.Content.Text);
        }
    }
}
