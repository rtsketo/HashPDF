using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using HashPDF.WinForms.Localization;
using HashPDF.WinForms.Models;

namespace HashPDF.WinForms.Services
{
    public static class UpdateLauncher
    {
        private const string UpdaterFileName = "HashPDF.Updater.exe";

        public static void Launch(UpdateInfo updateInfo, AppLanguage language)
        {
            string installDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string sourceUpdaterPath = Path.Combine(installDirectory, UpdaterFileName);
            if (!File.Exists(sourceUpdaterPath))
            {
                throw new FileNotFoundException("The updater executable was not found.", sourceUpdaterPath);
            }

            string runnerDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "HashPDF",
                "Updater");
            Directory.CreateDirectory(runnerDirectory);

            string runnerPath = Path.Combine(runnerDirectory, UpdaterFileName);
            File.Copy(sourceUpdaterPath, runnerPath, true);

            ProcessStartInfo startInfo = new ProcessStartInfo(runnerPath);
            startInfo.Arguments = BuildArguments(updateInfo, installDirectory, language);
            startInfo.UseShellExecute = false;
            Process.Start(startInfo);
        }

        private static string BuildArguments(UpdateInfo updateInfo, string installDirectory, AppLanguage language)
        {
            StringBuilder builder = new StringBuilder();
            AppendArgument(builder, "--parent-pid", Process.GetCurrentProcess().Id.ToString());
            AppendArgument(builder, "--install-dir", installDirectory);
            AppendArgument(builder, "--version", updateInfo.Version);
            AppendArgument(builder, "--tag", updateInfo.TagName);
            AppendArgument(builder, "--language", language == AppLanguage.English ? "english" : "greek");
            return builder.ToString();
        }

        private static void AppendArgument(StringBuilder builder, string key, string value)
        {
            if (builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(key);
            builder.Append(' ');
            builder.Append(Quote(value));
        }

        private static string Quote(string value)
        {
            if (value == null)
            {
                return "\"\"";
            }

            StringBuilder builder = new StringBuilder();
            builder.Append('"');
            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                if (character == '\\' || character == '"')
                {
                    builder.Append('\\');
                }

                builder.Append(character);
            }

            builder.Append('"');
            return builder.ToString();
        }
    }
}
