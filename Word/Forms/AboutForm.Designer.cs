namespace Word.Forms
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.label_Copyright = new System.Windows.Forms.Label();
            this.linkLabel_Site = new System.Windows.Forms.LinkLabel();
            this.label_DonateText = new System.Windows.Forms.Label();
            this.pictureBox_QRMonobank = new System.Windows.Forms.PictureBox();
            this.button_linkMonobank = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_QRMonobank)).BeginInit();
            this.SuspendLayout();
            // 
            // label_Copyright
            // 
            this.label_Copyright.AutoSize = true;
            this.label_Copyright.Location = new System.Drawing.Point(12, 9);
            this.label_Copyright.Name = "label_Copyright";
            this.label_Copyright.Size = new System.Drawing.Size(126, 13);
            this.label_Copyright.TabIndex = 7;
            this.label_Copyright.Text = "Copyright © Andrii Lytvyn\r\n";
            // 
            // linkLabel_Site
            // 
            this.linkLabel_Site.AutoSize = true;
            this.linkLabel_Site.Location = new System.Drawing.Point(12, 63);
            this.linkLabel_Site.Name = "linkLabel_Site";
            this.linkLabel_Site.Size = new System.Drawing.Size(46, 13);
            this.linkLabel_Site.TabIndex = 8;
            this.linkLabel_Site.TabStop = true;
            this.linkLabel_Site.Text = "Website";
            this.linkLabel_Site.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Site_LinkClicked);
            // 
            // label_DonateText
            // 
            this.label_DonateText.BackColor = System.Drawing.Color.Transparent;
            this.label_DonateText.Location = new System.Drawing.Point(12, 32);
            this.label_DonateText.Name = "label_DonateText";
            this.label_DonateText.Size = new System.Drawing.Size(191, 31);
            this.label_DonateText.TabIndex = 9;
            this.label_DonateText.Text = "Thank you for using!\r\nPlease support me with a donation.\r\n\r\n\r\n\r\n";
            // 
            // pictureBox_QRMonobank
            // 
            this.pictureBox_QRMonobank.BackgroundImage = global::Word.Properties.Resources._5370913027089627819;
            this.pictureBox_QRMonobank.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox_QRMonobank.Location = new System.Drawing.Point(209, 9);
            this.pictureBox_QRMonobank.Name = "pictureBox_QRMonobank";
            this.pictureBox_QRMonobank.Size = new System.Drawing.Size(107, 102);
            this.pictureBox_QRMonobank.TabIndex = 4;
            this.pictureBox_QRMonobank.TabStop = false;
            // 
            // button_linkMonobank
            // 
            this.button_linkMonobank.Image = global::Word.Properties.Resources.BMP__24_bit___2__16x16;
            this.button_linkMonobank.Location = new System.Drawing.Point(15, 82);
            this.button_linkMonobank.Name = "button_linkMonobank";
            this.button_linkMonobank.Size = new System.Drawing.Size(188, 29);
            this.button_linkMonobank.TabIndex = 3;
            this.button_linkMonobank.Text = "monobank";
            this.button_linkMonobank.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_linkMonobank.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_linkMonobank.UseVisualStyleBackColor = true;
            this.button_linkMonobank.Click += new System.EventHandler(this.button_linkMonobank_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 120);
            this.Controls.Add(this.label_DonateText);
            this.Controls.Add(this.linkLabel_Site);
            this.Controls.Add(this.label_Copyright);
            this.Controls.Add(this.pictureBox_QRMonobank);
            this.Controls.Add(this.button_linkMonobank);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Spaghetti";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AboutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_QRMonobank)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_linkMonobank;
        private System.Windows.Forms.Label label_Copyright;
        private System.Windows.Forms.LinkLabel linkLabel_Site;
        private System.Windows.Forms.Label label_DonateText;
        private System.Windows.Forms.PictureBox pictureBox_QRMonobank;
    }
}