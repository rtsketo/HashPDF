using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using HashPDF.WinForms.Models;

namespace HashPDF.WinForms.Services
{
    public static class UpdateCheckService
    {
        private const string LatestReleaseUrl = "https://api.github.com/repos/rtsketo/HashPDF/releases/latest";
        private const string UserAgent = "HashPDF-Updater";
        private const string ManifestAssetName = "HashPDF.update.json";
        private const string AppAssetName = "HashPDF.exe";
        private const string UpdaterAssetName = "HashPDF.Updater.exe";

        public static UpdateInfo CheckForUpdate()
        {
            string json = DownloadString(GetLatestReleaseUrl());
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> release = serializer.Deserialize<Dictionary<string, object>>(json);
            if (release == null || GetBoolean(release, "draft") || GetBoolean(release, "prerelease"))
            {
                return null;
            }

            string tagName = GetString(release, "tag_name");
            Version latestVersion;
            if (!TryParseReleaseVersion(tagName, out latestVersion))
            {
                return null;
            }

            Version currentVersion = GetCurrentSemanticVersion();
            if (latestVersion.CompareTo(currentVersion) <= 0)
            {
                return null;
            }

            if (!HasRequiredUpdateAssets(release))
            {
                return null;
            }

            return new UpdateInfo(latestVersion.ToString(3), tagName, GetString(release, "html_url"));
        }

        private static string DownloadString(string url)
        {
            ConfigureNetwork();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = UserAgent;
            request.Accept = "application/vnd.github+json";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private static string GetLatestReleaseUrl()
        {
            string overrideUrl = Environment.GetEnvironmentVariable("HASHPDF_UPDATE_LATEST_URL");
            return string.IsNullOrEmpty(overrideUrl) ? LatestReleaseUrl : overrideUrl;
        }

        private static bool HasRequiredUpdateAssets(Dictionary<string, object> release)
        {
            bool hasManifest = false;
            bool hasApp = false;
            bool hasUpdater = false;

            if (!release.ContainsKey("assets"))
            {
                return false;
            }

            IEnumerable assets = release["assets"] as IEnumerable;
            if (assets == null)
            {
                return false;
            }

            foreach (object assetObject in assets)
            {
                Dictionary<string, object> asset = assetObject as Dictionary<string, object>;
                if (asset == null)
                {
                    continue;
                }

                string name = GetString(asset, "name");
                hasManifest = hasManifest || ManifestAssetName.Equals(name, StringComparison.OrdinalIgnoreCase);
                hasApp = hasApp || AppAssetName.Equals(name, StringComparison.OrdinalIgnoreCase);
                hasUpdater = hasUpdater || UpdaterAssetName.Equals(name, StringComparison.OrdinalIgnoreCase);
            }

            return hasManifest && hasApp && hasUpdater;
        }

        private static Version GetCurrentSemanticVersion()
        {
            Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return new Version(assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build);
        }

        private static bool TryParseReleaseVersion(string tagName, out Version version)
        {
            version = null;
            if (string.IsNullOrEmpty(tagName))
            {
                return false;
            }

            string value = tagName.StartsWith("v", StringComparison.OrdinalIgnoreCase)
                ? tagName.Substring(1)
                : tagName;

            Version parsedVersion;
            if (!Version.TryParse(value, out parsedVersion) || parsedVersion.Build < 0)
            {
                return false;
            }

            version = new Version(parsedVersion.Major, parsedVersion.Minor, parsedVersion.Build);
            return true;
        }

        private static void ConfigureNetwork()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }

        private static string GetString(Dictionary<string, object> values, string key)
        {
            if (!values.ContainsKey(key) || values[key] == null)
            {
                return null;
            }

            return Convert.ToString(values[key]);
        }

        private static bool GetBoolean(Dictionary<string, object> values, string key)
        {
            if (!values.ContainsKey(key) || values[key] == null)
            {
                return false;
            }

            return Convert.ToBoolean(values[key]);
        }
    }
}
