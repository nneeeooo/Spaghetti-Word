using System;
using System.Windows.Forms;

namespace Word.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            const int startYear = 2025;
            var currentYear = DateTime.Now.Year;
            var yearDisplay = currentYear > startYear ? $"{startYear}-{currentYear}" : $"{startYear}";

            label_Copyright.Text = $"Copyright © {yearDisplay} Andrii Lytvyn";
            Text = $"About Spaghetti {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";

            linkLabel_Site.Links.Add(0, linkLabel_Site.Text.Length, "https://nneeeooo.pp.ua/");
        }

        private void button_linkMonobank_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://send.monobank.ua/jar/2xVJnpjWMi");
        }

        private void linkLabel_Site_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://nneeeooo.pp.ua");
        }
    }
}
