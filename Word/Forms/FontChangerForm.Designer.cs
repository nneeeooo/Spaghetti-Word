namespace Word.Forms
{
    partial class FontChangerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontChangerForm));
            this.comboBox_SourceFont = new System.Windows.Forms.ComboBox();
            this.comboBox_TargetFont = new System.Windows.Forms.ComboBox();
            this.comboBox_AsianFonts = new System.Windows.Forms.ComboBox();
            this.comboBox_LatinFonts = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_ComplexScriptFonts = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_CyrillicOtherFonts = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button_SetAsianFont = new System.Windows.Forms.Button();
            this.button_SetLatinFont = new System.Windows.Forms.Button();
            this.button_SetCyrillicOtherFont = new System.Windows.Forms.Button();
            this.button_SetRTLThaiFont = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button_ChangeFontV2 = new System.Windows.Forms.Button();
            this.button_ChangeFont = new System.Windows.Forms.Button();
            this.button_Refresh = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox_SourceFont
            // 
            this.comboBox_SourceFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_SourceFont.FormattingEnabled = true;
            this.comboBox_SourceFont.Location = new System.Drawing.Point(126, 6);
            this.comboBox_SourceFont.Name = "comboBox_SourceFont";
            this.comboBox_SourceFont.Size = new System.Drawing.Size(217, 21);
            this.comboBox_SourceFont.TabIndex = 45;
            // 
            // comboBox_TargetFont
            // 
            this.comboBox_TargetFont.FormattingEnabled = true;
            this.comboBox_TargetFont.Location = new System.Drawing.Point(126, 33);
            this.comboBox_TargetFont.Name = "comboBox_TargetFont";
            this.comboBox_TargetFont.Size = new System.Drawing.Size(217, 21);
            this.comboBox_TargetFont.TabIndex = 46;
            // 
            // comboBox_AsianFonts
            // 
            this.comboBox_AsianFonts.FormattingEnabled = true;
            this.comboBox_AsianFonts.Location = new System.Drawing.Point(126, 33);
            this.comboBox_AsianFonts.Name = "comboBox_AsianFonts";
            this.comboBox_AsianFonts.Size = new System.Drawing.Size(217, 21);
            this.comboBox_AsianFonts.TabIndex = 49;
            // 
            // comboBox_LatinFonts
            // 
            this.comboBox_LatinFonts.FormattingEnabled = true;
            this.comboBox_LatinFonts.Location = new System.Drawing.Point(126, 6);
            this.comboBox_LatinFonts.Name = "comboBox_LatinFonts";
            this.comboBox_LatinFonts.Size = new System.Drawing.Size(217, 21);
            this.comboBox_LatinFonts.TabIndex = 50;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(47, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 52;
            this.label1.Text = "Source font";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(50, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 53;
            this.label2.Text = "Target font";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(56, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 54;
            this.label3.Text = "Asian font";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(59, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 55;
            this.label4.Text = "Latin font";
            // 
            // comboBox_ComplexScriptFonts
            // 
            this.comboBox_ComplexScriptFonts.FormattingEnabled = true;
            this.comboBox_ComplexScriptFonts.Location = new System.Drawing.Point(126, 60);
            this.comboBox_ComplexScriptFonts.Name = "comboBox_ComplexScriptFonts";
            this.comboBox_ComplexScriptFonts.Size = new System.Drawing.Size(217, 21);
            this.comboBox_ComplexScriptFonts.TabIndex = 57;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(23, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 58;
            this.label5.Text = "RTL + Thai font";
            // 
            // comboBox_CyrillicOtherFonts
            // 
            this.comboBox_CyrillicOtherFonts.FormattingEnabled = true;
            this.comboBox_CyrillicOtherFonts.Location = new System.Drawing.Point(126, 87);
            this.comboBox_CyrillicOtherFonts.Name = "comboBox_CyrillicOtherFonts";
            this.comboBox_CyrillicOtherFonts.Size = new System.Drawing.Size(217, 21);
            this.comboBox_CyrillicOtherFonts.TabIndex = 61;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 13);
            this.label6.TabIndex = 62;
            this.label6.Text = "Cyrillic + Other font";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(-4, -2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(357, 230);
            this.tabControl1.TabIndex = 63;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.button_SetAsianFont);
            this.tabPage1.Controls.Add(this.comboBox_CyrillicOtherFonts);
            this.tabPage1.Controls.Add(this.button_SetLatinFont);
            this.tabPage1.Controls.Add(this.button_SetCyrillicOtherFont);
            this.tabPage1.Controls.Add(this.comboBox_AsianFonts);
            this.tabPage1.Controls.Add(this.button_SetRTLThaiFont);
            this.tabPage1.Controls.Add(this.comboBox_LatinFonts);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.comboBox_ComplexScriptFonts);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(349, 204);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Apply subsets";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button_SetAsianFont
            // 
            this.button_SetAsianFont.Image = global::Word.Properties.Resources.font_adobe_1;
            this.button_SetAsianFont.Location = new System.Drawing.Point(253, 114);
            this.button_SetAsianFont.Name = "button_SetAsianFont";
            this.button_SetAsianFont.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_SetAsianFont.Size = new System.Drawing.Size(90, 37);
            this.button_SetAsianFont.TabIndex = 47;
            this.button_SetAsianFont.Text = "Set Asian";
            this.button_SetAsianFont.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_SetAsianFont.UseVisualStyleBackColor = true;
            this.button_SetAsianFont.Click += new System.EventHandler(this.button_SetAsianFont_Click);
            // 
            // button_SetLatinFont
            // 
            this.button_SetLatinFont.Image = global::Word.Properties.Resources.font_tt_1;
            this.button_SetLatinFont.Location = new System.Drawing.Point(157, 114);
            this.button_SetLatinFont.Name = "button_SetLatinFont";
            this.button_SetLatinFont.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_SetLatinFont.Size = new System.Drawing.Size(90, 37);
            this.button_SetLatinFont.TabIndex = 48;
            this.button_SetLatinFont.Text = "Set Latin";
            this.button_SetLatinFont.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_SetLatinFont.UseVisualStyleBackColor = true;
            this.button_SetLatinFont.Click += new System.EventHandler(this.button_SetLatinFont_Click);
            // 
            // button_SetCyrillicOtherFont
            // 
            this.button_SetCyrillicOtherFont.Image = global::Word.Properties.Resources.font_tt_1;
            this.button_SetCyrillicOtherFont.Location = new System.Drawing.Point(16, 157);
            this.button_SetCyrillicOtherFont.Name = "button_SetCyrillicOtherFont";
            this.button_SetCyrillicOtherFont.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_SetCyrillicOtherFont.Size = new System.Drawing.Size(135, 37);
            this.button_SetCyrillicOtherFont.TabIndex = 60;
            this.button_SetCyrillicOtherFont.Text = "Set Cyrillic + Other";
            this.button_SetCyrillicOtherFont.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_SetCyrillicOtherFont.UseVisualStyleBackColor = true;
            this.button_SetCyrillicOtherFont.Click += new System.EventHandler(this.button_SetCyrillicOtherFont_Click);
            // 
            // button_SetRTLThaiFont
            // 
            this.button_SetRTLThaiFont.Image = global::Word.Properties.Resources.font_opentype_11;
            this.button_SetRTLThaiFont.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_SetRTLThaiFont.Location = new System.Drawing.Point(16, 114);
            this.button_SetRTLThaiFont.Name = "button_SetRTLThaiFont";
            this.button_SetRTLThaiFont.Size = new System.Drawing.Size(135, 37);
            this.button_SetRTLThaiFont.TabIndex = 59;
            this.button_SetRTLThaiFont.Text = "Set RTL + Thai";
            this.button_SetRTLThaiFont.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_SetRTLThaiFont.UseVisualStyleBackColor = true;
            this.button_SetRTLThaiFont.Click += new System.EventHandler(this.button_SetRTLThaiFont_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.comboBox_TargetFont);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.comboBox_SourceFont);
            this.tabPage2.Controls.Add(this.button_ChangeFontV2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(349, 204);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Simple replace";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button_ChangeFontV2
            // 
            this.button_ChangeFontV2.Image = global::Word.Properties.Resources.abc1_BMP__8_bit___1__16x16;
            this.button_ChangeFontV2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_ChangeFontV2.Location = new System.Drawing.Point(242, 60);
            this.button_ChangeFontV2.Name = "button_ChangeFontV2";
            this.button_ChangeFontV2.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_ChangeFontV2.Size = new System.Drawing.Size(101, 37);
            this.button_ChangeFontV2.TabIndex = 56;
            this.button_ChangeFontV2.Text = "Change font";
            this.button_ChangeFontV2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_ChangeFontV2.UseVisualStyleBackColor = true;
            this.button_ChangeFontV2.Click += new System.EventHandler(this.button_ChangeFontV2_Click);
            // 
            // button_ChangeFont
            // 
            this.button_ChangeFont.Image = global::Word.Properties.Resources.abc1_BMP__8_bit___1__16x16;
            this.button_ChangeFont.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_ChangeFont.Location = new System.Drawing.Point(551, 12);
            this.button_ChangeFont.Name = "button_ChangeFont";
            this.button_ChangeFont.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_ChangeFont.Size = new System.Drawing.Size(111, 37);
            this.button_ChangeFont.TabIndex = 44;
            this.button_ChangeFont.Text = "Change font v1";
            this.button_ChangeFont.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_ChangeFont.UseVisualStyleBackColor = true;
            this.button_ChangeFont.Visible = false;
            this.button_ChangeFont.Click += new System.EventHandler(this.button_ChangeFont_Click);
            // 
            // button_Refresh
            // 
            this.button_Refresh.Image = global::Word.Properties.Resources.application_hourglass_small_5;
            this.button_Refresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_Refresh.Location = new System.Drawing.Point(9, 230);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_Refresh.Size = new System.Drawing.Size(111, 37);
            this.button_Refresh.TabIndex = 51;
            this.button_Refresh.Text = "Refresh fonts";
            this.button_Refresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_Refresh.UseVisualStyleBackColor = true;
            this.button_Refresh.Click += new System.EventHandler(this.button_Refresh_Click);
            // 
            // FontChangerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 276);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button_ChangeFont);
            this.Controls.Add(this.button_Refresh);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FontChangerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Font Changer";
            this.TopMost = true;
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_ChangeFont;
        private System.Windows.Forms.ComboBox comboBox_SourceFont;
        private System.Windows.Forms.ComboBox comboBox_TargetFont;
        private System.Windows.Forms.Button button_SetAsianFont;
        private System.Windows.Forms.Button button_SetLatinFont;
        private System.Windows.Forms.ComboBox comboBox_AsianFonts;
        private System.Windows.Forms.ComboBox comboBox_LatinFonts;
        private System.Windows.Forms.Button button_Refresh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_ChangeFontV2;
        private System.Windows.Forms.ComboBox comboBox_ComplexScriptFonts;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_SetRTLThaiFont;
        private System.Windows.Forms.Button button_SetCyrillicOtherFont;
        private System.Windows.Forms.ComboBox comboBox_CyrillicOtherFonts;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}