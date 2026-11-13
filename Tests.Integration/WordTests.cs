using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Word.Modules;

namespace Tests.Integration
{
    [TestClass]
    public class WordTests
    {
        private static Microsoft.Office.Interop.Word.Application _app;
        private string _dir;

        [AssemblyInitialize]
        public static void Init(TestContext _)
        {
            try
            {
                // always a NEW instance, never GetActiveObject
                _app = new Microsoft.Office.Interop.Word.Application
                {
                    Visible = false,
                    DisplayAlerts = WdAlertLevel.wdAlertsNone
                };
            }
            catch (Exception ex)
            {
                Assert.Inconclusive("Word cannot start: " + ex.Message);
            }
            Word.Globals.ThisAddIn.Application = _app;
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            if (_app == null) return;
            try
            {
                for (var i = _app.Documents.Count; i >= 1; i--)
                    _app.Documents[i].Close(WdSaveOptions.wdDoNotSaveChanges);
                _app.Quit(WdSaveOptions.wdDoNotSaveChanges);
            }
            catch { }
            Marshal.ReleaseComObject(_app);
            _app = null;
        }

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
            if (_app != null) CloseAll();
            try { Directory.Delete(_dir, true); } catch { }
        }

        private static void CloseAll()
        {
            for (var i = _app.Documents.Count; i >= 1; i--)
                _app.Documents[i].Close(WdSaveOptions.wdDoNotSaveChanges);
        }

        private static Document NewDoc() => _app.Documents.Add();

        private static Range EndRange(Document doc)
        {
            var r = doc.Content;
            r.MoveEnd(WdUnits.wdCharacter, -1);
            r.Collapse(WdCollapseDirection.wdCollapseEnd);
            return r;
        }

        private static void Prepare(bool sectionBreaks = false, bool trim = false, bool specialIndent = false)
        {
            Word.Modules.Text.Prepare(false, sectionBreaks, false, trim, false, false, false, false, false, specialIndent,
                false, false, false, false, false, false, false);
        }

        private static string Plain(Document doc) => doc.Content.Text.TrimEnd('\r');

        [TestMethod, TestCategory("Word")]
        public void Fix3_RemoveSectionBreaks_ReplacesByType()
        {
            var doc = NewDoc();
            EndRange(doc).InsertAfter("one");
            EndRange(doc).InsertBreak(WdBreakType.wdSectionBreakNextPage);
            EndRange(doc).InsertAfter("two");
            EndRange(doc).InsertBreak(WdBreakType.wdSectionBreakContinuous);
            EndRange(doc).InsertAfter("three");
            Assert.AreEqual(3, doc.Sections.Count);

            Prepare(sectionBreaks: true);

            Assert.AreEqual(1, doc.Sections.Count);
            var text = doc.Content.Text;
            Assert.AreEqual(1, text.Count(c => c == '\f'), "page breaks (from next-page section)");
            Assert.AreEqual(1, text.Count(c => c == '\v'), "line breaks (from continuous section)");
        }

        [TestMethod, TestCategory("Word")]
        public void Fix4_FastDtp_HighlightsHangingLetters()
        {
            var doc = NewDoc();
            const string s = "\u0432\u0456\u043d \u0456 \u0432 \u043c\u0456\u0441\u0442\u0456 \u0430 \u0431 \u0449\u0435";
            doc.Content.InsertAfter(s);

            Spotlight.Run(doFastDtp: true);

            for (var i = 0; i < s.Length; i++)
            {
                var hl = doc.Range(i, i + 1).HighlightColorIndex;
                var expected = i == 4 || i == 6 || i == 14 || i == 16; // i v a b
                Assert.AreEqual(expected ? WdColorIndex.wdYellow : WdColorIndex.wdNoHighlight, hl,
                    $"char {i} '{s[i]}'");
            }
        }

        [TestMethod, TestCategory("Word")]
        public void Fix6_RemoveSpecialIndent_KeepsLeftIndent()
        {
            var doc = NewDoc();
            doc.Content.InsertAfter("indented");
            var pf = doc.Paragraphs[1].Format;
            pf.CharacterUnitLeftIndent = 3;
            pf.CharacterUnitFirstLineIndent = 2;
            var left = pf.LeftIndent;
            Assert.IsTrue(left > 0);

            Prepare(specialIndent: true);

            pf = doc.Paragraphs[1].Format;
            Assert.AreEqual(0f, pf.FirstLineIndent, 0.01f);
            Assert.AreEqual(left, pf.LeftIndent, 0.01f);
            Assert.AreEqual(3f, pf.CharacterUnitLeftIndent, 0.01f);
        }

        [TestMethod, TestCategory("Word")]
        public void Fix9_ParagraphBreaks_RoundTrip()
        {
            var doc = NewDoc();
            var srt = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples", "test.srt"))
                .Replace("\r\n", "\r").Replace("\n", "\r").TrimEnd('\r');
            doc.Content.InsertAfter(srt);
            var before = doc.Content.Text;

            Subtitles.EncodeParagraphBreaks(doc);
            StringAssert.Contains(doc.Content.Text, "\u2E3B");
            Subtitles.DecodeParagraphBreaks(doc);

            var after = doc.Content.Text;
            var d = Enumerable.Range(0, Math.Min(before.Length, after.Length)).FirstOrDefault(i => before[i] != after[i]);
            Assert.AreEqual(before, after, $"len {before.Length}/{after.Length}, first diff at {d}: [{before.Substring(Math.Max(0, d - 5), Math.Min(20, before.Length - Math.Max(0, d - 5)))}] vs [{after.Substring(Math.Max(0, d - 5), Math.Min(20, after.Length - Math.Max(0, d - 5)))}]");
        }

        [TestMethod, TestCategory("Word")]
        public void Issue14_ReplaceDocumentFont_ChangesHeader()
        {
            var doc = NewDoc();
            doc.Content.InsertAfter("body");
            var header = doc.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
            header.Text = "Header text";
            header.Font.Name = "Arial";
            Assert.AreEqual("Arial", doc.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Font.Name);

            Fonts.ReplaceDocumentFont("Arial", "Times New Roman");

            Assert.AreEqual("Times New Roman",
                doc.Sections[1].Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary].Range.Font.Name);
        }

        [TestMethod, TestCategory("Word")]
        public void TextPrepare_TrimSpaces()
        {
            var doc = NewDoc();
            doc.Content.InsertAfter("a  b ( c )");

            Prepare(trim: true);

            Assert.AreEqual("a b (c)", Plain(doc));
        }

        [TestMethod, TestCategory("Word")]
        public void TablesPrepare_RemoveBorders()
        {
            var src = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples", "test_tables.docx");
            var copy = Path.Combine(_dir, "test_tables.docx");
            File.Copy(src, copy);
            var doc = _app.Documents.Open(copy, ReadOnly: false);
            Assert.IsTrue(doc.Tables.Count > 0);

            Word.Modules.Tables.Prepare(false, false, false, false, false, false, false, false, false, doRemoveBorders: true);

            foreach (Table t in doc.Tables)
                Assert.AreEqual(0, t.Borders.Enable);
        }

        [TestMethod, TestCategory("Word")]
        public void Shared_HasActiveDocument()
        {
            CloseAll();
            Assert.IsFalse(Shared.HasActiveDocument());
            NewDoc();
            Assert.IsTrue(Shared.HasActiveDocument());
        }
    }
}

