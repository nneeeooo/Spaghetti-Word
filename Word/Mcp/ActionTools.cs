using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using Interop = Microsoft.Office.Interop.Word;

namespace Word.Mcp
{
    /// <summary>
    /// action_list / action_run / undo: an allow-list mapping action names to existing module calls.
    /// </summary>
    internal static class ActionTools
    {
        private sealed class ActionDef
        {
            public string Description;
            public bool NeedsDocument;
            public Func<JsonObject, JsonNode> Run;
        }

        private static readonly Dictionary<string, ActionDef> Actions = Build();

        private static Dictionary<string, ActionDef> Build()
        {
            var d = new Dictionary<string, ActionDef>(StringComparer.Ordinal);

            d["text.prepare"] = new ActionDef
            {
                NeedsDocument = true,
                Description = "Word Prep text part. Boolean flags (default false): removeColumnBreaks, removeSectionBreaks, setSingleColumn, trimSpaces, fixFontProperties, setLinespacingSingle, removeSpacingBeforeAfter, removeLeftIndent, removeRightIndent, removeSpecianIndent, removeTabstops, removeTabs, lockImageAspectRatio, autofitTextboxes, removeOptionalHyphens, disableAutomaticHyphenation, removeSoftbreaks ('do' prefix accepted).",
                Run = a =>
                {
                    Word.Modules.Text.Prepare(
                        J.Flag(a, "RemoveColumnBreaks"), J.Flag(a, "RemoveSectionBreaks"), J.Flag(a, "SetSingleColumn"),
                        J.Flag(a, "TrimSpaces"), J.Flag(a, "FixFontProperties"), J.Flag(a, "SetLinespacingSingle"),
                        J.Flag(a, "RemoveSpacingBeforeAfter"), J.Flag(a, "RemoveLeftIndent"), J.Flag(a, "RemoveRightIndent"),
                        J.Flag(a, "RemoveSpecianIndent") || J.Flag(a, "RemoveSpecialIndent"),
                        J.Flag(a, "RemoveTabstops"), J.Flag(a, "RemoveTabs"), J.Flag(a, "LockImageAspectRatio"),
                        J.Flag(a, "AutofitTextboxes"), J.Flag(a, "RemoveOptionalHyphens"),
                        J.Flag(a, "DisableAutomaticHyphenation"), J.Flag(a, "RemoveSoftbreaks"));
                    return null;
                }
            };

            d["tables.prepare"] = new ActionDef
            {
                NeedsDocument = true,
                Description = "Word Prep tables part. Boolean flags (default false): autofitWindow, autofitRowHeight, doNotResizeToFitContents, removeSpacingBeforeAfter, doNotBreakAcrossPages, setZeroMargins, setDefaultMargins, setCustomMargins, resetPaginationSettings, removeBorders, convertSelectionOnly. Numbers: customMarginTop/Bottom/Left/Right.",
                Run = a =>
                {
                    Word.Modules.Tables.Prepare(
                        J.Flag(a, "AutofitWindow"), J.Flag(a, "AutofitRowHeight"), J.Flag(a, "DoNotResizeToFitContents"),
                        J.Flag(a, "RemoveSpacingBeforeAfter"), J.Flag(a, "DoNotBreakAcrossPages"),
                        J.Flag(a, "SetZeroMargins"), J.Flag(a, "SetDefaultMargins"), J.Flag(a, "SetCustomMargins"),
                        J.Flag(a, "ResetPaginationSettings"), J.Flag(a, "RemoveBorders"),
                        J.Flag(a, "ConvertSelectionOnly"),
                        J.Float(a, "customMarginTop"), J.Float(a, "customMarginBottom"),
                        J.Float(a, "customMarginLeft"), J.Float(a, "customMarginRight"));
                    return null;
                }
            };

            d["spotlight.run"] = new ActionDef
            {
                NeedsDocument = true,
                Description = "Highlight preset. Args: fastDtp, justified, undelimited (booleans), color (Word color name, default yellow, e.g. brightGreen, turquoise).",
                Run = a =>
                {
                    Word.Modules.Spotlight.Run(
                        doFastDtp: J.Flag(a, "FastDtp"),
                        doJustifiedText: J.Flag(a, "Justified"),
                        doUndelimitedText: J.Flag(a, "Undelimited"),
                        color: ParseColor(J.Str(a, "color")));
                    return null;
                }
            };

            d["spotlight.clear"] = new ActionDef
            {
                NeedsDocument = true,
                Description = "Remove ALL highlights from the active document.",
                Run = a => { Word.Modules.Spotlight.Clear(); return null; }
            };
            d["text.global_hide"] = new ActionDef
            {
                NeedsDocument = true,
                Description = "Hide all text (Font.Hidden = 1) everywhere.",
                Run = a => { Word.Modules.Text.GlobalHide(); return null; }
            };
            d["text.global_unhide"] = new ActionDef
            {
                NeedsDocument = true,
                Description = "Unhide all text everywhere.",
                Run = a => { Word.Modules.Text.GlobalUnhide(); return null; }
            };

            d["fonts.replace"] = new ActionDef
            {
                NeedsDocument = true,
                Description = "Replace a font by another in the whole document. Args: source, target.",
                Run = a =>
                {
                    Word.Modules.Fonts.ReplaceDocumentFont(J.Str(a, "source", true), J.Str(a, "target", true));
                    return null;
                }
            };
            d["fonts.set_latin"] = FontSetter("Set the Latin font everywhere. Arg: font.", Word.Modules.Fonts.SetGlobalLatinFont);
            d["fonts.set_asian"] = FontSetter("Set the Asian font everywhere. Arg: font.", Word.Modules.Fonts.SetGlobalAsianFont);
            d["fonts.set_rtl"] = FontSetter("Set the RTL/Thai font everywhere. Arg: font.", Word.Modules.Fonts.SetGlobalRtlThaiFont);
            d["fonts.set_other"] = FontSetter("Set the Cyrillic/other font everywhere. Arg: font.", Word.Modules.Fonts.SetGlobalCyrillicOtherFont);

            d["fonts.document_fonts"] = new ActionDef
            {
                NeedsDocument = true,
                Description = "List fonts used by the active document. Arg real:true lists only fonts actually used in the text.",
                Run = a =>
                {
                    var list = J.Bool(a, "real") ? Word.Modules.Fonts.GetDocumentRealFonts() : Word.Modules.Fonts.GetDocumentFonts();
                    return new JsonObject { ["fonts"] = new JsonArray(list.Select(f => (JsonNode)JsonValue.Create(f)).ToArray()) };
                }
            };

            d["subtitles.convert"] = new ActionDef
            {
                Description = "Pure file conversion SRT<->VTT, no Word. Args: inputPath (.srt/.vtt/.txt), targetExtension (srt|vtt), outputPath (.srt/.vtt).",
                Run = a =>
                {
                    var input = WordTools.ValidateFile(J.Str(a, "inputPath", true), new[] { ".srt", ".vtt", ".txt" }, true, "inputPath");
                    var ext = (J.Str(a, "targetExtension", true) ?? "").TrimStart('.').ToLowerInvariant();
                    if (ext != "srt" && ext != "vtt") throw new ToolException("targetExtension must be srt or vtt.");
                    var output = WordTools.ValidateFile(J.Str(a, "outputPath", true), new[] { ".srt", ".vtt" }, false, "outputPath");
                    var converted = Word.Modules.Subtitles.ConvertFormat(File.ReadAllText(input), ext, input);
                    File.WriteAllText(output, converted, new UTF8Encoding(false));
                    return new JsonObject { ["outputPath"] = output, ["length"] = converted.Length };
                }
            };

            d["images.extract"] = new ActionDef
            {
                Description = "Extract all images from a closed .docx file. Args: docPath, outputFolder (absolute; created if missing).",
                Run = a =>
                {
                    var doc = WordTools.ValidateFile(J.Str(a, "docPath", true), new[] { ".docx" }, true, "docPath");
                    var folder = J.Str(a, "outputFolder", true);
                    if (!Path.IsPathRooted(folder) || folder.IndexOf(':') != 1)
                        throw new ToolException("'outputFolder' must be an absolute local path.");
                    Word.Modules.Images.ExtractImages(doc, Path.GetFullPath(folder));
                    var n = Directory.Exists(folder) ? Directory.GetFiles(folder).Length : 0;
                    return new JsonObject { ["outputFolder"] = Path.GetFullPath(folder), ["files"] = n };
                }
            };

            return d;
        }

        private static ActionDef FontSetter(string description, Action<string> set) => new ActionDef
        {
            NeedsDocument = true,
            Description = description,
            Run = a => { set(J.Str(a, "font", true)); return null; }
        };

        private static Interop.WdColorIndex ParseColor(string name)
        {
            if (string.IsNullOrEmpty(name)) return Interop.WdColorIndex.wdYellow;
            var n = name.StartsWith("wd", StringComparison.OrdinalIgnoreCase) ? name : "wd" + name;
            if (Enum.TryParse<Interop.WdColorIndex>(n, true, out var c)) return c;
            throw new ToolException($"Unknown color '{name}'.");
        }

        public static void Register(ToolRegistry r)
        {
            r.Add("action_list", "List allowed actions for action_run with their arguments.", J.Schema(null), a =>
            {
                var arr = new JsonArray();
                foreach (var kv in Actions.OrderBy(k => k.Key, StringComparer.Ordinal))
                    arr.Add(new JsonObject { ["action"] = kv.Key, ["needsDocument"] = kv.Value.NeedsDocument, ["description"] = kv.Value.Description });
                arr.Add(new JsonObject { ["action"] = "undo (separate tool)", ["needsDocument"] = true, ["description"] = "Use the 'undo' tool." });
                return new JsonObject { ["actions"] = arr };
            });

            r.Add("action_run", "Run an allowed action (see action_list). Message boxes the add-in would show are returned in uiMessages.",
                J.Schema(new[] { "action" }, ("action", "string", "Action name from action_list."),
                    ("args", "object", "Action arguments.")), a =>
                {
                    var name = J.Str(a, "action", true);
                    if (!Actions.TryGetValue(name, out var def)) throw new ToolException($"Unknown action '{name}'. Use action_list.");
                    if (def.NeedsDocument && !Word.Modules.Shared.HasActiveDocument()) throw new ToolException("No document is open.");
                    var args = J.Get(a, "args") as JsonObject ?? new JsonObject();
                    var res = def.Run(args);
                    var o = res as JsonObject ?? new JsonObject();
                    o["action"] = name;
                    o["ok"] = true;
                    return o;
                });

            r.Add("undo", "Undo the last Word operation(s) in the active document.",
                J.Schema(null, ("times", "integer", "How many steps (default 1).")), a =>
                {
                    if (!Word.Modules.Shared.HasActiveDocument()) throw new ToolException("No document is open.");
                    var times = Math.Max(1, Math.Min(J.Int(a, "times", 1), 100));
                    object t = times;
                    var ok = Globals.ThisAddIn.Application.ActiveDocument.Undo(ref t);
                    return new JsonObject { ["undone"] = ok, ["times"] = times };
                });
        }
    }
}
