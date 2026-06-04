using System;
using System.Windows.Forms;

namespace HashPDF.Updater
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            UpdateArguments updateArguments;
            if (!UpdateArguments.TryParse(args, out updateArguments))
            {
                MessageBox.Show(
                    "HashPDF updater could not start because the update arguments are invalid.",
                    "HashPDF Updater",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UpdaterForm(updateArguments));
        }
    }
}
