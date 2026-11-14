using System;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using Word.Helpers;
using Word.Modules;

namespace Word.Forms
{
    public partial class SpotlightForm : Form
    {
        public SpotlightForm()
        {
            InitializeComponent();
        }

        public void HLForm_Load(object sender, EventArgs e)
        {
            LoadState();
        }

        public void HLForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveState();
        }

        /// <summary>
        /// Load form state from the registry.
        /// </summary>
        internal void LoadState()
        {
            const string highlighterPath = @"Software\Macaroni";

            radioButtonHL_ColorBrightGreen.Checked = RegistryHelper.LoadValue(highlighterPath, "ColorBrightGreen", true);
            radioButtonHL_ColorTurquoise.Checked = RegistryHelper.LoadValue(highlighterPath, "ColorTurquoise", false);
            radioButtonHL_ColorYellow.Checked = RegistryHelper.LoadValue(highlighterPath, "ColorYellow", false);

            //textBox_CustomWildcard.Text = RegistryHelper.LoadValue(highlighterPath, "CustomWildcard", String.Empty);
        }

        /// <summary>
        /// Saves the current state of the form to the registry.
        /// </summary>
        internal void SaveState()
        {
            const string highlighterPath = @"Software\Macaroni";

            // custom wildcard
            //RegistryHelper.SaveValue(highlighterPath, "CustomWildcard", textBox_CustomWildcard.Text);

            // color
            RegistryHelper.SaveValue(highlighterPath, "ColorBrightGreen", radioButtonHL_ColorBrightGreen.Checked);
            RegistryHelper.SaveValue(highlighterPath, "ColorTurquoise", radioButtonHL_ColorTurquoise.Checked);
            RegistryHelper.SaveValue(highlighterPath, "ColorYellow", radioButtonHL_ColorYellow.Checked);
        }

        private WdColorIndex GetColor()
        {
            WdColorIndex color;

            if (radioButtonHL_ColorBrightGreen.Checked) color = WdColorIndex.wdBrightGreen;
            else if (radioButtonHL_ColorTurquoise.Checked) color = WdColorIndex.wdTurquoise;
            else color = WdColorIndex.wdYellow;

            return color;
        }

        private void radioButtonHL_ColorYellow_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonHL_ColorYellow.Checked)
            {
                radioButtonHL_ColorYellow.Text = "✓";
                radioButtonHL_ColorTurquoise.Text = "";
                radioButtonHL_ColorBrightGreen.Text = "";
            }

            SaveState();

            Globals.ThisAddIn.Application.Activate();
        }

        private void radioButtonHL_ColorBrightGreen_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonHL_ColorBrightGreen.Checked)
            {
                radioButtonHL_ColorBrightGreen.Text = "✓";
                radioButtonHL_ColorTurquoise.Text = "";
                radioButtonHL_ColorYellow.Text = "";
            }

            SaveState();

            Globals.ThisAddIn.Application.Activate();
        }

        private void radioButtonHL_ColorTurquoise_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonHL_ColorTurquoise.Checked)
            {
                radioButtonHL_ColorTurquoise.Text = "✓";
                radioButtonHL_ColorBrightGreen.Text = "";
                radioButtonHL_ColorYellow.Text = "";
            }

            SaveState();

            Globals.ThisAddIn.Application.Activate();
        }

        private void button_FastDTP_Click(object sender, EventArgs e)
        {
            Spotlight.Clear();

            Spotlight.Run(
                doFastDtp: true,
                color: GetColor()
            );

            Globals.ThisAddIn.Application.Activate();
        }

        private void button_UndelimitedText_Click(object sender, EventArgs e)
        {
            Spotlight.Clear();

            Spotlight.Run(
                doUndelimitedText: true,
                color: GetColor()
            );

            Globals.ThisAddIn.Application.Activate();
        }

        private void button_JusttifiedText_Click(object sender, EventArgs e)
        {
            Spotlight.Clear();

            Spotlight.Run(
                doJustifiedText: true,
                color: GetColor()
            );

            Globals.ThisAddIn.Application.Activate();
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            Spotlight.Clear();

            Globals.ThisAddIn.Application.Activate();
        }
    }
}
