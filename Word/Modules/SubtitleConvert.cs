using System.Collections.Generic;
using System.IO;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace Word.Modules
{
    // Kept free of Word interop types so the unit tests can link just this file.
    internal static partial class Subtitles
    {
        /// <summary>
        /// Converts subtitle text from one format to another (SRT - VTT).
        /// </summary>
        /// <param name="inputText">Input subtitle string</param>
        /// <param name="targetExtension">Target extension (SRT, VTT)</param>
        /// <param name="sourceFileName">Source filename</param>
        /// <returns>Converted subtitle text.</returns>
        /// <exception cref="InvalidDataException">Input is not SRT/VTT, has no cues, or the target is not SRT/VTT.</exception>
        internal static string ConvertFormat(string inputText, string targetExtension, string sourceFileName)
        {
            // Check WEBVTT first: VTT cues also contain "-->", so the SRT check would catch them.
            SubtitleFormat inputFormat;
            if (inputText.TrimStart().StartsWith("WEBVTT"))
                inputFormat = new WebVTT();
            else if (inputText.Contains("-->"))
                inputFormat = new SubRip();
            else
                throw new InvalidDataException("Invalid subtitle format.\nOnly SRT and VTT are supported.");

            var subtitle = new Subtitle();
            inputFormat.LoadSubtitle(subtitle, new List<string>(
                inputText.Replace('\r', '\n')
                       .Replace("\n\n\n", "\n\n")
                       .Trim()
                       .Split('\n')),
                sourceFileName);

            if (subtitle.Paragraphs.Count == 0)
                throw new InvalidDataException("No subtitle cues found in the document.");

            SubtitleFormat outputFormat;
            var ext = targetExtension.ToLowerInvariant();
            switch (ext)
            {
                case "vtt":
                    outputFormat = new WebVTT();
                    break;
                case "srt":
                    outputFormat = new SubRip();
                    break;
                default:
                    throw new InvalidDataException("Invalid target format.\nOnly SRT and VTT are supported.");
            }

            return outputFormat.ToText(subtitle, Path.GetFileName(sourceFileName));
        }
    }
}
