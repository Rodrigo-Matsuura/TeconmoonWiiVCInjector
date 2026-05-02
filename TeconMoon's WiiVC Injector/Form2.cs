using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;


namespace TeconMoon_s_WiiVC_Injector
{
    public partial class SDCardMenu : Form
    {
        public SDCardMenu()
        {
            InitializeComponent();
        }
        string SelectedDriveLetter;
        bool DriveSpecified;

        //Load Drives and set drive variable on load
        private void SDCardMenu_Load(object sender, EventArgs e)
        {
            ReloadDriveList();
            SpecifyDrive();
            MemcardBlocks.SelectedIndex = 0;
            VideoForceMode.SelectedIndex = 0;
            VideoTypeMode.SelectedIndex = 0;
            LanguageBox.SelectedIndex = 0;
            wiiUGamepadSlotBox.SelectedIndex = 0;
            NintendontOptions.SetItemChecked(0, true);
            NintendontOptions.SetItemChecked(7, true);
        }

        //Callable voids for commands
        private void SpecifyDrive()
        {
            SelectedDriveLetter = DriveBox.SelectedValue?.ToString().Substring(0, 3) ?? string.Empty;
            DriveSpecified = !string.IsNullOrEmpty(SelectedDriveLetter);
        }

        private void ReloadDriveList()
        {
            DriveBox.DataSource = DriveInfo.GetDrives()
                .Where(d => d.IsReady && d.DriveType == DriveType.Removable)
                .Select(d => d.Name + " (" + d.VolumeLabel + ")")
                .ToList();
        }

        private void CheckForBoxes()
        {
            bool memcardEnabled = NintendontOptions.GetItemChecked(0);
            MemcardText.Enabled = memcardEnabled;
            MemcardBlocks.Enabled = memcardEnabled;
            MemcardMulti.Enabled = memcardEnabled;

            bool autoVideo = NintendontOptions.GetItemChecked(7);
            VideoWidth.Enabled = !autoVideo;
            VideoWidthText.Enabled = !autoVideo;
            WidthNumber.Text = autoVideo ? "Auto" : VideoWidth.Value.ToString();
        }

        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        //Reload Drives when selected
        private void ReloadDrives_Click(object sender, EventArgs e)
        {
            ReloadDriveList();
            SpecifyDrive();
        }
        //Specify Drive variable when a drive is selected
        private void DriveBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SpecifyDrive();
        }
        

        //Changing config options
        public void NintendontOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckForBoxes();
        }
        private void NintendontOptions_DoubleClick(object sender, EventArgs e)
        {
            CheckForBoxes();
        }
        private void VideoForceMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (VideoForceMode.SelectedIndex == 0)
            {
                VideoTypeMode.SelectedIndex = 0;
                VideoTypeMode.Enabled = false;
            }
            else if (VideoForceMode.SelectedIndex == 3)
            {
                VideoTypeMode.SelectedIndex = 0;
                VideoTypeMode.Enabled = false;
            }
            else
            {
                VideoTypeMode.SelectedIndex = 1;
                VideoTypeMode.Enabled = true;
            }
        }
        private void VideoWidth_Scroll(object sender, EventArgs e)
        {
            WidthNumber.Text = VideoWidth.Value.ToString();
        }
        private void VideoTypeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (VideoForceMode.SelectedIndex != 0 && VideoForceMode.SelectedIndex != 3 && VideoTypeMode.SelectedIndex == 0)
            {
                VideoTypeMode.SelectedIndex = 1;
            }
        }

        //Buttons that make changes to SD Card
        private void NintendontUpdate_Click(object sender, EventArgs e)
        {
            string downloadPath = Path.Combine(Path.GetTempPath(), "WiiVCInjector", "SOURCETEMP", "Download");
            string tempPath = Path.Combine(downloadPath, "apps", "nintendont");
            string sdPath = Path.Combine(SelectedDriveLetter, "apps", "nintendont");

            if (!CheckForInternetConnection())
            {
                DialogResult dialogResult = CustomMessageBox.ShowYesNo(this, "Your internet connection could not be verified, do you wish to try and download Nintendont anyway?",
                    "Internet Connection Verification Failed",
                    MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }

            ActionStatus.Text = "Downloading...";
            ActionStatus.Refresh();
            Directory.CreateDirectory(tempPath);
            using (var client = new WebClient())
            {
                client.DownloadFile("https://raw.githubusercontent.com/FIX94/Nintendont/master/loader/loader.dol", Path.Combine(tempPath, "boot.dol"));
                client.DownloadFile("https://raw.githubusercontent.com/FIX94/Nintendont/master/nintendont/meta.xml", Path.Combine(tempPath, "meta.xml"));
                client.DownloadFile("https://raw.githubusercontent.com/FIX94/Nintendont/master/nintendont/icon.png", Path.Combine(tempPath, "icon.png"));
            }
            ActionStatus.Text = string.Empty;

            if (DriveSpecified)
            {
                if (Directory.Exists(sdPath))
                {
                    Directory.Delete(sdPath, true);
                }
                Directory.CreateDirectory(sdPath);

                var dir = new DirectoryInfo(tempPath);
                foreach (FileInfo file in dir.GetFiles())
                {
                    string outPath = Path.Combine(sdPath, file.Name);
                    file.CopyTo(outPath, true);
                }

                MessageBox.Show("Download complete.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
            }
            else // if no removable drive is specified
            {
                DialogResult dialogResult = CustomMessageBox.ShowYesNo(this, "SD Card not specified.\nDo you wish to save Nintendont somewhere else?",
                    "Drive not specified",
                    MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes) // if YES, ask where to save the file
                {
                    DateTime dateTime = DateTime.UtcNow.Date;

                    using (var saveFileDialog = new SaveFileDialog
                    {
                        Title = "Save Nintendont zip file",
                        CheckPathExists = true,
                        DefaultExt = "zip",
                        Filter = "Zip Files (*.zip)|*.zip",
                        FilterIndex = 2,
                        RestoreDirectory = true,
                        FileName = $"Nintendont-{dateTime:dd.MMM.yyyy}.zip"
                    })
                    {
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string zipPath = saveFileDialog.FileName;

                            if (File.Exists(zipPath))
                            {
                                File.Delete(zipPath);
                            }

                            ZipFile.CreateFromDirectory(downloadPath, zipPath);
                            MessageBox.Show("Download complete.",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1);
                        }
                    }
                }
            }
        }

        private struct ConfigFile
        {
            public uint MagicBytes;
            public uint Version;
            public uint Config;
            public uint VideoMode;
            public uint Language;       // mainly for PAL gamecube games
            public byte[] GamePath;
            public byte[] CheatPath;
            public uint MaxPads;        // old wii only
            public uint GameID;
            public byte MemCardBlocks;
            public sbyte VideoScale;
            public sbyte VideoOffset;
            public byte NetworkProfile; // wii only
            public uint WiiUGamepadSlot;
        }

        private enum NintendontConfig
        {
            Cheats = 1,
            Debugger = (1 << 1), // Only for Wii Version
            DebugWait = (1 << 2),   // Only for Wii Version
            MemcardEmu = (1 << 3), // ENABLED for Wii U and newer Wii
            CheatPath = (1 << 4),
            ForceWide = (1 << 5),
            ForceProg = (1 << 6),
            AutoBoot = (1 << 7),
            Hid = (1 << 8),
            RemLimit = (1 << 8),
            OsReport = (1 << 9),
            Usb = (1 << 10),       // old bit for WiiU Widescreen
            Led = (1 << 11),       // Only for Wii Version
            Log = (1 << 12),

            Multi = (1 << 13),
            NativeSI = (1 << 14),   // Only for Wii Version
            WiiUWide = (1 << 15),   // Only for Wii U Version
            ArcadeMode = (1 << 16),
            CcRumble = (1 << 17),
            SkipIPL = (1 << 18),
            BbaEmu = (1 << 19),
        }

        private enum NintendontVideoMode
        {
            Auto = (0 << 16),
            Force = (1 << 16),
            None = (2 << 16),
            ForceDf = (4 << 16),
            Mask = Auto | Force | None | ForceDf,

            ForcePal50 = (1 << 0), 
            ForcePal60 = (1 << 1),
            ForceNtsc = (1 << 2),
            ForceMpal = (1 << 3),
            ForceMask = ForcePal50 | ForcePal60 | ForceNtsc | ForceMpal,

            Prog = (1 << 4),   // important to prevent blackscreens
            PatchPal50 = (1 << 5), // different force behaviour
        }

        private enum NintendontLanguage : uint
        {
            English = 0,
            German = 1,
            French = 2,
            Spanish = 3,
            Italian = 4,
            Dutch = 5,

            /* Auto will use English for E/P region codes and 
               only other languages when these region codes are used: D/F/S/I/J  */
            Auto = 0xFFFFFFFF,
        }

        private void GenerateConfig_Click(object sender, EventArgs e)
        {
            ConfigFile nintendontCfg = new ConfigFile
            {
                MagicBytes = 0x01070CF6,
                Version = 10,
                Config = 0,
                VideoMode = 0,
                Language = 0,
                GamePath = new byte[256],
                CheatPath = new byte[256],
                MaxPads = 4,
                GameID = 0,
                MemCardBlocks = 0,
                VideoScale = 0,
                VideoOffset = 0,
                NetworkProfile = 0,
                WiiUGamepadSlot = 0
            };

            nintendontCfg.VideoMode |= (uint)NintendontVideoMode.Prog; // always required?

            // Memory Card Emulation
            if (NintendontOptions.GetItemChecked(0))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.MemcardEmu;
            }
            // Cheats
            if (NintendontOptions.GetItemChecked(1))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.Cheats;
            }
            // Cheat Path
            if (NintendontOptions.GetItemChecked(2))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.CheatPath;
            }
            // Unlock Disc Read Speed
            if (NintendontOptions.GetItemChecked(3))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.RemLimit;
            }
            // Wii Remote / Classic Controller Rumble
            if (NintendontOptions.GetItemChecked(4))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.CcRumble;
            }
            // Triforce Arcade Mode
            if (NintendontOptions.GetItemChecked(5))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.ArcadeMode;
            }
            // Broadband Adapter Emulation
            if (NintendontOptions.GetItemChecked(6))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.BbaEmu;
            }
            // AUTO VIDEO WIDTH
            if (NintendontOptions.GetItemChecked(7))
            {
                // nintendontCfg.VideoMode &= (uint)NintendontVideoMode.Mask;
            }
            // Patch PAL 50
            if (NintendontOptions.GetItemChecked(8))
            {
                nintendontCfg.VideoMode |= (uint)NintendontVideoMode.PatchPal50;

                if (VideoTypeMode.SelectedIndex == 0 || VideoTypeMode.SelectedIndex == 3)
                {
                    nintendontCfg.VideoMode |= (uint)NintendontVideoMode.ForcePal50;
                }
            }
            // Force Widescreen
            if (NintendontOptions.GetItemChecked(9))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.ForceWide;
                nintendontCfg.Config |= (uint)NintendontConfig.WiiUWide;
                // nintendontCfg.Config |= (uint)NintendontConfig.Usb;
            }
            // Force Progressive Scan
            if (NintendontOptions.GetItemChecked(10))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.ForceProg;
            }
            // Skip IPL
            if (NintendontOptions.GetItemChecked(11))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.SkipIPL;
            }
            // OSReport
            if (NintendontOptions.GetItemChecked(12))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.OsReport;
            }
            // Log
            if (NintendontOptions.GetItemChecked(13))
            {
                nintendontCfg.Config |= (uint)NintendontConfig.Log;
            }

            // Memcard Multi
            if (MemcardMulti.Checked)
            {
                nintendontCfg.Config |= (uint)NintendontConfig.Multi;
            }


            // VIDEO MODES
            // Auto
            if (VideoForceMode.SelectedIndex == 0)
            {
                // do nothing, it's 0
            }
            // Force
            if (VideoForceMode.SelectedIndex == 1)
            {
                nintendontCfg.VideoMode |= (uint)NintendontVideoMode.Force;
            }
            // Force (Deflicker)
            if (VideoForceMode.SelectedIndex == 2)
            {
                nintendontCfg.VideoMode |= (uint)NintendontVideoMode.ForceDf;
            }
            // None
            if (VideoForceMode.SelectedIndex == 3)
            {
                nintendontCfg.VideoMode |= (uint)NintendontVideoMode.None;
            }


            // VIDEO FORCE OPTIONS
            // None
            if (VideoTypeMode.SelectedIndex == 0)
            {
                // do nothing, it's 0;
            }
            // NTSC
            if (VideoTypeMode.SelectedIndex == 1)
            {
                nintendontCfg.VideoMode |= (uint)NintendontVideoMode.ForceNtsc;
            }
            // MPAL
            if (VideoTypeMode.SelectedIndex == 2)
            {
                nintendontCfg.VideoMode |= (uint)NintendontVideoMode.ForceMpal;
            }
            // PAL50
            if (VideoTypeMode.SelectedIndex == 3)
            {
                nintendontCfg.VideoMode |= (uint)NintendontVideoMode.ForcePal50;
            }
            // PAL60
            if (VideoTypeMode.SelectedIndex == 4)
            {
                nintendontCfg.VideoMode |= (uint)NintendontVideoMode.ForcePal60;
            }


            // LANGUAGE SELECTION
            if (LanguageBox.SelectedIndex == 0)
            {
                nintendontCfg.Language = (uint)NintendontLanguage.Auto;
            }
            else
            {
                nintendontCfg.Language = (uint)LanguageBox.SelectedIndex - 1;
            }

            // MEMCARD BLOCKS
            nintendontCfg.MemCardBlocks = (byte)MemcardBlocks.SelectedIndex;

            // VIDEO WIDTH
            if (NintendontOptions.GetItemChecked(7))
            {
                nintendontCfg.VideoScale = 0;
            }
            else
            {
                nintendontCfg.VideoScale = (sbyte)(VideoWidth.Value - 600);
            }

            // WII U GAMEPAD SLOT
            nintendontCfg.WiiUGamepadSlot = (uint)wiiUGamepadSlotBox.SelectedIndex;

            //
            // SAVING THE FILE
            string savePath = Path.Combine(SelectedDriveLetter, "nincfg.bin");

            // if removable drive isn't specified, save file manually
            if (!DriveSpecified)
            {
                DialogResult dialogResult = CustomMessageBox.ShowYesNo(this, "SD card not specified.\nDo you wish to save the file somewhere else?",
                    "Drive not specified",
                    MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes) // if YES, ask where to save the file
                {
                    using (var saveFileDialog = new SaveFileDialog
                    {
                        Title = "Save nincfg.bin",
                        CheckPathExists = true,
                        DefaultExt = "bin",
                        Filter = "nintendont config files (*.bin)|*.bin",
                        FilterIndex = 2,
                        RestoreDirectory = true,
                        FileName = "nincfg.bin"
                    })
                    {
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            savePath = saveFileDialog.FileName;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            // write it
            using (BinaryWriter cfgFile = new BinaryWriter(File.Open(savePath, FileMode.Create) ) )
            {
                byte[] magicBytes = BitConverter.GetBytes(nintendontCfg.MagicBytes);
                byte[] version = BitConverter.GetBytes(nintendontCfg.Version);
                byte[] config = BitConverter.GetBytes(nintendontCfg.Config);
                byte[] videoMode = BitConverter.GetBytes(nintendontCfg.VideoMode);
                byte[] language = BitConverter.GetBytes(nintendontCfg.Language);
                byte[] maxPads = BitConverter.GetBytes(nintendontCfg.MaxPads);
                byte[] gameID = BitConverter.GetBytes(nintendontCfg.GameID);
                byte[] wiiuGamepadSlot = BitConverter.GetBytes(nintendontCfg.WiiUGamepadSlot);


                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(magicBytes, 0, magicBytes.Length);
                    Array.Reverse(version, 0, version.Length);
                    Array.Reverse(config, 0, config.Length);
                    Array.Reverse(videoMode, 0, videoMode.Length);
                    Array.Reverse(language, 0, language.Length);
                    Array.Reverse(maxPads, 0, maxPads.Length);
                    Array.Reverse(gameID, 0, gameID.Length);
                    Array.Reverse(wiiuGamepadSlot, 0, wiiuGamepadSlot.Length);
                }

                cfgFile.Write(magicBytes);
                cfgFile.Write(version);
                cfgFile.Write(config);
                cfgFile.Write(videoMode);
                cfgFile.Write(language);
                cfgFile.Write(nintendontCfg.GamePath);
                cfgFile.Write(nintendontCfg.CheatPath);
                cfgFile.Write(maxPads);
                cfgFile.Write(gameID);
                cfgFile.Write(nintendontCfg.MemCardBlocks);
                cfgFile.Write(nintendontCfg.VideoScale);
                cfgFile.Write(nintendontCfg.VideoOffset);
                cfgFile.Write(nintendontCfg.NetworkProfile);
                cfgFile.Write(wiiuGamepadSlot);

            }

            MessageBox.Show("Config generation complete."
                            , "Information"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Information
                            , MessageBoxDefaultButton.Button1);
        }

        private void Format_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://ridgecrop.co.uk/index.htm?guiformat.htm");
            Process.Start("http://ridgecrop.co.uk/guiformat.exe");
        }
    }
}
