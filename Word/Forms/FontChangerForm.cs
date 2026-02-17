using System;
using System.Linq;
using System.Windows.Forms;
using Word.Modules;

namespace Word.Forms
{
    public partial class FontChangerForm : Form
    {
        public FontChangerForm()
        {
            InitializeComponent();
            RefreshForm();
        }
        private void RefreshForm()
        {
            // Make sure all combo boxes are updated safely
            comboBox_SourceFont.BeginUpdate();
            comboBox_TargetFont.BeginUpdate();
            comboBox_AsianFonts.BeginUpdate();
            comboBox_LatinFonts.BeginUpdate();
            comboBox_ComplexScriptFonts.BeginUpdate();

            try
            {
                comboBox_SourceFont.Items.Clear();
                comboBox_TargetFont.Items.Clear();
                comboBox_AsianFonts.Items.Clear();
                comboBox_LatinFonts.Items.Clear();
                comboBox_ComplexScriptFonts.Items.Clear();
                comboBox_CyrillicOtherFonts.Items.Clear();

                // Always scan the document dynamically
                var docFonts = Fonts.GetDocumentRealFonts()
                                    .Select(f => f.TrimStart('@'))
                                    .OrderBy(f => f, StringComparer.CurrentCultureIgnoreCase)
                                    .ToArray();

                comboBox_SourceFont.Items.AddRange(docFonts);

                var sysFonts = Fonts.GetInstalledFonts()
                                    .OrderBy(f => f, StringComparer.CurrentCultureIgnoreCase)
                                    .ToArray();

                comboBox_TargetFont.Items.AddRange(sysFonts);
                comboBox_AsianFonts.Items.AddRange(sysFonts);
                comboBox_LatinFonts.Items.AddRange(sysFonts);
                comboBox_ComplexScriptFonts.Items.AddRange(sysFonts);
                comboBox_CyrillicOtherFonts.Items.AddRange(sysFonts);

                // Select the first source font if nothing is selected
                comboBox_SourceFont.SelectedIndex = comboBox_SourceFont.Items.Count > 0 ? 0 : -1;

                // Keep target font synced if possible
                comboBox_TargetFont.SelectedItem = comboBox_SourceFont.SelectedItem;
            }
            finally
            {
                comboBox_SourceFont.EndUpdate();
                comboBox_TargetFont.EndUpdate();
                comboBox_AsianFonts.EndUpdate();
                comboBox_LatinFonts.EndUpdate();
                comboBox_ComplexScriptFonts.EndUpdate();
                comboBox_CyrillicOtherFonts.EndUpdate();
            }
        }

        private void button_ChangeFont_Click(object sender, EventArgs e)
        {

            if (comboBox_SourceFont.SelectedItem == null || comboBox_TargetFont.SelectedItem == null)
                return;

            var previousTarget = comboBox_TargetFont.SelectedItem.ToString();

            Fonts.ReplaceDocumentFont(
                comboBox_SourceFont.SelectedItem.ToString(),
                previousTarget);

            RefreshForm();

            // Select the previous target in the source combo
            if (comboBox_SourceFont.Items.Contains(previousTarget))
                comboBox_SourceFont.SelectedItem = previousTarget;
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            RefreshForm();
        }

        private void button_ChangeFontV2_Click(object sender, EventArgs e)
        {
            if (comboBox_SourceFont.SelectedItem == null || comboBox_TargetFont.SelectedItem == null)
                return;

            var previousTarget = comboBox_TargetFont.SelectedItem.ToString();

            Fonts.ReplaceDocumentFont(
                comboBox_SourceFont.SelectedItem.ToString(),
                previousTarget);

            RefreshForm();

            // Select the previous target in the source combo
            if (comboBox_SourceFont.Items.Contains(previousTarget))
                comboBox_SourceFont.SelectedItem = previousTarget;
        }

        private void button_SetRTLThaiFont_Click(object sender, EventArgs e)
        {
            ApplyFromCombo(comboBox_ComplexScriptFonts, Fonts.SetGlobalRtlThaiFont);
        }

        private void button_SetCyrillicOtherFont_Click(object sender, EventArgs e)
        {
            ApplyFromCombo(comboBox_CyrillicOtherFonts, Fonts.SetGlobalCyrillicOtherFont);
        }

        private void button_SetAsianFont_Click(object sender, EventArgs e)
        {
            ApplyFromCombo(comboBox_AsianFonts, Fonts.SetGlobalAsianFont, true);
        }

        private void button_SetLatinFont_Click(object sender, EventArgs e)
        {
            ApplyFromCombo(comboBox_LatinFonts, Fonts.SetGlobalLatinFont, true);
        }

        private void ApplyFromCombo(ComboBox combo, Action<string> action, bool refresh = false)
        {
            if (combo.SelectedItem == null)
                return;

            action(combo.SelectedItem.ToString());

            if (refresh)
                RefreshForm();
        }

    }
}
