using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace HashPDF.Updater
{
    internal sealed class UpdaterEngine
    {
        private const string ManifestAssetName = "HashPDF.update.json";
        private readonly Action<UpdateProgress> reportProgress;
        private readonly UpdaterText text;
        private readonly ReleaseClient releaseClient;

        public UpdaterEngine(Action<UpdateProgress> reportProgress, UpdaterText text)
        {
            this.reportProgress = reportProgress;
            this.text = text;
            releaseClient = new ReleaseClient();
        }

        public UpdateRunResult Run(UpdateArguments arguments)
        {
            Report(text.Starting, arguments.TargetVersion, 5, false);

            if (!CanWriteToInstallDirectory(arguments.InstallDirectory))
            {
                if (!arguments.Elevated)
                {
                    Report(text.Elevating, arguments.InstallDirectory, 10, true);
                    RelaunchElevated(arguments);
                    return UpdateRunResult.ElevationStarted;
                }
            }

            WaitForParent(arguments.ParentProcessId);

            string updateRoot = BuildUpdateRoot(arguments.TargetVersion);
            string downloadRoot = Path.Combine(updateRoot, "download");
            string backupRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "HashPDF",
                "Updates",
                "backup",
                DateTime.Now.ToString("yyyyMMddHHmmss"));

            Directory.CreateDirectory(downloadRoot);
            Directory.CreateDirectory(backupRoot);

            IList<ReleaseAsset> assets = releaseClient.GetReleaseAssets(arguments.TagName);
            ReleaseAsset manifestAsset = FindAsset(assets, ManifestAssetName);
            if (manifestAsset == null)
            {
                throw new InvalidOperationException("The update manifest was not found in the GitHub release.");
            }

            Report(text.Downloading, ManifestAssetName, 20, false);
            string manifestJson = releaseClient.DownloadString(manifestAsset.DownloadUrl);
            UpdateManifest manifest = new JavaScriptSerializer().Deserialize<UpdateManifest>(manifestJson);
            ValidateManifest(manifest, arguments.TargetVersion);

            Dictionary<UpdateManifestFile, string> downloadedFiles = DownloadFiles(manifest, assets, downloadRoot);
            VerifyDownloadedFiles(downloadedFiles);

            List<BackupEntry> backups = new List<BackupEntry>();
            try
            {
                BackupFiles(manifest, arguments.InstallDirectory, backupRoot, backups);
                ReplaceFiles(downloadedFiles, arguments.InstallDirectory);
            }
            catch
            {
                Report(text.RollingBack, backupRoot, 90, true);
                RollBack(backups);
                throw;
            }

            Report(text.Restarting, "HashPDF.exe", 98, false);
            RestartApplication(arguments.InstallDirectory);
            Report(text.Complete, arguments.TargetVersion, 100, false);
            return UpdateRunResult.Completed;
        }

        private Dictionary<UpdateManifestFile, string> DownloadFiles(
            UpdateManifest manifest,
            IList<ReleaseAsset> assets,
            string downloadRoot)
        {
            Dictionary<UpdateManifestFile, string> downloadedFiles = new Dictionary<UpdateManifestFile, string>();
            for (int index = 0; index < manifest.files.Length; index++)
            {
                UpdateManifestFile file = manifest.files[index];
                ReleaseAsset asset = FindAsset(assets, file.assetName);
                if (asset == null)
                {
                    throw new InvalidOperationException("Update asset was not found: " + file.assetName);
                }

                int percent = 25 + ((index * 35) / Math.Max(1, manifest.files.Length));
                Report(text.Downloading, file.assetName, percent, false);
                string downloadPath = Path.Combine(downloadRoot, file.assetName);
                releaseClient.DownloadFile(asset.DownloadUrl, downloadPath);
                downloadedFiles.Add(file, downloadPath);
            }

            return downloadedFiles;
        }

        private void VerifyDownloadedFiles(Dictionary<UpdateManifestFile, string> downloadedFiles)
        {
            Report(text.Verifying, string.Empty, 65, false);
            foreach (KeyValuePair<UpdateManifestFile, string> pair in downloadedFiles)
            {
                FileInfo fileInfo = new FileInfo(pair.Value);
                if (fileInfo.Length != pair.Key.size)
                {
                    throw new InvalidOperationException("Downloaded file size did not match: " + pair.Key.assetName);
                }

                string hash = ComputeSha256(pair.Value);
                if (!hash.Equals(pair.Key.sha256, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Downloaded file checksum did not match: " + pair.Key.assetName);
                }
            }
        }

        private void BackupFiles(
            UpdateManifest manifest,
            string installDirectory,
            string backupRoot,
            IList<BackupEntry> backups)
        {
            Report(text.Installing, "Backup", 75, false);
            for (int i = 0; i < manifest.files.Length; i++)
            {
                UpdateManifestFile file = manifest.files[i];
                string targetPath = ResolveTargetPath(installDirectory, file.path);
                string backupPath = Path.Combine(backupRoot, file.path);

                if (!File.Exists(targetPath))
                {
                    backups.Add(new BackupEntry(targetPath, backupPath, false));
                    continue;
                }

                string backupDirectory = Path.GetDirectoryName(backupPath);
                if (!string.IsNullOrEmpty(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                }

                File.Copy(targetPath, backupPath, true);
                backups.Add(new BackupEntry(targetPath, backupPath, true));
            }
        }

        private void ReplaceFiles(Dictionary<UpdateManifestFile, string> downloadedFiles, string installDirectory)
        {
            Report(text.Installing, installDirectory, 85, false);
            foreach (KeyValuePair<UpdateManifestFile, string> pair in downloadedFiles)
            {
                string targetPath = ResolveTargetPath(installDirectory, pair.Key.path);
                string targetDirectory = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                File.Copy(pair.Value, targetPath, true);
            }
        }

        private void RollBack(IEnumerable<BackupEntry> backups)
        {
            foreach (BackupEntry backup in backups)
            {
                if (backup.Existed)
                {
                    string targetDirectory = Path.GetDirectoryName(backup.TargetPath);
                    if (!string.IsNullOrEmpty(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    File.Copy(backup.BackupPath, backup.TargetPath, true);
                }
                else if (File.Exists(backup.TargetPath))
                {
                    File.Delete(backup.TargetPath);
                }
            }
        }

        private void WaitForParent(int parentProcessId)
        {
            if (parentProcessId <= 0)
            {
                return;
            }

            try
            {
                Process parent = Process.GetProcessById(parentProcessId);
                Report(text.Waiting, parent.ProcessName, 15, true);
                parent.WaitForExit();
            }
            catch (ArgumentException)
            {
                // Process has already exited.
            }
        }

        private void RelaunchElevated(UpdateArguments arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(Application.ExecutablePath);
            startInfo.Arguments = arguments.ToCommandLine(true);
            startInfo.UseShellExecute = true;
            startInfo.Verb = "runas";

            try
            {
                Process.Start(startInfo);
            }
            catch (Win32Exception ex)
            {
                throw new InvalidOperationException("Administrator permission was not granted.", ex);
            }
        }

        private bool CanWriteToInstallDirectory(string installDirectory)
        {
            try
            {
                Directory.CreateDirectory(installDirectory);
                string testPath = Path.Combine(installDirectory, ".hashpdf-update-write-test-" + Guid.NewGuid().ToString("N"));
                File.WriteAllText(testPath, "test");
                File.Delete(testPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private static void ValidateManifest(UpdateManifest manifest, string targetVersion)
        {
            if (manifest == null
                || string.IsNullOrEmpty(manifest.version)
                || manifest.files == null
                || manifest.files.Length == 0)
            {
                throw new InvalidOperationException("The update manifest is invalid.");
            }

            if (!manifest.version.Equals(targetVersion, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The update manifest version did not match the selected release.");
            }

            for (int i = 0; i < manifest.files.Length; i++)
            {
                UpdateManifestFile file = manifest.files[i];
                if (file == null
                    || string.IsNullOrEmpty(file.path)
                    || string.IsNullOrEmpty(file.assetName)
                    || string.IsNullOrEmpty(file.sha256)
                    || file.size <= 0)
                {
                    throw new InvalidOperationException("The update manifest contains an invalid file entry.");
                }
            }
        }

        private static string ResolveTargetPath(string installDirectory, string relativePath)
        {
            if (Path.IsPathRooted(relativePath) || relativePath.IndexOf("..", StringComparison.Ordinal) >= 0)
            {
                throw new InvalidOperationException("The update manifest contains an unsafe path.");
            }

            string installRoot = Path.GetFullPath(installDirectory);
            string targetPath = Path.GetFullPath(Path.Combine(installRoot, relativePath));
            string normalizedRoot = installRoot.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                ? installRoot
                : installRoot + Path.DirectorySeparatorChar;

            if (!targetPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The update manifest contains a path outside the install directory.");
            }

            return targetPath;
        }

        private static ReleaseAsset FindAsset(IList<ReleaseAsset> assets, string assetName)
        {
            for (int i = 0; i < assets.Count; i++)
            {
                if (assets[i].Name.Equals(assetName, StringComparison.OrdinalIgnoreCase))
                {
                    return assets[i];
                }
            }

            return null;
        }

        private static string BuildUpdateRoot(string targetVersion)
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "HashPDF",
                "Updates",
                targetVersion + "-" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        private static string ComputeSha256(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            using (SHA256 algorithm = SHA256.Create())
            {
                byte[] hashBytes = algorithm.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        private static void RestartApplication(string installDirectory)
        {
            string executablePath = Path.Combine(installDirectory, "HashPDF.exe");
            if (File.Exists(executablePath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(executablePath);
                startInfo.UseShellExecute = true;
                Process.Start(startInfo);
            }
        }

        private void Report(string title, string detail, int percent, bool indeterminate)
        {
            if (reportProgress != null)
            {
                reportProgress(new UpdateProgress(title, detail, percent, indeterminate));
            }
        }

        private sealed class BackupEntry
        {
            public BackupEntry(string targetPath, string backupPath, bool existed)
            {
                TargetPath = targetPath;
                BackupPath = backupPath;
                Existed = existed;
            }

            public string TargetPath { get; private set; }

            public string BackupPath { get; private set; }

            public bool Existed { get; private set; }
        }
    }

    internal enum UpdateRunResult
    {
        Completed,
        ElevationStarted
    }
}
