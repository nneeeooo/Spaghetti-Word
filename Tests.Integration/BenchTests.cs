using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Word.Modules;

namespace Tests.Integration
{
    /// <summary>Prints timings only, asserts nothing. Needs the docx at BenchSource (copied to temp, original untouched).</summary>
    [TestClass]
    public class BenchTests
    {
        private const string BenchSource = @"C:\Users\nneeeooo\Downloads\abx464-105-study-rpt-body_1-428.docx";

        private static Microsoft.Office.Interop.Word.Application App => Word.Globals.ThisAddIn.Application;

        private static void Bench(string name, string copy, Action run)
        {
            var doc = App.Documents.Open(copy, ReadOnly: false);
            try
            {
                var sw = Stopwatch.StartNew();
                run();
                sw.Stop();
                Console.WriteLine($"PERF {name}: {sw.Elapsed.TotalSeconds:F2} s");
            }
            finally { doc.Close(WdSaveOptions.wdDoNotSaveChanges); }
        }

        [TestMethod, TestCategory("Perf")]
        public void Perf_Report()
        {
            if (!File.Exists(BenchSource)) Assert.Inconclusive("bench docx missing");
            var dir = Path.Combine(Path.GetTempPath(), "spaghetti-bench-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            var copy = Path.Combine(dir, "bench.docx");
            File.Copy(BenchSource, copy);
            try
            {
                Bench("text.prepare removeSpacingBeforeAfter", copy, () =>
                    Word.Modules.Text.Prepare(false, false, false, false, false, false, true, false, false, false,
                        false, false, false, false, false, false, false));
                Bench("text.prepare removeTabstops", copy, () =>
                    Word.Modules.Text.Prepare(false, false, false, false, false, false, false, false, false, false,
                        true, false, false, false, false, false, false));
                Bench("spotlight fastDtp", copy, () => Spotlight.Run(doFastDtp: true));
                Bench("spotlight justified", copy, () => Spotlight.Run(doJustifiedText: true));
                Bench("spotlight undelimited", copy, () => Spotlight.Run(doUndelimitedText: true));
            }
            finally { try { Directory.Delete(dir, true); } catch { } }
        }
    }
}
