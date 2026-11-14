using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Interop = Microsoft.Office.Interop.Word;

namespace Word.Mcp
{
    /// <summary>Document-level tools (word_status, doc_*).</summary>
    internal static class WordTools
    {
        private static readonly string[] OpenExt = { ".docx", ".doc", ".docm", ".rtf", ".txt", ".srt", ".vtt" };
        private static readonly string[] SaveExt = { ".docx", ".doc", ".docm", ".rtf", ".txt", ".srt", ".vtt" };

        private static Interop.Application App => Globals.ThisAddIn.Application
            ?? throw new ToolException("Word application is not available.");

        internal static string ValidateFile(string path, string[] allowed, bool mustExist, string argName)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ToolException($"'{argName}' is empty.");
            if (!Path.IsPathRooted(path) || path.StartsWith(@"\\") || path.StartsWith("//") || path.IndexOf(':') != 1)
                throw new ToolException($"'{argName}' must be an absolute local path (e.g. C:\\dir\\file.docx).");
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (!allowed.Contains(ext)) throw new ToolException($"'{argName}' extension '{ext}' not allowed. Allowed: {string.Join(", ", allowed)}.");
            if (mustExist && !File.Exists(path)) throw new ToolException($"File not found: {path}");
            if (!mustExist && !Directory.Exists(Path.GetDirectoryName(path)))
                throw new ToolException($"Folder does not exist: {Path.GetDirectoryName(path)}");
            return Path.GetFullPath(path);
        }

        private static Interop.Document Doc(JsonObject a)
        {
            var app = App;
            var name = J.Str(a, "name");
            if (string.IsNullOrEmpty(name))
            {
                if (app.Documents.Count == 0) throw new ToolException("No document is open.");
                return app.ActiveDocument;
            }
            foreach (Interop.Document d in app.Documents)
                if (string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(d.FullName, name, StringComparison.OrdinalIgnoreCase))
                    return d;
            throw new ToolException($"No open document named '{name}'. Use doc_list.");
        }

        private static JsonObject Info(Interop.Document d, string activeName) => new JsonObject
        {
            ["name"] = d.Name,
            ["fullName"] = d.FullName,
            ["saved"] = d.Saved,
            ["readOnly"] = d.ReadOnly,
            ["active"] = d.Name == activeName,
            ["length"] = d.Content.End
        };

        private static string ActiveName(Interop.Application app) => app.Documents.Count > 0 ? app.ActiveDocument.Name : null;

        public static void Register(ToolRegistry r)
        {
            var nameOpt = ("name", "string", "Document name or full path; default is the active document.");

            r.Add("word_status", "Word version, open document count, active document and selection.", J.Schema(null), a =>
            {
                var app = App;
                var o = new JsonObject
                {
                    ["version"] = app.Version,
                    ["visible"] = app.Visible,
                    ["documents"] = app.Documents.Count
                };
                if (app.Documents.Count > 0)
                {
                    var d = app.ActiveDocument;
                    o["activeDocument"] = d.Name;
                    var sel = d.ActiveWindow.Selection;
                    o["selectionStart"] = sel.Start;
                    o["selectionEnd"] = sel.End;
                }
                return o;
            });

            r.Add("doc_list", "List open documents.", J.Schema(null), a =>
            {
                var app = App;
                var act = ActiveName(app);
                var arr = new JsonArray();
                foreach (Interop.Document d in app.Documents) arr.Add(Info(d, act));
                return new JsonObject { ["documents"] = arr };
            });

            r.Add("doc_open", "Open a document by absolute path (.docx/.doc/.docm/.rtf/.txt/.srt/.vtt).",
                J.Schema(new[] { "path" }, ("path", "string", "Absolute path of an existing file."),
                    ("readOnly", "boolean", "Open read-only.")), a =>
                {
                    var p = ValidateFile(J.Str(a, "path", true), OpenExt, true, "path");
                    var app = App;
                    var d = app.Documents.Open(p, ConfirmConversions: false, ReadOnly: J.Bool(a, "readOnly"), AddToRecentFiles: false);
                    return Info(d, ActiveName(app));
                });

            r.Add("doc_new", "Create a new blank document and make it active.", J.Schema(null), a =>
            {
                var app = App;
                var d = app.Documents.Add();
                return Info(d, ActiveName(app));
            });

            r.Add("doc_activate", "Make an open document active.", J.Schema(new[] { "name" }, nameOpt), a =>
            {
                var d = Doc(a);
                d.Activate();
                return Info(d, d.Name);
            });

            r.Add("doc_close", "Close a document, saving or discarding changes.",
                J.Schema(new[] { "save" }, nameOpt, ("save", "boolean", "true saves first (document must already have a path), false discards changes.")), a =>
                {
                    var d = Doc(a);
                    var name = d.Name;
                    var save = J.Bool(a, "save");
                    if (save && string.IsNullOrEmpty(d.Path))
                        throw new ToolException("Document was never saved; use doc_save with a path first, or close with save:false.");
                    d.Close(save ? Interop.WdSaveOptions.wdSaveChanges : Interop.WdSaveOptions.wdDoNotSaveChanges);
                    return new JsonObject { ["closed"] = name, ["saved"] = save };
                });

            r.Add("doc_save", "Save a document; with path, save as (.docx/.doc/.docm/.rtf/.txt/.srt/.vtt).",
                J.Schema(null, nameOpt, ("path", "string", "Absolute target path; omit to save in place.")), a =>
                {
                    var d = Doc(a);
                    var path = J.Str(a, "path");
                    if (string.IsNullOrEmpty(path))
                    {
                        if (string.IsNullOrEmpty(d.Path)) throw new ToolException("Document has no path yet; pass 'path'.");
                        d.Save();
                    }
                    else
                    {
                        var p = ValidateFile(path, SaveExt, false, "path");
                        var fmt = SaveFormat(Path.GetExtension(p).ToLowerInvariant());
                        d.SaveAs2(FileName: p, FileFormat: fmt, Encoding: 65001);
                    }
                    return new JsonObject { ["fullName"] = d.FullName, ["saved"] = d.Saved };
                });

            r.Add("doc_get_text", "Read document text (character offsets, end exclusive).",
                J.Schema(null, nameOpt, ("start", "integer", "Start offset, default 0."), ("end", "integer", "End offset, default end of document."),
                    ("maxChars", "integer", "Truncate to this many characters (default 20000).")), a =>
                {
                    var d = Doc(a);
                    var total = d.Content.End;
                    var start = Math.Max(0, Math.Min(J.IntOpt(a, "start") ?? 0, total));
                    var end = Math.Max(start, Math.Min(J.IntOpt(a, "end") ?? total, total));
                    var max = Math.Max(1, J.Int(a, "maxChars", 20000));
                    var text = d.Range(start, end).Text ?? "";
                    var truncated = text.Length > max;
                    if (truncated) text = text.Substring(0, max);
                    return new JsonObject { ["text"] = text, ["start"] = start, ["end"] = end, ["total"] = total, ["truncated"] = truncated };
                });

            r.Add("doc_insert_text", "Insert text at the start or end of the document, or at the selection start.",
                J.Schema(new[] { "text", "at" }, nameOpt, ("text", "string", "Text to insert."),
                    ("at", "string", "'start', 'end' or 'selection'.")), a =>
                {
                    var d = Doc(a);
                    var text = J.Str(a, "text", true);
                    var at = J.Str(a, "at", true);
                    Interop.Range rng;
                    switch (at)
                    {
                        case "start": rng = d.Range(0, 0); break;
                        case "end":
                            rng = d.Content;
                            rng.MoveEnd(Interop.WdUnits.wdCharacter, -1);
                            rng.Collapse(Interop.WdCollapseDirection.wdCollapseEnd);
                            break;
                        case "selection":
                            rng = d.ActiveWindow.Selection.Range;
                            rng.Collapse(Interop.WdCollapseDirection.wdCollapseStart);
                            break;
                        default: throw new ToolException("'at' must be 'start', 'end' or 'selection'.");
                    }
                    var pos = rng.Start;
                    rng.InsertAfter(text);
                    return new JsonObject { ["insertedAt"] = pos, ["length"] = text.Length, ["total"] = d.Content.End };
                });

            r.Add("doc_select", "Select a character range.",
                J.Schema(new[] { "start", "end" }, nameOpt, ("start", "integer", "Start offset."), ("end", "integer", "End offset.")), a =>
                {
                    var d = Doc(a);
                    var total = d.Content.End;
                    var s = J.Int(a, "start");
                    var e = J.Int(a, "end");
                    if (s < 0 || e < s || e > total) throw new ToolException($"Range must satisfy 0 <= start <= end <= {total}.");
                    d.Range(s, e).Select();
                    return new JsonObject { ["start"] = s, ["end"] = e };
                });

            r.Add("doc_find", "Find text (max 200 matches) and return positions.",
                J.Schema(new[] { "text" }, nameOpt, ("text", "string", "Text or wildcard pattern."),
                    ("wildcards", "boolean", "Use Word wildcards.")), a =>
                {
                    var d = Doc(a);
                    var rng = d.Content;
                    var f = rng.Find;
                    f.ClearFormatting();
                    f.Text = J.Str(a, "text", true);
                    f.Forward = true;
                    f.Wrap = Interop.WdFindWrap.wdFindStop;
                    f.Format = false;
                    f.MatchWildcards = J.Bool(a, "wildcards");
                    var arr = new JsonArray();
                    var truncated = false;
                    var last = -1;
                    while (f.Execute())
                    {
                        if (arr.Count >= 200) { truncated = true; break; }
                        if (rng.Start == last) break; // no progress
                        arr.Add(new JsonObject { ["start"] = rng.Start, ["end"] = rng.End, ["text"] = Clip(rng.Text) });
                        last = rng.Start;
                        rng.Collapse(Interop.WdCollapseDirection.wdCollapseEnd);
                    }
                    return new JsonObject { ["matches"] = arr, ["truncated"] = truncated };
                });
        }

        private static string Clip(string s) => s != null && s.Length > 200 ? s.Substring(0, 200) : s;

        private static Interop.WdSaveFormat SaveFormat(string ext)
        {
            switch (ext)
            {
                case ".docx": return Interop.WdSaveFormat.wdFormatXMLDocument;
                case ".docm": return Interop.WdSaveFormat.wdFormatXMLDocumentMacroEnabled;
                case ".doc": return Interop.WdSaveFormat.wdFormatDocument97;
                case ".rtf": return Interop.WdSaveFormat.wdFormatRTF;
                default: return Interop.WdSaveFormat.wdFormatUnicodeText; // .txt .srt .vtt
            }
        }
    }
}
