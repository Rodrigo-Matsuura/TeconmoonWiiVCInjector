using System;
using System.Windows.Forms;
using TeconMoon_s_WiiVC_Injector.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;

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
            using (var outputFolderSelect = new CommonOpenFileDialog("Specify your output folder")
            {
                InitialDirectory = Settings.Default.OutputPath,
                IsFolderPicker = true,
                EnsurePathExists = true
            })
            {
                if (outputFolderSelect.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    OutputDir.Text = outputFolderSelect.FileName;
                }
            }
        }
    }
}
