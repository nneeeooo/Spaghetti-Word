namespace VideoConverter
{
    partial class ConverterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConverterForm));
            this.listBox_FilesVideo = new System.Windows.Forms.ListBox();
            this.button_Convert = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_Video = new System.Windows.Forms.TabPage();
            this.tabPage_Subtitles = new System.Windows.Forms.TabPage();
            this.radioButton_Cpu = new System.Windows.Forms.RadioButton();
            this.radioButton_Gpu = new System.Windows.Forms.RadioButton();
            this.button_Clear = new System.Windows.Forms.Button();
            this.listBox_FilesSubtitles = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox_AudioCodec = new System.Windows.Forms.ComboBox();
            this.comboBox_VideoCodec = new System.Windows.Forms.ComboBox();
            this.label_AudioCodec = new System.Windows.Forms.Label();
            this.label_VideoCodec = new System.Windows.Forms.Label();
            this.label_Container = new System.Windows.Forms.Label();
            this.comboBox_Container = new System.Windows.Forms.ComboBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage_Video.SuspendLayout();
            this.tabPage_Subtitles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox_FilesVideo
            // 
            this.listBox_FilesVideo.AllowDrop = true;
            this.listBox_FilesVideo.FormattingEnabled = true;
            this.listBox_FilesVideo.Location = new System.Drawing.Point(6, 6);
            this.listBox_FilesVideo.Name = "listBox_FilesVideo";
            this.listBox_FilesVideo.Size = new System.Drawing.Size(1117, 329);
            this.listBox_FilesVideo.TabIndex = 0;
            this.listBox_FilesVideo.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_Files_DragDrop);
            this.listBox_FilesVideo.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox_Files_DragEnter);
            // 
            // button_Convert
            // 
            this.button_Convert.Location = new System.Drawing.Point(1048, 341);
            this.button_Convert.Name = "button_Convert";
            this.button_Convert.Size = new System.Drawing.Size(75, 32);
            this.button_Convert.TabIndex = 1;
            this.button_Convert.Text = "Convert";
            this.button_Convert.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_Video);
            this.tabControl1.Controls.Add(this.tabPage_Subtitles);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1137, 406);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage_Video
            // 
            this.tabPage_Video.Controls.Add(this.label1);
            this.tabPage_Video.Controls.Add(this.numericUpDown1);
            this.tabPage_Video.Controls.Add(this.label_Container);
            this.tabPage_Video.Controls.Add(this.comboBox_Container);
            this.tabPage_Video.Controls.Add(this.label_VideoCodec);
            this.tabPage_Video.Controls.Add(this.label_AudioCodec);
            this.tabPage_Video.Controls.Add(this.comboBox_VideoCodec);
            this.tabPage_Video.Controls.Add(this.comboBox_AudioCodec);
            this.tabPage_Video.Controls.Add(this.button_Clear);
            this.tabPage_Video.Controls.Add(this.radioButton_Gpu);
            this.tabPage_Video.Controls.Add(this.button_Convert);
            this.tabPage_Video.Controls.Add(this.listBox_FilesVideo);
            this.tabPage_Video.Controls.Add(this.radioButton_Cpu);
            this.tabPage_Video.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Video.Name = "tabPage_Video";
            this.tabPage_Video.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Video.Size = new System.Drawing.Size(1129, 380);
            this.tabPage_Video.TabIndex = 0;
            this.tabPage_Video.Text = "Video";
            this.tabPage_Video.UseVisualStyleBackColor = true;
            // 
            // tabPage_Subtitles
            // 
            this.tabPage_Subtitles.Controls.Add(this.button3);
            this.tabPage_Subtitles.Controls.Add(this.button2);
            this.tabPage_Subtitles.Controls.Add(this.button1);
            this.tabPage_Subtitles.Controls.Add(this.listBox_FilesSubtitles);
            this.tabPage_Subtitles.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Subtitles.Name = "tabPage_Subtitles";
            this.tabPage_Subtitles.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Subtitles.Size = new System.Drawing.Size(1129, 380);
            this.tabPage_Subtitles.TabIndex = 1;
            this.tabPage_Subtitles.Text = "Subtitles";
            this.tabPage_Subtitles.UseVisualStyleBackColor = true;
            // 
            // radioButton_Cpu
            // 
            this.radioButton_Cpu.AutoSize = true;
            this.radioButton_Cpu.Location = new System.Drawing.Point(941, 349);
            this.radioButton_Cpu.Name = "radioButton_Cpu";
            this.radioButton_Cpu.Size = new System.Drawing.Size(47, 17);
            this.radioButton_Cpu.TabIndex = 3;
            this.radioButton_Cpu.TabStop = true;
            this.radioButton_Cpu.Text = "CPU";
            this.radioButton_Cpu.UseVisualStyleBackColor = true;
            // 
            // radioButton_Gpu
            // 
            this.radioButton_Gpu.AutoSize = true;
            this.radioButton_Gpu.Location = new System.Drawing.Point(994, 349);
            this.radioButton_Gpu.Name = "radioButton_Gpu";
            this.radioButton_Gpu.Size = new System.Drawing.Size(48, 17);
            this.radioButton_Gpu.TabIndex = 4;
            this.radioButton_Gpu.TabStop = true;
            this.radioButton_Gpu.Text = "GPU";
            this.radioButton_Gpu.UseVisualStyleBackColor = true;
            // 
            // button_Clear
            // 
            this.button_Clear.Location = new System.Drawing.Point(6, 341);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(75, 32);
            this.button_Clear.TabIndex = 5;
            this.button_Clear.Text = "Clear";
            this.button_Clear.UseVisualStyleBackColor = true;
            // 
            // listBox_FilesSubtitles
            // 
            this.listBox_FilesSubtitles.AllowDrop = true;
            this.listBox_FilesSubtitles.FormattingEnabled = true;
            this.listBox_FilesSubtitles.Location = new System.Drawing.Point(6, 6);
            this.listBox_FilesSubtitles.Name = "listBox_FilesSubtitles";
            this.listBox_FilesSubtitles.Size = new System.Drawing.Size(1117, 329);
            this.listBox_FilesSubtitles.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1022, 341);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 32);
            this.button1.TabIndex = 2;
            this.button1.Text = "Convert to SRT";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(915, 341);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 32);
            this.button2.TabIndex = 3;
            this.button2.Text = "Convert to VTT";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(808, 342);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(101, 32);
            this.button3.TabIndex = 4;
            this.button3.Text = "Convert to Word";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // comboBox_AudioCodec
            // 
            this.comboBox_AudioCodec.FormattingEnabled = true;
            this.comboBox_AudioCodec.Items.AddRange(new object[] {
            "None",
            "AAC"});
            this.comboBox_AudioCodec.Location = new System.Drawing.Point(163, 348);
            this.comboBox_AudioCodec.Name = "comboBox_AudioCodec";
            this.comboBox_AudioCodec.Size = new System.Drawing.Size(121, 21);
            this.comboBox_AudioCodec.TabIndex = 6;
            // 
            // comboBox_VideoCodec
            // 
            this.comboBox_VideoCodec.FormattingEnabled = true;
            this.comboBox_VideoCodec.Items.AddRange(new object[] {
            "None",
            "H264"});
            this.comboBox_VideoCodec.Location = new System.Drawing.Point(376, 348);
            this.comboBox_VideoCodec.Name = "comboBox_VideoCodec";
            this.comboBox_VideoCodec.Size = new System.Drawing.Size(121, 21);
            this.comboBox_VideoCodec.TabIndex = 7;
            // 
            // label_AudioCodec
            // 
            this.label_AudioCodec.AutoSize = true;
            this.label_AudioCodec.Location = new System.Drawing.Point(87, 351);
            this.label_AudioCodec.Name = "label_AudioCodec";
            this.label_AudioCodec.Size = new System.Drawing.Size(70, 13);
            this.label_AudioCodec.TabIndex = 8;
            this.label_AudioCodec.Text = "Audio coded:";
            // 
            // label_VideoCodec
            // 
            this.label_VideoCodec.AutoSize = true;
            this.label_VideoCodec.Location = new System.Drawing.Point(300, 351);
            this.label_VideoCodec.Name = "label_VideoCodec";
            this.label_VideoCodec.Size = new System.Drawing.Size(70, 13);
            this.label_VideoCodec.TabIndex = 9;
            this.label_VideoCodec.Text = "Video codec:";
            // 
            // label_Container
            // 
            this.label_Container.AutoSize = true;
            this.label_Container.Location = new System.Drawing.Point(519, 351);
            this.label_Container.Name = "label_Container";
            this.label_Container.Size = new System.Drawing.Size(55, 13);
            this.label_Container.TabIndex = 11;
            this.label_Container.Text = "Container:";
            // 
            // comboBox_Container
            // 
            this.comboBox_Container.FormattingEnabled = true;
            this.comboBox_Container.Items.AddRange(new object[] {
            "None",
            "MP4",
            "MOV",
            "MP3"});
            this.comboBox_Container.Location = new System.Drawing.Point(580, 348);
            this.comboBox_Container.Name = "comboBox_Container";
            this.comboBox_Container.Size = new System.Drawing.Size(121, 21);
            this.comboBox_Container.TabIndex = 10;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(775, 349);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(45, 20);
            this.numericUpDown1.TabIndex = 12;
            this.numericUpDown1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(721, 351);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Scale %:";
            // 
            // ConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1161, 434);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConverterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Video converter";
            this.Load += new System.EventHandler(this.ConverterForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage_Video.ResumeLayout(false);
            this.tabPage_Video.PerformLayout();
            this.tabPage_Subtitles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_FilesVideo;
        private System.Windows.Forms.Button button_Convert;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_Video;
        private System.Windows.Forms.TabPage tabPage_Subtitles;
        private System.Windows.Forms.RadioButton radioButton_Cpu;
        private System.Windows.Forms.RadioButton radioButton_Gpu;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.ListBox listBox_FilesSubtitles;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label_VideoCodec;
        private System.Windows.Forms.Label label_AudioCodec;
        private System.Windows.Forms.ComboBox comboBox_VideoCodec;
        private System.Windows.Forms.ComboBox comboBox_AudioCodec;
        private System.Windows.Forms.Label label_Container;
        private System.Windows.Forms.ComboBox comboBox_Container;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
    }
}

