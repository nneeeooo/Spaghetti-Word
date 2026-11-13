using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Word.Modules;

namespace Tests
{
    [TestClass]
    public class SubtitleConvertTests
    {
        private static string Sample(string name) =>
            File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples", name));

        [TestMethod]
        public void SrtToVtt()
        {
            var result = Subtitles.ConvertFormat(Sample("test.srt"), "vtt", "test.srt");

            StringAssert.StartsWith(result.TrimStart('\uFEFF'), "WEBVTT");
            Assert.IsTrue(result.Contains("-->"));
            Assert.IsTrue(result.Contains("00:00:09.443"));
        }

        [TestMethod]
        public void VttToSrt()
        {
            var result = Subtitles.ConvertFormat(Sample("test.vtt"), "srt", "test.vtt");

            Assert.IsTrue(result.Contains("-->"));
            Assert.IsTrue(result.Contains("00:00:09,443"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void NonSubtitleTextThrows()
        {
            Subtitles.ConvertFormat("just some plain text", "vtt", "x.txt");
        }
    }
}
