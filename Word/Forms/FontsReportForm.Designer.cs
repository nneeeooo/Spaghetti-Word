namespace Word.Forms
{
    partial class FontsReportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontsReportForm));
            this.dataGridView_FontTable = new System.Windows.Forms.DataGridView();
            this.button_Refresh = new System.Windows.Forms.Button();
            this.button_CollectFonts = new System.Windows.Forms.Button();
            this.checkBox_CollectDefaultFonts = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_FontTable)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_FontTable
            // 
            this.dataGridView_FontTable.AllowUserToAddRows = false;
            this.dataGridView_FontTable.AllowUserToDeleteRows = false;
            this.dataGridView_FontTable.AllowUserToResizeColumns = false;
            this.dataGridView_FontTable.AllowUserToResizeRows = false;
            this.dataGridView_FontTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_FontTable.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView_FontTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_FontTable.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridView_FontTable.Location = new System.Drawing.Point(12, 35);
            this.dataGridView_FontTable.Name = "dataGridView_FontTable";
            this.dataGridView_FontTable.ReadOnly = true;
            this.dataGridView_FontTable.RowHeadersVisible = false;
            this.dataGridView_FontTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_FontTable.Size = new System.Drawing.Size(309, 343);
            this.dataGridView_FontTable.TabIndex = 0;
            this.dataGridView_FontTable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_FontTable_KeyDown);
            // 
            // button_Refresh
            // 
            this.button_Refresh.Image = global::Word.Properties.Resources.application_hourglass_small_5;
            this.button_Refresh.Location = new System.Drawing.Point(12, 384);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_Refresh.Size = new System.Drawing.Size(105, 32);
            this.button_Refresh.TabIndex = 1;
            this.button_Refresh.Text = "Refresh fonts";
            this.button_Refresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_Refresh.UseVisualStyleBackColor = true;
            this.button_Refresh.Click += new System.EventHandler(this.button_Refresh_Click);
            // 
            // button_CollectFonts
            // 
            this.button_CollectFonts.Image = global::Word.Properties.Resources.directory_fonts_2;
            this.button_CollectFonts.Location = new System.Drawing.Point(213, 384);
            this.button_CollectFonts.Name = "button_CollectFonts";
            this.button_CollectFonts.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.button_CollectFonts.Size = new System.Drawing.Size(108, 32);
            this.button_CollectFonts.TabIndex = 2;
            this.button_CollectFonts.Text = "Collect fonts";
            this.button_CollectFonts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button_CollectFonts.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button_CollectFonts.UseVisualStyleBackColor = true;
            this.button_CollectFonts.Click += new System.EventHandler(this.button_CollectFonts_Click);
            // 
            // checkBox_CollectDefaultFonts
            // 
            this.checkBox_CollectDefaultFonts.AutoSize = true;
            this.checkBox_CollectDefaultFonts.Location = new System.Drawing.Point(12, 12);
            this.checkBox_CollectDefaultFonts.Name = "checkBox_CollectDefaultFonts";
            this.checkBox_CollectDefaultFonts.Size = new System.Drawing.Size(166, 17);
            this.checkBox_CollectDefaultFonts.TabIndex = 3;
            this.checkBox_CollectDefaultFonts.Text = "Collect Windows system fonts";
            this.checkBox_CollectDefaultFonts.UseVisualStyleBackColor = true;
            // 
            // FontsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 426);
            this.Controls.Add(this.checkBox_CollectDefaultFonts);
            this.Controls.Add(this.button_CollectFonts);
            this.Controls.Add(this.button_Refresh);
            this.Controls.Add(this.dataGridView_FontTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FontsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fonts Report";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FontsForm_FormClosing);
            this.Load += new System.EventHandler(this.FontsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_FontTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_FontTable;
        private System.Windows.Forms.Button button_Refresh;
        private System.Windows.Forms.Button button_CollectFonts;
        private System.Windows.Forms.CheckBox checkBox_CollectDefaultFonts;
    }
}