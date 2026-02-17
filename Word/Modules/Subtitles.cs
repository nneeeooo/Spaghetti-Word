using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Office.Interop.Word;
using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;

namespace Word.Modules
{
    internal static class Subtitles
    {
        /// <summary>
        /// Converts subtitle text from one format to another (SRT - VTT).
        /// </summary>
        /// <param name="inputText">Input subtitle string</param>
        /// <param name="targetExtension">Target extension (SRT, VTT)</param>
        /// <param name="sourceFileName">Source filename</param>
        /// <returns></returns>
        internal static string ConvertFormat(string inputText, string targetExtension, string sourceFileName)
        {
            SubtitleFormat inputFormat;
            if (inputText.Contains("-->"))
                inputFormat = new SubRip();
            else if (inputText.Contains("WEBVTT"))
                inputFormat = new WebVTT();
            else
                return "Invalid subtitle format.\nOnly SRT and VTT are supported.";

            var subtitle = new Subtitle();
            inputFormat.LoadSubtitle(subtitle, new List<string>(
                inputText.Replace('\r', '\n')
                       .Replace("\n\n\n", "\n\n")
                       .Trim()
                       .Split('\n')),
                sourceFileName);

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
                    return "Invalid subtitle format.\nOnly SRT and VTT are supported.";
            }

            return outputFormat.ToText(subtitle, Path.GetFileName(sourceFileName));
        }

        /// <summary>
        /// Replaces all paragraph break characters in the specified document with a two-em dash character.
        /// </summary>
        /// <param name="doc">The document in which paragraph breaks will be replaced. It must be a valid Document object.</param>
        internal static void EncodeParagraphBreaks(Document doc)
        {
            var find = doc.Content.Find;
            find.ClearFormatting();
            find.Replacement.ClearFormatting();

            // Only match visible (non-hidden) text
            find.Font.Hidden = 0;

            find.Text = "^p";
            find.Replacement.Text = "⸻"; // U+2E3B TWO-EM DASH

            find.Forward = true;
            find.Wrap = WdFindWrap.wdFindContinue;
            find.Format = true; // must be true when using Font settings
            find.MatchCase = false;
            find.MatchWholeWord = false;
            find.MatchWildcards = false;
            find.MatchSoundsLike = false;
            find.MatchAllWordForms = false;

            find.Execute(Replace: WdReplace.wdReplaceAll);
        }

        /// <summary>
        /// Replaces all occurrences of the TWO-EM DASH character (U+2E3B) in the specified document with paragraph
        /// breaks.
        /// </summary>
        /// <param name="doc">The document in which paragraph breaks are to be decoded. This parameter must not be null.</param>
        internal static void DecodeParagraphBreaks(Document doc)
        {
            var find = doc.Content.Find;
            find.ClearFormatting();
            find.Replacement.ClearFormatting();

            find.Text = "⸻"; // U+2E3B TWO-EM DASH — matches what we used earlier
            find.Replacement.Text = "^p"; // restores paragraph break

            find.Forward = true;
            find.Wrap = WdFindWrap.wdFindContinue;
            find.Format = false;
            find.MatchCase = false;
            find.MatchWholeWord = false;
            find.MatchWildcards = false;
            find.MatchSoundsLike = false;
            find.MatchAllWordForms = false;

            find.Execute(Replace: WdReplace.wdReplaceAll);
        }

        internal static void RemoveExcessiveParagraphs(Application app)
        {
            // TODO: Figure out why I added this method in the first place, probably fixed some cases. It was most likely made for transcription Docx preparation.
            var selection = app.Selection;

            for (var count = 30; count >= 4; count--)
            {
                var target = string.Concat(Enumerable.Repeat("^p", count));
                var replacement = "^p^p";

                var find = selection.Find;
                find.ClearFormatting();
                find.Replacement.ClearFormatting();

                find.Text = target;
                find.Replacement.Text = replacement;
                find.Forward = true;
                find.Wrap = WdFindWrap.wdFindContinue;
                find.Format = false;
                find.MatchCase = false;
                find.MatchWholeWord = false;
                find.MatchWildcards = false;

                find.Execute(Replace: WdReplace.wdReplaceAll);
            }
        }
    }
}
