namespace Word.Forms
{
    partial class SubtitleToolsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubtitleToolsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_ApplyStyle = new System.Windows.Forms.Button();
            this.button_RemoveLinebreaks = new System.Windows.Forms.Button();
            this.button_SaveDocx = new System.Windows.Forms.Button();
            this.button_SaveSRT = new System.Windows.Forms.Button();
            this.button_SaveVTT = new System.Windows.Forms.Button();
            this.button_ShowTS = new System.Windows.Forms.Button();
            this.button_RemoveTS = new System.Windows.Forms.Button();
            this.button_HideTS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(151, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Timestamps";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(238, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Convert to";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Utilities";
            // 
            // button_ApplyStyle
            // 
            this.button_ApplyStyle.Image = global::Word.Properties.Resources.Filling_forms_16x16_21;
            this.button_ApplyStyle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_ApplyStyle.Location = new System.Drawing.Point(12, 63);
            this.button_ApplyStyle.Name = "button_ApplyStyle";
            this.button_ApplyStyle.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_ApplyStyle.Size = new System.Drawing.Size(136, 32);
            this.button_ApplyStyle.TabIndex = 10;
            this.button_ApplyStyle.Text = "Apply style";
            this.button_ApplyStyle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_ApplyStyle.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_ApplyStyle.UseVisualStyleBackColor = true;
            this.button_ApplyStyle.Click += new System.EventHandler(this.button_ApplyStyle_Click);
            // 
            // button_RemoveLinebreaks
            // 
            this.button_RemoveLinebreaks.Image = global::Word.Properties.Resources.Filling_forms_16x16_21;
            this.button_RemoveLinebreaks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_RemoveLinebreaks.Location = new System.Drawing.Point(12, 25);
            this.button_RemoveLinebreaks.Name = "button_RemoveLinebreaks";
            this.button_RemoveLinebreaks.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_RemoveLinebreaks.Size = new System.Drawing.Size(136, 32);
            this.button_RemoveLinebreaks.TabIndex = 9;
            this.button_RemoveLinebreaks.Text = "Remove linebreaks";
            this.button_RemoveLinebreaks.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_RemoveLinebreaks.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_RemoveLinebreaks.UseVisualStyleBackColor = true;
            this.button_RemoveLinebreaks.Click += new System.EventHandler(this.button_RemoveLinebreaks_Click);
            // 
            // button_SaveDocx
            // 
            this.button_SaveDocx.Image = global::Word.Properties.Resources.document_1;
            this.button_SaveDocx.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_SaveDocx.Location = new System.Drawing.Point(241, 101);
            this.button_SaveDocx.Name = "button_SaveDocx";
            this.button_SaveDocx.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_SaveDocx.Size = new System.Drawing.Size(81, 32);
            this.button_SaveDocx.TabIndex = 6;
            this.button_SaveDocx.Text = "DOCX";
            this.button_SaveDocx.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_SaveDocx.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_SaveDocx.UseVisualStyleBackColor = true;
            this.button_SaveDocx.Click += new System.EventHandler(this.button_SaveDocx_Click);
            // 
            // button_SaveSRT
            // 
            this.button_SaveSRT.Image = global::Word.Properties.Resources.video_mk_3;
            this.button_SaveSRT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_SaveSRT.Location = new System.Drawing.Point(241, 63);
            this.button_SaveSRT.Name = "button_SaveSRT";
            this.button_SaveSRT.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_SaveSRT.Size = new System.Drawing.Size(81, 32);
            this.button_SaveSRT.TabIndex = 5;
            this.button_SaveSRT.Text = "SRT";
            this.button_SaveSRT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_SaveSRT.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_SaveSRT.UseVisualStyleBackColor = true;
            this.button_SaveSRT.Click += new System.EventHandler(this.button_SaveSRT_Click);
            // 
            // button_SaveVTT
            // 
            this.button_SaveVTT.Image = global::Word.Properties.Resources.Projector_sheets_16x16_1;
            this.button_SaveVTT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_SaveVTT.Location = new System.Drawing.Point(241, 25);
            this.button_SaveVTT.Name = "button_SaveVTT";
            this.button_SaveVTT.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_SaveVTT.Size = new System.Drawing.Size(81, 32);
            this.button_SaveVTT.TabIndex = 4;
            this.button_SaveVTT.Text = "VTT";
            this.button_SaveVTT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_SaveVTT.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_SaveVTT.UseVisualStyleBackColor = true;
            this.button_SaveVTT.Click += new System.EventHandler(this.button_SaveVTT_Click);
            // 
            // button_ShowTS
            // 
            this.button_ShowTS.Image = global::Word.Properties.Resources.directory_check_3;
            this.button_ShowTS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_ShowTS.Location = new System.Drawing.Point(154, 25);
            this.button_ShowTS.Name = "button_ShowTS";
            this.button_ShowTS.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_ShowTS.Size = new System.Drawing.Size(81, 32);
            this.button_ShowTS.TabIndex = 2;
            this.button_ShowTS.Text = "Show";
            this.button_ShowTS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_ShowTS.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_ShowTS.UseVisualStyleBackColor = true;
            this.button_ShowTS.Click += new System.EventHandler(this.button_ShowTS_Click);
            // 
            // button_RemoveTS
            // 
            this.button_RemoveTS.Image = global::Word.Properties.Resources.no2_1;
            this.button_RemoveTS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_RemoveTS.Location = new System.Drawing.Point(154, 101);
            this.button_RemoveTS.Name = "button_RemoveTS";
            this.button_RemoveTS.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_RemoveTS.Size = new System.Drawing.Size(81, 32);
            this.button_RemoveTS.TabIndex = 1;
            this.button_RemoveTS.Text = "Remove";
            this.button_RemoveTS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_RemoveTS.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_RemoveTS.UseVisualStyleBackColor = true;
            this.button_RemoveTS.Click += new System.EventHandler(this.button_RemoveTS_Click);
            // 
            // button_HideTS
            // 
            this.button_HideTS.Image = global::Word.Properties.Resources.Filling_forms_16x16_21;
            this.button_HideTS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_HideTS.Location = new System.Drawing.Point(154, 63);
            this.button_HideTS.Name = "button_HideTS";
            this.button_HideTS.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_HideTS.Size = new System.Drawing.Size(81, 32);
            this.button_HideTS.TabIndex = 0;
            this.button_HideTS.Text = "Hide";
            this.button_HideTS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_HideTS.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_HideTS.UseVisualStyleBackColor = true;
            this.button_HideTS.Click += new System.EventHandler(this.button_HideTS_Click);
            // 
            // SubtitleToolsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 144);
            this.Controls.Add(this.button_ApplyStyle);
            this.Controls.Add(this.button_RemoveLinebreaks);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_SaveDocx);
            this.Controls.Add(this.button_SaveSRT);
            this.Controls.Add(this.button_SaveVTT);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_ShowTS);
            this.Controls.Add(this.button_RemoveTS);
            this.Controls.Add(this.button_HideTS);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SubtitleToolsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Subtitle Tools";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_HideTS;
        private System.Windows.Forms.Button button_RemoveTS;
        private System.Windows.Forms.Button button_ShowTS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_SaveVTT;
        private System.Windows.Forms.Button button_SaveSRT;
        private System.Windows.Forms.Button button_SaveDocx;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_RemoveLinebreaks;
        private System.Windows.Forms.Button button_ApplyStyle;
    }
}