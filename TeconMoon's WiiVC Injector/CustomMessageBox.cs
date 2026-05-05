using System.Windows.Forms;

namespace TeconMoon_s_WiiVC_Injector
{
    public static class CustomMessageBox
    {
        public static DialogResult ShowYesNo(IWin32Window owner, string text, string caption = "", MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, MessageBoxOptions options = 0)
        {
            return MessageBox.Show(owner, text, caption, MessageBoxButtons.YesNo, icon, defaultButton, options);
        }
    }
}
