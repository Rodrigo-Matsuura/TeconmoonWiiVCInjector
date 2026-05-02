using System;
using System.Drawing;
using System.Windows.Forms;

namespace TeconMoon_s_WiiVC_Injector
{
    public static class CustomMessageBox
    {
        public static DialogResult ShowYesNo(IWin32Window owner, string text, string caption, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            using (var form = new Form())
            {
                form.Text = caption;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.ShowIcon = false;
                form.ShowInTaskbar = false;
                form.StartPosition = owner != null ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen;
                form.AutoScaleMode = AutoScaleMode.Font;
                form.ClientSize = new Size(420, 160);

                var label = new Label
                {
                    AutoSize = false,
                    Text = text,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Location = new Point(12, 12),
                    Size = new Size(form.ClientSize.Width - 24, form.ClientSize.Height - 70)
                };

                var yesButton = new Button
                {
                    Text = "Yes",
                    DialogResult = DialogResult.Yes,
                    Size = new Size(80, 28)
                };
                var noButton = new Button
                {
                    Text = "No",
                    DialogResult = DialogResult.No,
                    Size = new Size(80, 28)
                };

                form.Controls.Add(label);
                form.Controls.Add(yesButton);
                form.Controls.Add(noButton);

                int textHeight = TextRenderer.MeasureText(text, label.Font, new Size(label.Width, int.MaxValue), TextFormatFlags.WordBreak).Height;
                label.Height = textHeight + 10;
                int desiredHeight = label.Bottom + 60;
                form.ClientSize = new Size(form.ClientSize.Width, Math.Max(desiredHeight, 140));

                yesButton.Location = new Point(form.ClientSize.Width - 190, form.ClientSize.Height - 40);
                noButton.Location = new Point(form.ClientSize.Width - 100, form.ClientSize.Height - 40);

                form.AcceptButton = yesButton;
                form.CancelButton = noButton;

                return form.ShowDialog(owner);
            }
        }
    }
}
