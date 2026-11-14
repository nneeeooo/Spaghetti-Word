namespace Word.Forms
{
    partial class SpotlightForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpotlightForm));
            this.labelHL_Dtp = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButtonHL_ColorTurquoise = new System.Windows.Forms.RadioButton();
            this.radioButtonHL_ColorBrightGreen = new System.Windows.Forms.RadioButton();
            this.radioButtonHL_ColorYellow = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.button_Clear = new System.Windows.Forms.Button();
            this.button_JusttifiedText = new System.Windows.Forms.Button();
            this.button_UndelimitedText = new System.Windows.Forms.Button();
            this.button_FastDTP = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHL_Dtp
            // 
            this.labelHL_Dtp.AutoSize = true;
            this.labelHL_Dtp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHL_Dtp.Location = new System.Drawing.Point(12, 9);
            this.labelHL_Dtp.Name = "labelHL_Dtp";
            this.labelHL_Dtp.Size = new System.Drawing.Size(49, 13);
            this.labelHL_Dtp.TabIndex = 25;
            this.labelHL_Dtp.Text = "Presets";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.radioButtonHL_ColorTurquoise);
            this.panel1.Controls.Add(this.radioButtonHL_ColorBrightGreen);
            this.panel1.Controls.Add(this.radioButtonHL_ColorYellow);
            this.panel1.Location = new System.Drawing.Point(230, 89);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(114, 41);
            this.panel1.TabIndex = 33;
            // 
            // radioButtonHL_ColorTurquoise
            // 
            this.radioButtonHL_ColorTurquoise.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonHL_ColorTurquoise.BackColor = System.Drawing.Color.Turquoise;
            this.radioButtonHL_ColorTurquoise.Location = new System.Drawing.Point(79, 6);
            this.radioButtonHL_ColorTurquoise.Name = "radioButtonHL_ColorTurquoise";
            this.radioButtonHL_ColorTurquoise.Size = new System.Drawing.Size(32, 32);
            this.radioButtonHL_ColorTurquoise.TabIndex = 2;
            this.radioButtonHL_ColorTurquoise.TabStop = true;
            this.radioButtonHL_ColorTurquoise.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonHL_ColorTurquoise.UseVisualStyleBackColor = false;
            this.radioButtonHL_ColorTurquoise.CheckedChanged += new System.EventHandler(this.radioButtonHL_ColorTurquoise_CheckedChanged);
            // 
            // radioButtonHL_ColorBrightGreen
            // 
            this.radioButtonHL_ColorBrightGreen.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonHL_ColorBrightGreen.BackColor = System.Drawing.Color.Lime;
            this.radioButtonHL_ColorBrightGreen.Location = new System.Drawing.Point(41, 6);
            this.radioButtonHL_ColorBrightGreen.Name = "radioButtonHL_ColorBrightGreen";
            this.radioButtonHL_ColorBrightGreen.Size = new System.Drawing.Size(32, 32);
            this.radioButtonHL_ColorBrightGreen.TabIndex = 1;
            this.radioButtonHL_ColorBrightGreen.TabStop = true;
            this.radioButtonHL_ColorBrightGreen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonHL_ColorBrightGreen.UseVisualStyleBackColor = false;
            this.radioButtonHL_ColorBrightGreen.CheckedChanged += new System.EventHandler(this.radioButtonHL_ColorBrightGreen_CheckedChanged);
            // 
            // radioButtonHL_ColorYellow
            // 
            this.radioButtonHL_ColorYellow.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonHL_ColorYellow.BackColor = System.Drawing.Color.Yellow;
            this.radioButtonHL_ColorYellow.Location = new System.Drawing.Point(3, 6);
            this.radioButtonHL_ColorYellow.Name = "radioButtonHL_ColorYellow";
            this.radioButtonHL_ColorYellow.Size = new System.Drawing.Size(32, 32);
            this.radioButtonHL_ColorYellow.TabIndex = 0;
            this.radioButtonHL_ColorYellow.TabStop = true;
            this.radioButtonHL_ColorYellow.Text = "✓";
            this.radioButtonHL_ColorYellow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonHL_ColorYellow.UseVisualStyleBackColor = false;
            this.radioButtonHL_ColorYellow.CheckedChanged += new System.EventHandler(this.radioButtonHL_ColorYellow_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label1.Location = new System.Drawing.Point(12, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 26);
            this.label1.TabIndex = 41;
            this.label1.Text = "WARNING! \r\nClear removes ALL highlights.";
            // 
            // button_Clear
            // 
            this.button_Clear.Image = global::Word.Properties.Resources.recycle_bin_empty_cool_4;
            this.button_Clear.Location = new System.Drawing.Point(15, 98);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(74, 32);
            this.button_Clear.TabIndex = 39;
            this.button_Clear.Text = "Clear";
            this.button_Clear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_Clear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // button_JusttifiedText
            // 
            this.button_JusttifiedText.Image = global::Word.Properties.Resources.message_file_1;
            this.button_JusttifiedText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_JusttifiedText.Location = new System.Drawing.Point(149, 25);
            this.button_JusttifiedText.Name = "button_JusttifiedText";
            this.button_JusttifiedText.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_JusttifiedText.Size = new System.Drawing.Size(102, 32);
            this.button_JusttifiedText.TabIndex = 36;
            this.button_JusttifiedText.Text = "Justified text";
            this.button_JusttifiedText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_JusttifiedText.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_JusttifiedText.UseVisualStyleBackColor = true;
            this.button_JusttifiedText.Click += new System.EventHandler(this.button_JusttifiedText_Click);
            // 
            // button_UndelimitedText
            // 
            this.button_UndelimitedText.Image = global::Word.Properties.Resources.unbreak_sm;
            this.button_UndelimitedText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_UndelimitedText.Location = new System.Drawing.Point(15, 25);
            this.button_UndelimitedText.Name = "button_UndelimitedText";
            this.button_UndelimitedText.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_UndelimitedText.Size = new System.Drawing.Size(128, 32);
            this.button_UndelimitedText.TabIndex = 35;
            this.button_UndelimitedText.Text = "Undelimited text";
            this.button_UndelimitedText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_UndelimitedText.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_UndelimitedText.UseVisualStyleBackColor = true;
            this.button_UndelimitedText.Click += new System.EventHandler(this.button_UndelimitedText_Click);
            // 
            // button_FastDTP
            // 
            this.button_FastDTP.Image = global::Word.Properties.Resources.directory_check_31;
            this.button_FastDTP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_FastDTP.Location = new System.Drawing.Point(257, 25);
            this.button_FastDTP.Name = "button_FastDTP";
            this.button_FastDTP.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_FastDTP.Size = new System.Drawing.Size(87, 32);
            this.button_FastDTP.TabIndex = 34;
            this.button_FastDTP.Text = "Fast DTP";
            this.button_FastDTP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_FastDTP.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_FastDTP.UseVisualStyleBackColor = true;
            this.button_FastDTP.Click += new System.EventHandler(this.button_FastDTP_Click);
            // 
            // SpotlightForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(356, 136);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.button_JusttifiedText);
            this.Controls.Add(this.button_UndelimitedText);
            this.Controls.Add(this.button_FastDTP);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelHL_Dtp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SpotlightForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DTP Spotlight";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HLForm_FormClosing);
            this.Load += new System.EventHandler(this.HLForm_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelHL_Dtp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonHL_ColorTurquoise;
        private System.Windows.Forms.RadioButton radioButtonHL_ColorBrightGreen;
        private System.Windows.Forms.RadioButton radioButtonHL_ColorYellow;
        private System.Windows.Forms.Button button_FastDTP;
        private System.Windows.Forms.Button button_UndelimitedText;
        private System.Windows.Forms.Button button_JusttifiedText;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.Label label1;
    }
}