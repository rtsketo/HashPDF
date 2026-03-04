using System.Drawing;
using System.Windows.Forms;

namespace HashPDF.WinForms
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "HashPDF";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(960, 640);
            ClientSize = new Size(1100, 720);
            BackColor = Color.FromArgb(245, 247, 250);
        }
    }
}
