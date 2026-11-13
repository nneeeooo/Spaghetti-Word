using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Windows.Forms;

namespace Word.Mcp
{
    /// <summary>
    /// Fixed name to form map. The add-in registers its forms (spotlight, wordprep, ...) at ribbon load;
    /// a provider returns the live instance, optionally creating/showing it.
    /// </summary>
    internal sealed class FormRegistry
    {
        internal static readonly FormRegistry Instance = new FormRegistry();

        private readonly Dictionary<string, Func<bool, Form>> _providers =
            new Dictionary<string, Func<bool, Form>>(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _names = new List<string>();

        public IReadOnlyList<string> Names => _names;

        public void Register(string name, Func<bool, Form> provider)
        {
            if (!_providers.ContainsKey(name)) _names.Add(name);
            _providers[name] = provider;
        }

        /// <summary>Live form or null. With <paramref name="open"/> the provider creates and shows it.</summary>
        public Form Get(string name, bool open)
        {
            if (name == null || !_providers.TryGetValue(name, out var p))
                throw new ToolException($"Unknown form '{name}'. Known: {string.Join(", ", _names)}.");
            var f = p(open);
            return f == null || f.IsDisposed ? null : f;
        }
    }

    /// <summary>Describes WinForms controls as JSON (read-only walk; no reflection by name).</summary>
    internal static class ControlInspector
    {
        private const int Max = 200;

        public static IEnumerable<Control> Walk(Control root)
        {
            yield return root;
            foreach (Control c in root.Controls)
                foreach (var d in Walk(c))
                    yield return d;
        }

        public static JsonObject Describe(Control c)
        {
            var text = c.Text ?? "";
            var o = new JsonObject
            {
                ["name"] = c.Name,
                ["type"] = c.GetType().Name,
                ["text"] = text.Length > 2000 ? text.Substring(0, 2000) : text,
                ["enabled"] = c.Enabled,
                ["visible"] = c.Visible
            };

            if (c is CheckBox cb) o["checked"] = cb.Checked;
            else if (c is RadioButton rb) o["checked"] = rb.Checked;

            if (c is NumericUpDown n)
            {
                o["value"] = JsonValue.Create(n.Value);
                o["minimum"] = JsonValue.Create(n.Minimum);
                o["maximum"] = JsonValue.Create(n.Maximum);
            }

            if (c is ComboBox combo)
            {
                o["selectedItem"] = combo.SelectedItem == null ? null : combo.GetItemText(combo.SelectedItem);
                o["items"] = Items(combo.Items.Cast<object>().Select(combo.GetItemText), combo.Items.Count, o);
            }
            else if (c is ListBox lb)
            {
                o["selectedItem"] = lb.SelectedItem == null ? null : lb.GetItemText(lb.SelectedItem);
                o["items"] = Items(lb.Items.Cast<object>().Select(lb.GetItemText), lb.Items.Count, o);
            }

            if (c is DataGridView grid)
            {
                o["columns"] = new JsonArray(grid.Columns.Cast<DataGridViewColumn>()
                    .Select(col => (JsonNode)JsonValue.Create(col.Name)).ToArray());
                var rows = new JsonArray();
                foreach (DataGridViewRow r in grid.Rows)
                {
                    if (r.IsNewRow) continue;
                    if (rows.Count >= Max) break;
                    rows.Add(new JsonArray(r.Cells.Cast<DataGridViewCell>()
                        .Select(cell => (JsonNode)JsonValue.Create(Convert.ToString(cell.Value))).ToArray()));
                }
                o["rowCount"] = grid.Rows.Count;
                o["rows"] = rows;
            }
            return o;
        }

        private static JsonArray Items(IEnumerable<string> items, int total, JsonObject owner)
        {
            if (total > Max) owner["itemsTruncated"] = true;
            return new JsonArray(items.Take(Max).Select(s => (JsonNode)JsonValue.Create(s)).ToArray());
        }
    }

    internal static class FormTools
    {
        public static void Register(ToolRegistry r, FormRegistry forms)
        {
            var formArg = ("form", "string", "Form name: " + string.Join(", ", new[] {
                "spotlight", "wordprep", "fontsreport", "fontchanger", "subtitles", "shortcuts", "colorpicker", "about" }));

            r.Add("form_list", "List registered add-in forms and whether each is open/visible.",
                J.Schema(null), a =>
                {
                    var arr = new JsonArray();
                    foreach (var n in forms.Names)
                    {
                        var f = forms.Get(n, false);
                        arr.Add(new JsonObject { ["name"] = n, ["open"] = f != null, ["visible"] = f != null && f.Visible });
                    }
                    return new JsonObject { ["forms"] = arr };
                });

            r.Add("form_open", "Open (show) an add-in form. Needs an open Word document.",
                J.Schema(new[] { "form" }, formArg), a =>
                {
                    var f = forms.Get(J.Str(a, "form", true), true);
                    if (f == null) throw new ToolException("Form could not be opened (is a Word document open?).");
                    return new JsonObject { ["open"] = true, ["visible"] = f.Visible, ["title"] = f.Text };
                });

            r.Add("form_close", "Close an add-in form.",
                J.Schema(new[] { "form" }, formArg), a =>
                {
                    var f = forms.Get(J.Str(a, "form", true), false);
                    if (f == null) return new JsonObject { ["closed"] = false, ["note"] = "not open" };
                    f.Close();
                    return new JsonObject { ["closed"] = true };
                });

            r.Add("form_controls", "List every control of an open form (recursive): name, type, text, enabled, visible, checked, value, selectedItem, items, rows.",
                J.Schema(new[] { "form" }, formArg), a =>
                {
                    var f = Open(forms, a);
                    var arr = new JsonArray();
                    foreach (var c in ControlInspector.Walk(f).Skip(1)) arr.Add(ControlInspector.Describe(c));
                    return new JsonObject { ["form"] = J.Str(a, "form"), ["title"] = f.Text, ["controls"] = arr };
                });

            var ctl = ("control", "string", "Control name (as in form_controls).");

            r.Add("form_get", "Describe one control of an open form.",
                J.Schema(new[] { "form", "control" }, formArg, ctl), a =>
                    ControlInspector.Describe(Find(Open(forms, a), a)));

            r.Add("form_set", "Set a control value: text for text boxes, bool for check/radio, number for numeric up-down, item text for combo boxes.",
                J.Schema(new[] { "form", "control", "value" }, formArg, ctl,
                    ("value", "string|number|boolean", "New value.")), a =>
                {
                    var c = Find(Open(forms, a), a);
                    RequireEnabled(c);
                    var v = J.Get(a, "value") ?? throw new ToolException("Missing required argument 'value'.");
                    try
                    {
                        if (c is CheckBox cb) cb.Checked = v.GetValue<bool>();
                        else if (c is RadioButton rb) rb.Checked = v.GetValue<bool>();
                        else if (c is NumericUpDown n) n.Value = v.GetValue<decimal>();
                        else if (c is ComboBox combo)
                        {
                            var s = v.ToString();
                            var item = combo.Items.Cast<object>().FirstOrDefault(i =>
                                string.Equals(combo.GetItemText(i), s, StringComparison.OrdinalIgnoreCase));
                            if (item != null) combo.SelectedItem = item;
                            else if (combo.DropDownStyle != ComboBoxStyle.DropDownList) combo.Text = s;
                            else throw new ToolException($"Item '{s}' not in the combo box.");
                        }
                        else if (c is TextBoxBase || c is Label) c.Text = v.ToString();
                        else throw new ToolException($"Control type {c.GetType().Name} cannot be set.");
                    }
                    catch (FormatException) { throw new ToolException("Value has the wrong type for this control."); }
                    catch (InvalidOperationException) { throw new ToolException("Value has the wrong type for this control."); }
                    catch (ArgumentOutOfRangeException ex) { throw new ToolException(ex.Message); }
                    return ControlInspector.Describe(c);
                });

            r.Add("form_click", "Click a control (Button.PerformClick; other controls get a Click event). Message boxes are returned in uiMessages.",
                J.Schema(new[] { "form", "control" }, formArg, ctl), a =>
                {
                    var c = Find(Open(forms, a), a);
                    RequireEnabled(c);
                    if (c is Button b) b.PerformClick();
                    else
                    {
                        // One fixed protected method of Control, not a lookup over arbitrary names.
                        typeof(Control).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic)
                            .Invoke(c, new object[] { EventArgs.Empty });
                    }
                    return new JsonObject { ["clicked"] = c.Name };
                });
        }

        private static Form Open(FormRegistry forms, JsonObject a)
        {
            var name = J.Str(a, "form", true);
            return forms.Get(name, false) ?? throw new ToolException($"Form '{name}' is not open. Use form_open first.");
        }

        private static Control Find(Form f, JsonObject a)
        {
            var name = J.Str(a, "control", true);
            return ControlInspector.Walk(f).Skip(1).FirstOrDefault(c => c.Name == name)
                   ?? throw new ToolException($"Control '{name}' not found on the form.");
        }

        private static void RequireEnabled(Control c)
        {
            if (!c.Enabled) throw new ToolException($"Control '{c.Name}' is disabled.");
        }
    }
}
