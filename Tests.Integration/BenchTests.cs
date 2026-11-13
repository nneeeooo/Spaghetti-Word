using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Word.Modules;

namespace Tests.Integration
{
    /// <summary>
    /// Prints timings only, asserts nothing. Needs the docx at BenchSource (copied to temp, original untouched).
    /// With env SPAGHETTI_DUMP_DIR set it also writes a canonical highlight dump per option, to diff two builds.
    /// </summary>
    [TestClass]
    public class BenchTests
    {
        private const string BenchSource = @"C:\Users\nneeeooo\Downloads\abx464-105-study-rpt-body_1-428.docx";

        private static Microsoft.Office.Interop.Word.Application App => Word.Globals.ThisAddIn.Application;

        private static void Bench(string name, string copy, Action run, bool dump = false)
        {
            var doc = App.Documents.Open(copy, ReadOnly: false);
            try
            {
                var sw = Stopwatch.StartNew();
                run();
                sw.Stop();
                Console.WriteLine($"PERF {name}: {sw.Elapsed.TotalSeconds:F2} s");
                var dir = Environment.GetEnvironmentVariable("SPAGHETTI_DUMP_DIR");
                if (dump && !string.IsNullOrEmpty(dir))
                    File.WriteAllText(Path.Combine(dir, name.Replace(' ', '_') + ".txt"), HighlightDump(doc), Encoding.UTF8);
            }
            finally { doc.Close(WdSaveOptions.wdDoNotSaveChanges); }
        }

        /// <summary>Body text with highlighted spans in [ ], paragraphs ended by a pilcrow, from the flat OPC XML.</summary>
        private static string HighlightDump(Document doc)
        {
            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            var xml = XDocument.Parse(doc.Content.WordOpenXML);
            var sb = new StringBuilder();
            var on = false;
            foreach (var p in xml.Descendants(w + "body").Descendants(w + "p"))
            {
                foreach (var r in p.Descendants(w + "r"))
                {
                    var hl = r.Element(w + "rPr")?.Element(w + "highlight");
                    var h = hl != null && (string)hl.Attribute(w + "val") != "none";
                    var text = string.Concat(r.Elements().Select(e =>
                        e.Name == w + "t" ? e.Value : e.Name == w + "tab" ? "\t" : ""));
                    if (text.Length == 0) continue;
                    if (h && !on) sb.Append('[');
                    if (!h && on) sb.Append(']');
                    on = h;
                    sb.Append(text);
                }
                if (on) { sb.Append(']'); on = false; }
                sb.Append('\u00B6').Append('\n');
            }
            return sb.ToString();
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
                Bench("spotlight fastDtp", copy, () => Spotlight.Run(doFastDtp: true), dump: true);
                Bench("spotlight justified", copy, () => Spotlight.Run(doJustifiedText: true), dump: true);
                Bench("spotlight undelimited", copy, () => Spotlight.Run(doUndelimitedText: true), dump: true);
            }
            finally { try { Directory.Delete(dir, true); } catch { } }
        }
    }
}
