using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace HashPDF.Updater
{
    internal sealed class ReleaseClient
    {
        private const string RepositoryApiUrl = "https://api.github.com/repos/rtsketo/HashPDF";
        private const string UserAgent = "HashPDF-Updater";

        public IList<ReleaseAsset> GetReleaseAssets(string tagName)
        {
            string url = BuildReleaseUrl(tagName);
            string json = DownloadString(url);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> release = serializer.Deserialize<Dictionary<string, object>>(json);
            if (release == null || !release.ContainsKey("assets"))
            {
                throw new InvalidOperationException("The GitHub release did not include update assets.");
            }

            List<ReleaseAsset> assets = new List<ReleaseAsset>();
            IEnumerable assetList = release["assets"] as IEnumerable;
            if (assetList == null)
            {
                return assets;
            }

            foreach (object assetObject in assetList)
            {
                Dictionary<string, object> asset = assetObject as Dictionary<string, object>;
                if (asset == null)
                {
                    continue;
                }

                string name = GetString(asset, "name");
                string downloadUrl = GetString(asset, "browser_download_url");
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(downloadUrl))
                {
                    continue;
                }

                ReleaseAsset releaseAsset = new ReleaseAsset();
                releaseAsset.Name = name;
                releaseAsset.DownloadUrl = downloadUrl;
                releaseAsset.Size = GetLong(asset, "size");
                assets.Add(releaseAsset);
            }

            return assets;
        }

        private static string BuildReleaseUrl(string tagName)
        {
            string overrideTemplate = Environment.GetEnvironmentVariable("HASHPDF_UPDATE_RELEASE_URL_TEMPLATE");
            if (!string.IsNullOrEmpty(overrideTemplate))
            {
                return overrideTemplate.Replace("{tag}", Uri.EscapeDataString(tagName));
            }

            return RepositoryApiUrl + "/releases/tags/" + Uri.EscapeDataString(tagName);
        }

        public string DownloadString(string url)
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

        public void DownloadFile(string url, string destinationPath)
        {
            ConfigureNetwork();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = UserAgent;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream source = response.GetResponseStream())
            using (FileStream destination = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] buffer = new byte[81920];
                while (true)
                {
                    int bytesRead = source.Read(buffer, 0, buffer.Length);
                    if (bytesRead <= 0)
                    {
                        break;
                    }

                    destination.Write(buffer, 0, bytesRead);
                }
            }
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

        private static long GetLong(Dictionary<string, object> values, string key)
        {
            if (!values.ContainsKey(key) || values[key] == null)
            {
                return 0L;
            }

            return Convert.ToInt64(values[key]);
        }
    }
}
