using System;
using System.IO;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Word.Modules;

namespace Tests.Integration
{
    [TestClass]
    public class ContextMenuTests
    {
        private static Microsoft.Office.Interop.Word.Application App => Word.Globals.ThisAddIn.Application;

        [TestInitialize]
        public void Setup() => CloseAll();

        [TestCleanup]
        public void Teardown()
        {
            if (App != null) CloseAll();
        }

        private static void CloseAll()
        {
            for (var i = App.Documents.Count; i >= 1; i--)
                App.Documents[i].Close(WdSaveOptions.wdDoNotSaveChanges);
        }

        private static Document TwoTables()
        {
            var doc = App.Documents.Add();
            doc.Content.Text = "before\r";
            var r = doc.Range(doc.Content.End - 1, doc.Content.End - 1);
            doc.Tables.Add(r, 2, 2);
            doc.Content.InsertParagraphAfter();
            doc.Content.InsertAfter("middle");
            doc.Content.InsertParagraphAfter();
            r = doc.Range(doc.Content.End - 1, doc.Content.End - 1);
            doc.Tables.Add(r, 2, 2);
            Assert.AreEqual(2, doc.Tables.Count);
            foreach (Table t in doc.Tables)
            {
                t.Range.ParagraphFormat.SpaceBefore = 7;
                t.Range.ParagraphFormat.SpaceAfter = 9;
                t.Rows.AllowBreakAcrossPages = -1;
            }
            return doc;
        }

        private static TablePrepSettings Settings() => new TablePrepSettings
        {
            RemoveSpacingBeforeAfter = true,
            DoNotBreakAcrossPages = true,
            ResetPaginationSettings = true,
        };

        private static void AssertUntouched(Table t)
        {
            Assert.AreEqual(7f, t.Range.Paragraphs[1].SpaceBefore, 0.01f);
            Assert.AreEqual(9f, t.Range.Paragraphs[1].SpaceAfter, 0.01f);
            Assert.AreEqual(-1, t.Rows.AllowBreakAcrossPages);
        }

        private static void AssertFixed(Table t)
        {
            Assert.AreEqual(0f, t.Range.Paragraphs[1].SpaceBefore, 0.01f);
            Assert.AreEqual(0f, t.Range.Paragraphs[1].SpaceAfter, 0.01f);
            Assert.AreEqual(0, t.Rows.AllowBreakAcrossPages);
        }

        [TestMethod, TestCategory("Word")]
        public void FixTable_ChangesOnlyTargetTable_AndIsOneUndoStep()
        {
            var doc = TwoTables();
            doc.Tables[2].Cell(1, 1).Range.Select();
            App.Selection.Collapse(WdCollapseDirection.wdCollapseStart);

            var target = Word.Modules.Tables.GetSelectionOuterTable(App.Selection);
            Settings().Apply(onlyTable: target, undoName: "Fix table");

            AssertUntouched(doc.Tables[1]);
            AssertFixed(doc.Tables[2]);

            doc.Undo(1);
            AssertUntouched(doc.Tables[2]);
        }

        [TestMethod, TestCategory("Word")]
        public void Prepare_ConvertSelectionOnly_TouchesOnlySelectedTable()
        {
            var doc = TwoTables();
            doc.Tables[1].Cell(1, 1).Range.Select();
            App.Selection.Collapse(WdCollapseDirection.wdCollapseStart);

            Settings().Apply(convertSelectionOnly: true);

            AssertFixed(doc.Tables[1]);
            AssertUntouched(doc.Tables[2]);
        }

        [TestMethod, TestCategory("Word")]
        public void FixTable_NestedSelection_TargetsOuterTable()
        {
            var doc = TwoTables();
            doc.Tables[1].Cell(1, 1).Tables.Add(doc.Tables[1].Cell(1, 1).Range, 1, 1);
            var inner = doc.Tables[1].Cell(1, 1).Tables[1];
            inner.Cell(1, 1).Range.Select();
            App.Selection.Collapse(WdCollapseDirection.wdCollapseStart);

            var target = Word.Modules.Tables.GetSelectionOuterTable(App.Selection);
            Assert.AreEqual(1, target.NestingLevel);
            Assert.AreEqual(doc.Tables[1].Range.Start, target.Range.Start);
        }

        [TestMethod, TestCategory("Word")]
        public void PageBreakBefore_Toggle_MixedSelection_OnThenOff()
        {
            var doc = App.Documents.Add();
            doc.Content.Text = "one\rtwo\rthree";
            doc.Paragraphs[2].PageBreakBefore = -1;
            doc.Content.Select();

            Assert.IsFalse(ParagraphOps.AllSelectedHavePageBreakBefore());

            ParagraphOps.TogglePageBreakBefore();
            foreach (Paragraph p in doc.Paragraphs) Assert.AreEqual(-1, p.PageBreakBefore);
            Assert.IsTrue(ParagraphOps.AllSelectedHavePageBreakBefore());

            ParagraphOps.TogglePageBreakBefore();
            foreach (Paragraph p in doc.Paragraphs) Assert.AreEqual(0, p.PageBreakBefore);
            Assert.IsFalse(ParagraphOps.AllSelectedHavePageBreakBefore());
        }
    }
}
