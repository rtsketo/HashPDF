namespace HashPDF.WinForms.Models
{
    public sealed class UpdateInfo
    {
        public UpdateInfo(string version, string tagName, string releaseUrl)
        {
            Version = version;
            TagName = tagName;
            ReleaseUrl = releaseUrl;
        }

        public string Version { get; private set; }

        public string TagName { get; private set; }

        public string ReleaseUrl { get; private set; }
    }
}
