using System;
using System.Windows.Forms;
using TeconMoon_s_WiiVC_Injector.Properties;


namespace TeconMoon_s_WiiVC_Injector
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        // Load current settings into the form
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            BannersRepository.Text = Settings.Default.BannersRepository;
            OutputDir.Text = Settings.Default.OutputPathFixed ?? string.Empty;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Settings.Default.BannersRepository = BannersRepository.Text;
            Settings.Default.OutputPathFixed = OutputDir.Text;
            Settings.Default.Save();
            Close();
        }

        private void OutputDirButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Specify your output folder";
                fbd.SelectedPath = Settings.Default.OutputPath;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    OutputDir.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
