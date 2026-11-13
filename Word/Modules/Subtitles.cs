using System.Linq;
using Microsoft.Office.Interop.Word;

namespace Word.Modules
{
    internal static partial class Subtitles
    {
        /// <summary>
        /// Replaces all paragraph break characters in the specified document with a two-em dash character.
        /// </summary>
        /// <param name="doc">The document in which paragraph breaks will be replaced. It must be a valid Document object.</param>
        internal static void EncodeParagraphBreaks(Document doc)
        {
            var find = doc.Range(doc.Content.Start, doc.Content.End - 1).Find; // skip the final paragraph mark
            find.ClearFormatting();
            find.Replacement.ClearFormatting();

            // Only match visible (non-hidden) text
            find.Font.Hidden = 0;

            find.Text = "^p";
            find.Replacement.Text = "⸻"; // U+2E3B TWO-EM DASH

            find.Forward = true;
            find.Wrap = WdFindWrap.wdFindStop; // stay inside the range (final mark excluded)
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
            // U+2E3B TWO-EM DASH, matches what we used earlier; restores paragraph break
            Shared.ReplaceAll(doc.Content, "⸻", "^p");
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
