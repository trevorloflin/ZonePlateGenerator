using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZonePlateGenerator
{
    public partial class Form1 : Form
    {
        private frmProgress wndProgress;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (dlgSaveLocation.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSaveFile.Text = dlgSaveLocation.FileName;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            ZoneGenerator generator = new ZoneGenerator();
            generator.Size = int.Parse(txtWidth.Text);
            generator.Scale = int.Parse(txtScale.Text);

            generator.ProgressUpdate += UpdateProgress;
            using (wndProgress = new frmProgress()) {
                ProgressBar pbProgress = (ProgressBar) wndProgress.Controls[0];
                pbProgress.Maximum = generator.Size * generator.Size;
                wndProgress.Show();

                if (generator.Generate())
                {
                    generator.Result.Save(txtSaveFile.Text, ImageFormat.Png);
                }

                wndProgress.Hide();
            }
        }

        private void UpdateProgress(object sender, int progress)
        {
            ProgressBar pbProgress = (ProgressBar)wndProgress.Controls[0];
            pbProgress.Value = progress + 1;
            pbProgress.Value = progress;
            Application.DoEvents();
        }
    }
}
