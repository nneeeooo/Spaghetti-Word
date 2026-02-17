using System;
using System.Windows.Forms;

namespace VideoConverter
{
    public partial class ConverterForm : Form
    {
        public ConverterForm()
        {
            InitializeComponent();
        }

        private void listBox_Files_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void listBox_Files_DragDrop(object sender, DragEventArgs e)
        {
            object[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            listBox_FilesVideo.Items.AddRange(files);
        }

        private void ConverterForm_Load(object sender, EventArgs e)
        {

        }
    }
}
