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

        //Load Drives and set drive variable on load
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            BannersRepository.Text = Settings.Default.BannersRepository;
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
            using (var outputFolderSelect = new FolderBrowserDialog())
            {
                outputFolderSelect.Description = "Specify your output folder";
                outputFolderSelect.SelectedPath = Settings.Default.OutputPath;
                outputFolderSelect.ShowNewFolderButton = true;

                if (outputFolderSelect.ShowDialog() == DialogResult.OK)
                {
                    OutputDir.Text = outputFolderSelect.SelectedPath;
                }
            }
        }
    }
}
