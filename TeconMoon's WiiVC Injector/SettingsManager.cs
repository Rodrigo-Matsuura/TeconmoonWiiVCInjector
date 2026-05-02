using System;
using System.IO;
using Newtonsoft.Json;

namespace TeconMoon_s_WiiVC_Injector
{
    public class AppSettings
    {
        public string WiiUCommonKey { get; set; } = "00000000000000000000000000000000";
        public string TitleKey { get; set; } = "00000000000000000000000000000000";
        public string AncastKey { get; set; } = "00000000000000000000000000000000";
    }

    public static class SettingsManager
    {
        private static readonly string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WiiVCInjector", "settings.json");

        static SettingsManager()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath));
        }

        public static AppSettings LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                string json = File.ReadAllText(SettingsFilePath);
                return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
            }
            return new AppSettings();
        }

        public static void SaveSettings(AppSettings settings)
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
        }

        public static string GetSetting(string key)
        {
            var settings = LoadSettings();
            switch (key)
            {
                case "WiiUCommonKey": return settings.WiiUCommonKey;
                case "TitleKey": return settings.TitleKey;
                case "AncastKey": return settings.AncastKey;
                default: return "00000000000000000000000000000000";
            }
        }

        public static void SetSetting(string key, string value)
        {
            var settings = LoadSettings();
            switch (key)
            {
                case "WiiUCommonKey": settings.WiiUCommonKey = value; break;
                case "TitleKey": settings.TitleKey = value; break;
                case "AncastKey": settings.AncastKey = value; break;
            }
            SaveSettings(settings);
        }
    }
}