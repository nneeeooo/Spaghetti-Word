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
            this.linkLabel_Site.Location = new System.Drawing.Point(12, 32);
            this.linkLabel_Site.Name = "linkLabel_Site";
            this.linkLabel_Site.Size = new System.Drawing.Size(46, 13);
            this.linkLabel_Site.TabIndex = 8;
            this.linkLabel_Site.TabStop = true;
            this.linkLabel_Site.Text = "Website";
            this.linkLabel_Site.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Site_LinkClicked);
            // 
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 56);
            this.Controls.Add(this.linkLabel_Site);
            this.Controls.Add(this.label_Copyright);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Spaghetti";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_Copyright;
        private System.Windows.Forms.LinkLabel linkLabel_Site;
    }
}