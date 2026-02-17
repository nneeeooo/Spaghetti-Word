namespace Word
{
    partial class Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabSpaghetti = this.Factory.CreateRibbonTab();
            this.groupUtilities = this.Factory.CreateRibbonGroup();
            this.button2 = this.Factory.CreateRibbonButton();
            this.button_ExtractAllImages = this.Factory.CreateRibbonButton();
            this.button_SubtitleTools = this.Factory.CreateRibbonButton();
            this.button_ColorPicker = this.Factory.CreateRibbonButton();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.button_FontManager = this.Factory.CreateRibbonButton();
            this.button_FontChanger = this.Factory.CreateRibbonButton();
            this.groupDtpAndPrep = this.Factory.CreateRibbonGroup();
            this.buttonHLForm = this.Factory.CreateRibbonButton();
            this.buttonConversionForm = this.Factory.CreateRibbonButton();
            this.button_Shortcuts = this.Factory.CreateRibbonButton();
            this.tabSpaghetti.SuspendLayout();
            this.groupUtilities.SuspendLayout();
            this.group1.SuspendLayout();
            this.groupDtpAndPrep.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSpaghetti
            // 
            this.tabSpaghetti.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabSpaghetti.Groups.Add(this.groupUtilities);
            this.tabSpaghetti.Groups.Add(this.group1);
            this.tabSpaghetti.Groups.Add(this.groupDtpAndPrep);
            this.tabSpaghetti.Label = "Spaghetti";
            this.tabSpaghetti.Name = "tabSpaghetti";
            // 
            // groupUtilities
            // 
            this.groupUtilities.Items.Add(this.button2);
            this.groupUtilities.Items.Add(this.button_ExtractAllImages);
            this.groupUtilities.Items.Add(this.button_SubtitleTools);
            this.groupUtilities.Items.Add(this.button_ColorPicker);
            this.groupUtilities.Label = "Utilities";
            this.groupUtilities.Name = "groupUtilities";
            // 
            // button2
            // 
            this.button2.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button2.Image = global::Word.Properties.Resources.Etherbrian_Webuosities_Crossing1;
            this.button2.Label = "About Spaghetti";
            this.button2.Name = "button2";
            this.button2.ShowImage = true;
            this.button2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button2_Click);
            // 
            // button_ExtractAllImages
            // 
            this.button_ExtractAllImages.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button_ExtractAllImages.Image = global::Word.Properties.Resources.Folder_Transfer_32x32;
            this.button_ExtractAllImages.Label = "Extract All Images";
            this.button_ExtractAllImages.Name = "button_ExtractAllImages";
            this.button_ExtractAllImages.ShowImage = true;
            this.button_ExtractAllImages.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button_ExtractAllImages_Click);
            // 
            // button_SubtitleTools
            // 
            this.button_SubtitleTools.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button_SubtitleTools.Image = global::Word.Properties.Resources.video__1;
            this.button_SubtitleTools.Label = "Subtitle Tools";
            this.button_SubtitleTools.Name = "button_SubtitleTools";
            this.button_SubtitleTools.ShowImage = true;
            this.button_SubtitleTools.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button_SubtitleTools_Click);
            // 
            // button_ColorPicker
            // 
            this.button_ColorPicker.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button_ColorPicker.Image = global::Word.Properties.Resources._32_x_32___BMP__4_bit_;
            this.button_ColorPicker.Label = "Color Picker";
            this.button_ColorPicker.Name = "button_ColorPicker";
            this.button_ColorPicker.ShowImage = true;
            this.button_ColorPicker.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button_ColorPicker_Click);
            // 
            // group1
            // 
            this.group1.Items.Add(this.button_FontManager);
            this.group1.Items.Add(this.button_FontChanger);
            this.group1.Label = "Fonts";
            this.group1.Name = "group1";
            // 
            // button_FontManager
            // 
            this.button_FontManager.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button_FontManager.Image = global::Word.Properties.Resources.font_tt_0;
            this.button_FontManager.Label = "Fonts Report";
            this.button_FontManager.Name = "button_FontManager";
            this.button_FontManager.ShowImage = true;
            this.button_FontManager.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button_FontChecker_Click);
            // 
            // button_FontChanger
            // 
            this.button_FontChanger.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button_FontChanger.Image = global::Word.Properties.Resources.Font_slide_32x32;
            this.button_FontChanger.Label = "Font Changer";
            this.button_FontChanger.Name = "button_FontChanger";
            this.button_FontChanger.ShowImage = true;
            this.button_FontChanger.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button_FontChanger_Click);
            // 
            // groupDtpAndPrep
            // 
            this.groupDtpAndPrep.Items.Add(this.buttonHLForm);
            this.groupDtpAndPrep.Items.Add(this.buttonConversionForm);
            this.groupDtpAndPrep.Items.Add(this.button_Shortcuts);
            this.groupDtpAndPrep.Label = "DTP and Prep";
            this.groupDtpAndPrep.Name = "groupDtpAndPrep";
            // 
            // buttonHLForm
            // 
            this.buttonHLForm.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonHLForm.Image = global::Word.Properties.Resources.Lamp_32x321;
            this.buttonHLForm.Label = "DTP Spotlight";
            this.buttonHLForm.Name = "buttonHLForm";
            this.buttonHLForm.ShowImage = true;
            this.buttonHLForm.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonHLForm_Click);
            // 
            // buttonConversionForm
            // 
            this.buttonConversionForm.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonConversionForm.Image = global::Word.Properties.Resources.abc1_BMP__8_bit___3__32x32;
            this.buttonConversionForm.Label = "Word Prep";
            this.buttonConversionForm.Name = "buttonConversionForm";
            this.buttonConversionForm.ShowImage = true;
            this.buttonConversionForm.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonConversionForm_Click);
            // 
            // button_Shortcuts
            // 
            this.button_Shortcuts.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button_Shortcuts.Image = global::Word.Properties.Resources.Regedit_32x32_0;
            this.button_Shortcuts.Label = "Shortcuts";
            this.button_Shortcuts.Name = "button_Shortcuts";
            this.button_Shortcuts.ShowImage = true;
            this.button_Shortcuts.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button_Shortcuts_Click);
            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tabSpaghetti);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.tabSpaghetti.ResumeLayout(false);
            this.tabSpaghetti.PerformLayout();
            this.groupUtilities.ResumeLayout(false);
            this.groupUtilities.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.groupDtpAndPrep.ResumeLayout(false);
            this.groupDtpAndPrep.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabSpaghetti;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonHLForm;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupUtilities;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonConversionForm;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button_FontManager;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button_ExtractAllImages;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button_SubtitleTools;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button_Shortcuts;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupDtpAndPrep;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button_FontChanger;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button_ColorPicker;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
