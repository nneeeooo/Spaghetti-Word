namespace Word.Forms
{
    partial class ColorPickerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPickerForm));
            this.pictureBox_colorPreview = new System.Windows.Forms.PictureBox();
            this.button_ColorPicker = new System.Windows.Forms.Button();
            this.textBox_HexColor = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_colorPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_colorPreview
            // 
            this.pictureBox_colorPreview.BackColor = System.Drawing.Color.White;
            this.pictureBox_colorPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_colorPreview.Location = new System.Drawing.Point(12, 12);
            this.pictureBox_colorPreview.Name = "pictureBox_colorPreview";
            this.pictureBox_colorPreview.Size = new System.Drawing.Size(58, 58);
            this.pictureBox_colorPreview.TabIndex = 49;
            this.pictureBox_colorPreview.TabStop = false;
            // 
            // button_ColorPicker
            // 
            this.button_ColorPicker.Image = global::Word.Properties.Resources.autocad_16x16_01;
            this.button_ColorPicker.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_ColorPicker.Location = new System.Drawing.Point(76, 38);
            this.button_ColorPicker.Name = "button_ColorPicker";
            this.button_ColorPicker.Size = new System.Drawing.Size(114, 32);
            this.button_ColorPicker.TabIndex = 48;
            this.button_ColorPicker.Text = "Pick color";
            this.button_ColorPicker.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_ColorPicker.UseVisualStyleBackColor = true;
            this.button_ColorPicker.Click += new System.EventHandler(this.button_ColorPicker_Click);
            // 
            // textBox_HexColor
            // 
            this.textBox_HexColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_HexColor.Location = new System.Drawing.Point(76, 12);
            this.textBox_HexColor.Name = "textBox_HexColor";
            this.textBox_HexColor.ReadOnly = true;
            this.textBox_HexColor.Size = new System.Drawing.Size(114, 20);
            this.textBox_HexColor.TabIndex = 50;
            this.textBox_HexColor.Text = "#FFFFFF";
            // 
            // ColorPickerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(202, 79);
            this.Controls.Add(this.textBox_HexColor);
            this.Controls.Add(this.pictureBox_colorPreview);
            this.Controls.Add(this.button_ColorPicker);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorPickerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Color Picker";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorPickerForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_colorPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox_colorPreview;
        private System.Windows.Forms.Button button_ColorPicker;
        private System.Windows.Forms.TextBox textBox_HexColor;
    }
}