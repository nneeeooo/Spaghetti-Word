using System;
using System.Windows.Forms;
using Word.Helpers;
using Word.Modules;

namespace Word.Forms
{
    public partial class WordPrepForm : Form
    {
        public WordPrepForm()
        {
            InitializeComponent();
        }

        internal void button_Convert_Click(object sender, EventArgs e)
        {
            if (Globals.ThisAddIn.Application == null || Globals.ThisAddIn.Application.ActiveDocument == null) return;

            Modules.Text.Prepare(
                doRemoveSectionBreaks: checkBox_RemoveSectionBreaks.Checked,
                doRemoveColumnBreaks: checkBox_RemoveColumnBreaks.Checked,
                doSetSingleColumn: checkBox_SetSingleColumn.Checked,

                doTrimSpaces: checkBox_TrimSpaces.Checked,
                doFixFontProperties: checkBox_BakeFont.Checked,
                doSetLinespacingSingle: checkBox_LinespacingSingle.Checked,

                doRemoveSpacingBeforeAfter: checkBox_RemoveSpacingBeforeAfter.Checked,
                doRemoveRightIndent: checkBox_RemoveRightIndent.Checked,
                doRemoveLeftIndent: checkBox_RemoveLeftIndent.Checked,
                doRemoveSpecianIndent: checkBox_RemoveSpecialIndent.Checked,

                doRemoveTabstops: checkBox_RemoveTabstops.Checked,
                doRemoveTabs: checkBox_RemoveTabs.Checked,

                doLockImageAspectRatio: checkBox_LockImageAspectRatio.Checked,

                doAutofitTextboxes: checkBox_AutofitTextboxes.Checked,
                doRemoveOptionalHyphens: checkBox_RemoveOptionalHyphens.Checked,
                doDisableAutomaticHyphenation: checkBox_DisableAutomaticHyphenation.Checked,
                doRemoveSoftbreaks: checkBox_RemoveSoftbreaks.Checked
            );


            Tables.Prepare(
                doDoNotResizeToFitContents: checkBox_DoNotResizeToFitContent.Checked,
                doAutofitRowHeight: checkBox_AutofitRowHeight.Checked,
                doAutofitWindow: checkBox_AutofitWindow.Checked,
                doRemoveSpacingBeforeAfter: checkBox_RemoveSpacingBeforeAfter.Checked,
                doDoNotBreakAcrossPages: checkBox_DoNotBreakAcrossPages.Checked,

                doSetZeroMargins: radioButton_ZeroMargins.Checked,
                doSetDefaultMargins: radioButton_DefaultMargins.Checked,

                doResetPaginationSettings: checkBox_ResetPaginationSettings.Checked,
                doRemoveBorders: radioButton_RemoveBorders.Checked,

                doSetCustomMargins: radioButton_SetCustomMargins.Checked,

                customMarginTop: (float)numericUpDown_TopMargin.Value,
                customMarginBottom: (float)numericUpDown_BottomMargin.Value,
                customMarginLeft: (float)numericUpDown_LeftMargin.Value,
                customMarginRight: (float)numericUpDown_RightMargin.Value
            );

            Close();

            MessageBox.Show(
                "Conversion completed successfully!",
                "Conversion",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            Globals.ThisAddIn.Application.Activate();
        }

        public void ConversionForm_Load(object sender, EventArgs e)
        {
            LoadState();
        }

        private void ConversionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveState();
        }

        internal void SaveState()
        {
            const string converterPath = @"Software\Macaroni\Converter";

            RegistryHelper.SaveValue(converterPath, "RemoveSectionBreaks", checkBox_RemoveSectionBreaks.Checked);
            RegistryHelper.SaveValue(converterPath, "RemoveColumnBreaks", checkBox_RemoveColumnBreaks.Checked);
            RegistryHelper.SaveValue(converterPath, "SetSingleColumn", checkBox_SetSingleColumn.Checked);
            RegistryHelper.SaveValue(converterPath, "TrimSpaces", checkBox_TrimSpaces.Checked);
            RegistryHelper.SaveValue(converterPath, "LinespacingSingle", checkBox_LinespacingSingle.Checked);
            RegistryHelper.SaveValue(converterPath, "BakeFont", checkBox_BakeFont.Checked);
            RegistryHelper.SaveValue(converterPath, "AutofitTextboxes", checkBox_AutofitTextboxes.Checked);
            RegistryHelper.SaveValue(converterPath, "RemoveSpacingBeforeAfter", checkBox_RemoveSpacingBeforeAfter.Checked);
            RegistryHelper.SaveValue(converterPath, "RemoveLeftIndent", checkBox_RemoveLeftIndent.Checked);
            RegistryHelper.SaveValue(converterPath, "RemoveRightIndent", checkBox_RemoveRightIndent.Checked);
            RegistryHelper.SaveValue(converterPath, "RemoveSpecialIndent", checkBox_RemoveSpecialIndent.Checked);
            RegistryHelper.SaveValue(converterPath, "RemoveTabstops", checkBox_RemoveTabstops.Checked);
            RegistryHelper.SaveValue(converterPath, "LockImageAspectRatio", checkBox_LockImageAspectRatio.Checked);
            RegistryHelper.SaveValue(converterPath, "DisableAutomaticHyphenation", checkBox_DisableAutomaticHyphenation.Checked);
            RegistryHelper.SaveValue(converterPath, "RemoveOptionalHyphens", checkBox_RemoveOptionalHyphens.Checked);
            RegistryHelper.SaveValue(converterPath, "RemoveTabs", checkBox_RemoveTabs.Checked);
            RegistryHelper.SaveValue(converterPath, "RemoveSoftbreaks", checkBox_RemoveSoftbreaks.Checked);

            const string bakeTablesPath = @"Software\Macaroni\TableConverter";

            // save the checkbox state to the registry
            RegistryHelper.SaveValue(bakeTablesPath, "DoAutofitWindow", checkBox_AutofitWindow.Checked);
            RegistryHelper.SaveValue(bakeTablesPath, "DoAutofitRowHeight", checkBox_AutofitRowHeight.Checked);
            RegistryHelper.SaveValue(bakeTablesPath, "DoDoNotResizeToFitContents", checkBox_DoNotResizeToFitContent.Checked);
            RegistryHelper.SaveValue(bakeTablesPath, "DoSetZeroMargins", radioButton_ZeroMargins.Checked);
            RegistryHelper.SaveValue(bakeTablesPath, "DoSetDefaultMargins", radioButton_DefaultMargins.Checked);
            RegistryHelper.SaveValue(bakeTablesPath, "DoResetPaginationSettings", checkBox_ResetPaginationSettings.Checked);
            RegistryHelper.SaveValue(bakeTablesPath, "DoRemoveBorders", radioButton_RemoveBorders.Checked);
            RegistryHelper.SaveValue(bakeTablesPath, "DoKeepMargins", radioButton_KeepMargins.Checked);
            RegistryHelper.SaveValue(bakeTablesPath, "DoDoNotBreakAcrossPages", checkBox_DoNotBreakAcrossPages.Checked);

            RegistryHelper.SaveValue(bakeTablesPath, "DoSetCustomMargins", radioButton_SetCustomMargins.Checked);
            RegistryHelper.SaveValue(bakeTablesPath, "MarginLeft", (float)numericUpDown_LeftMargin.Value);
            RegistryHelper.SaveValue(bakeTablesPath, "MarginRight", (float)numericUpDown_RightMargin.Value);
            RegistryHelper.SaveValue(bakeTablesPath, "MarginTop", (float)numericUpDown_TopMargin.Value);
            RegistryHelper.SaveValue(bakeTablesPath, "MarginBottom", (float)numericUpDown_BottomMargin.Value);


        }
        internal void LoadState()
        {
            const string converterPath = @"Software\Macaroni\Converter";

            checkBox_RemoveSectionBreaks.Checked = RegistryHelper.LoadValue(converterPath, "RemoveSectionBreaks", true);
            checkBox_RemoveColumnBreaks.Checked = RegistryHelper.LoadValue(converterPath, "RemoveColumnBreaks", true);
            checkBox_SetSingleColumn.Checked = RegistryHelper.LoadValue(converterPath, "SetSingleColumn", true);
            checkBox_TrimSpaces.Checked = RegistryHelper.LoadValue(converterPath, "TrimSpaces", true);
            checkBox_LinespacingSingle.Checked = RegistryHelper.LoadValue(converterPath, "LinespacingSingle", true);
            checkBox_BakeFont.Checked = RegistryHelper.LoadValue(converterPath, "BakeFont", true);
            checkBox_AutofitTextboxes.Checked = RegistryHelper.LoadValue(converterPath, "AutofitTextboxes", true);
            checkBox_RemoveSpacingBeforeAfter.Checked = RegistryHelper.LoadValue(converterPath, "RemoveSpacingBeforeAfter", true);
            checkBox_RemoveLeftIndent.Checked = RegistryHelper.LoadValue(converterPath, "RemoveLeftIndent", true);
            checkBox_RemoveRightIndent.Checked = RegistryHelper.LoadValue(converterPath, "RemoveRightIndent", true);
            checkBox_RemoveSpecialIndent.Checked = RegistryHelper.LoadValue(converterPath, "RemoveSpecialIndent", true);
            checkBox_RemoveTabstops.Checked = RegistryHelper.LoadValue(converterPath, "RemoveTabstops", true);
            checkBox_LockImageAspectRatio.Checked = RegistryHelper.LoadValue(converterPath, "LockImageAspectRatio", true);
            checkBox_DisableAutomaticHyphenation.Checked = RegistryHelper.LoadValue(converterPath, "DisableAutomaticHyphenation", true);
            checkBox_RemoveOptionalHyphens.Checked = RegistryHelper.LoadValue(converterPath, "RemoveOptionalHyphens", true);
            checkBox_RemoveTabs.Checked = RegistryHelper.LoadValue(converterPath, "RemoveTabs", true);
            checkBox_RemoveSoftbreaks.Checked = RegistryHelper.LoadValue(converterPath, "RemoveSoftbreaks", true);

            const string bakeTablesPath = @"Software\Macaroni\TableConverter";

            checkBox_AutofitRowHeight.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoAutofitRowHeight", true);
            checkBox_AutofitWindow.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoAutofitWindow", true);
            checkBox_DoNotResizeToFitContent.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoDoNotResizeToFitContents", true);
            checkBox_RemoveSpacingBeforeAfter.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoRemoveSpacingBeforeAfter", true);
            checkBox_DoNotBreakAcrossPages.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoDoNotBreakAcrossPages", true);

            radioButton_ZeroMargins.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoSetZeroMargins", false);
            radioButton_DefaultMargins.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoSetDefaultMargins", false);
            checkBox_ResetPaginationSettings.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoResetPaginationSettings", true);
            radioButton_RemoveBorders.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoRemoveBorders", false);
            radioButton_KeepMargins.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoKeepMargins", true);

            radioButton_SetCustomMargins.Checked = RegistryHelper.LoadValue(bakeTablesPath, "DoSetCustomMargins", false);
            numericUpDown_LeftMargin.Value = (decimal)RegistryHelper.LoadValue(bakeTablesPath, "MarginLeft", 0f);
            numericUpDown_RightMargin.Value = (decimal)RegistryHelper.LoadValue(bakeTablesPath, "MarginRight", 0f);
            numericUpDown_TopMargin.Value = (decimal)RegistryHelper.LoadValue(bakeTablesPath, "MarginTop", 0f);
            numericUpDown_BottomMargin.Value = (decimal)RegistryHelper.LoadValue(bakeTablesPath, "MarginBottom", 0f);
        }

        private void button_ClearSelection_Click(object sender, EventArgs e)
        {
            checkBox_RemoveSectionBreaks.Checked = false;
            checkBox_RemoveColumnBreaks.Checked = false;
            checkBox_SetSingleColumn.Checked = false;
            checkBox_LinespacingSingle.Checked = false;
            checkBox_BakeFont.Checked = false;
            checkBox_TrimSpaces.Checked = false;
            checkBox_RemoveTabstops.Checked = false;
            checkBox_RemoveTabs.Checked = false;
            checkBox_RemoveSoftbreaks.Checked = false;
            checkBox_AutofitTextboxes.Checked = false;
            checkBox_LockImageAspectRatio.Checked = false;
            checkBox_DisableAutomaticHyphenation.Checked = false;
            checkBox_RemoveOptionalHyphens.Checked = false;

            checkBox_RemoveLeftIndent.Checked = false;
            checkBox_RemoveRightIndent.Checked = false;
            checkBox_RemoveSpecialIndent.Checked = false;
            checkBox_RemoveSpacingBeforeAfter.Checked = false;

            button_DisableTables_Click(sender, e);
        }

        private void button_SelectAll_Click(object sender, EventArgs e)
        {
            checkBox_RemoveSectionBreaks.Checked = true;
            checkBox_RemoveColumnBreaks.Checked = true;
            checkBox_SetSingleColumn.Checked = true;
            checkBox_LinespacingSingle.Checked = true;
            checkBox_BakeFont.Checked = true;
            checkBox_TrimSpaces.Checked = true;
            checkBox_RemoveTabstops.Checked = true;
            checkBox_RemoveTabs.Checked = true;

            checkBox_RemoveLeftIndent.Checked = true;
            checkBox_RemoveRightIndent.Checked = true;
            checkBox_RemoveSpecialIndent.Checked = true;
            checkBox_RemoveSpacingBeforeAfter.Checked = true;

            checkBox_AutofitTextboxes.Checked = true;
            checkBox_LockImageAspectRatio.Checked = true;
            checkBox_DisableAutomaticHyphenation.Checked = true;
            checkBox_RemoveOptionalHyphens.Checked = true;
            checkBox_RemoveSoftbreaks.Checked = true;

            // tables
            checkBox_AutofitWindow.Checked = true;
            checkBox_AutofitRowHeight.Checked = true;
            checkBox_DoNotResizeToFitContent.Checked = true;
            checkBox_ResetPaginationSettings.Checked = true;
            checkBox_DoNotBreakAcrossPages.Checked = true;
            checkBox_RemoveSpacingBeforeAfter.Checked = true;
            checkBox_RemoveSpacingBeforeAfter.Checked = true;
        }

        private void radioButton_SetCustomMargins_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_SetCustomMargins.Checked)
            {
                numericUpDown_TopMargin.Enabled = true;
                numericUpDown_BottomMargin.Enabled = true;
                numericUpDown_LeftMargin.Enabled = true;
                numericUpDown_RightMargin.Enabled = true;
            }
            else
            {
                numericUpDown_TopMargin.Enabled = false;
                numericUpDown_BottomMargin.Enabled = false;
                numericUpDown_LeftMargin.Enabled = false;
                numericUpDown_RightMargin.Enabled = false;
            }
        }

        private void button_DisableTables_Click(object sender, EventArgs e)
        {
            checkBox_AutofitWindow.Checked = false;
            checkBox_AutofitRowHeight.Checked = false;
            checkBox_DoNotResizeToFitContent.Checked = false;
            checkBox_ResetPaginationSettings.Checked = false;
            checkBox_DoNotBreakAcrossPages.Checked = false;

            radioButton_KeepMargins.Checked = true;
            radioButton_KeepBorders.Checked = true;
        }
    }
}
