using System;
using System.Net.Http;
using System.Windows.Forms;

namespace TeconMoon_s_WiiVC_Injector
{
    static class Program
    {
        public static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WiiVC_Injector());
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                var response = Client.GetAsync("http://clients3.google.com/generate_204").Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
