using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Word.Helpers;
using Word.Modules;

namespace Tests
{
    [TestClass]
    public class SubtitleEdgeCaseTests
    {
        private static string Sample(string name) =>
            File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples", name));

        private static int Cues(string s) => s.Split(new[] { "-->" }, StringSplitOptions.None).Length - 1;

        [TestMethod]
        public void SrtToSrt_KeepsCuesAndFirstTimestamp()
        {
            var src = Sample("test.srt");
            var r = Subtitles.ConvertFormat(src, "srt", "test.srt");
            Assert.AreEqual(Cues(src), Cues(r));
            Assert.IsTrue(r.Contains("00:00:09,443 --> 00:00:13,413"));
        }

        [TestMethod]
        public void VttToVtt_KeepsCuesAndFirstTimestamp()
        {
            var src = Sample("test.vtt");
            var r = Subtitles.ConvertFormat(src, "vtt", "test.vtt");
            Assert.AreEqual(Cues(src), Cues(r));
            Assert.IsTrue(r.Contains("00:00:09.443"));
        }

        [TestMethod]
        public void CrlfInput()
        {
            var src = Sample("test.srt").Replace("\r\n", "\n").Replace("\n", "\r\n");
            var r = Subtitles.ConvertFormat(src, "vtt", "test.srt");
            Assert.IsTrue(r.Contains("00:00:09.443"));
        }

        [TestMethod]
        public void ExtraBlankLines()
        {
            var src = Sample("test.srt").Replace("\r\n", "\n").Replace("\n\n", "\n\n\n");
            var r = Subtitles.ConvertFormat(src, "vtt", "test.srt");
            Assert.AreEqual(Cues(Sample("test.srt")), Cues(r));
        }

        [TestMethod]
        public void UppercaseTargetExtension()
        {
            var r = Subtitles.ConvertFormat(Sample("test.srt"), "SRT", "test.srt");
            Assert.IsTrue(r.Contains("00:00:09,443"));
            r = Subtitles.ConvertFormat(Sample("test.srt"), "VTT", "test.srt");
            StringAssert.StartsWith(r.TrimStart('﻿'), "WEBVTT");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void UnknownTargetThrows() => Subtitles.ConvertFormat(Sample("test.srt"), "ass", "test.srt");

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void EmptyThrows() => Subtitles.ConvertFormat("", "srt", "x.srt");
    }

    [TestClass]
    public class ImagesTests
    {
        private string _dir;

        [TestInitialize]
        public void Init()
        {
            _dir = Path.Combine(Path.GetTempPath(), "spag_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        [TestCleanup]
        public void Done()
        {
            try { Directory.Delete(_dir, true); } catch { }
        }

        private string CopySample(string name)
        {
            var dst = Path.Combine(_dir, name);
            File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples", name), dst);
            return dst;
        }

        [TestMethod]
        public void Extract_WritesNamedFiles_AndReleasesDocx()
        {
            var docx = CopySample("test.docx");
            var outDir = Path.Combine(_dir, "out");
            Images.ExtractImages(docx, outDir);

            var files = Directory.GetFiles(outDir);
            Assert.IsTrue(files.Length > 0);
            Assert.IsTrue(files.All(f => Path.GetFileName(f).StartsWith("image_")));
            Assert.IsTrue(files.Any(f => Path.GetFileNameWithoutExtension(f) == "image_001"));
            Assert.IsTrue(files.All(f => new FileInfo(f).Length > 0));
            File.Delete(docx);
            Assert.IsFalse(File.Exists(docx));
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Extract_MissingFile_Throws() =>
            Images.ExtractImages(Path.Combine(_dir, "nope.docx"), Path.Combine(_dir, "o"));

        [TestMethod]
        public void Extract_NoImages_Throws_AndReleasesFile()
        {
            var docx = Path.Combine(_dir, "empty.docx");
            using (var d = WordprocessingDocument.Create(docx, WordprocessingDocumentType.Document))
            {
                var main = d.AddMainDocumentPart();
                main.Document = new Document(new Body(new Paragraph(new Run(new Text("hi")))));
                main.Document.Save();
            }
            Assert.ThrowsException<InvalidDataException>(() => Images.ExtractImages(docx, Path.Combine(_dir, "o")));
            File.Delete(docx);
        }

        [TestMethod]
        public void GetImageExtension_Maps()
        {
            Assert.AreEqual(".png", Images.GetImageExtension("image/png"));
            Assert.AreEqual(".jpg", Images.GetImageExtension("image/jpeg"));
            Assert.AreEqual(".gif", Images.GetImageExtension("image/gif"));
            Assert.AreEqual(".tif", Images.GetImageExtension("image/tiff"));
            Assert.AreEqual(".bmp", Images.GetImageExtension("image/bmp"));
            Assert.AreEqual(".emf", Images.GetImageExtension("image/x-emf"));
            Assert.AreEqual(".wmf", Images.GetImageExtension("image/x-wmf"));
            Assert.AreEqual(".bin", Images.GetImageExtension("image/webp"));
        }
    }

    [TestClass]
    public class RegistryHelperTests
    {
        private string _path;
        private CultureInfo _old;

        [TestInitialize]
        public void Init()
        {
            _path = @"Software\SpaghettiTests\" + Guid.NewGuid().ToString("N");
            _old = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("uk-UA");
        }

        [TestCleanup]
        public void Done()
        {
            Thread.CurrentThread.CurrentCulture = _old;
            Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(_path, false);
        }

        [TestMethod]
        public void BoolRoundTrip()
        {
            RegistryHelper.SaveValue(_path, "b", true);
            Assert.IsTrue(RegistryHelper.LoadValue(_path, "b", false));
            RegistryHelper.SaveValue(_path, "b", false);
            Assert.IsFalse(RegistryHelper.LoadValue(_path, "b", true));
        }

        [TestMethod]
        public void IntRoundTrip()
        {
            RegistryHelper.SaveValue(_path, "i", 42);
            Assert.AreEqual(42, RegistryHelper.LoadValue(_path, "i", 0));
        }

        [TestMethod]
        public void FloatRoundTrip_InvariantCulture()
        {
            RegistryHelper.SaveValue(_path, "f", 1.5f);
            Assert.AreEqual(1.5f, RegistryHelper.LoadValue(_path, "f", 0f));
            using (var k = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(_path))
                Assert.AreEqual("1.5", (string)k.GetValue("f"));
        }

        [TestMethod]
        public void DefaultsWhenMissing()
        {
            Assert.IsTrue(RegistryHelper.LoadValue(_path, "x", true));
            Assert.AreEqual(7, RegistryHelper.LoadValue(_path, "x", 7));
            Assert.AreEqual(2.5f, RegistryHelper.LoadValue(_path, "x", 2.5f));
        }
    }

    [TestClass]
    public class FontIndexTests
    {
        [TestMethod]
        public void ArialMapsToExistingFile()
        {
            var idx = Word.Modules.Fonts.BuildFontPathIndex();
            Assert.IsTrue(idx.TryGetValue("Arial", out var path));
            Assert.IsTrue(File.Exists(path));
        }
    }
}
