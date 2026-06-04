namespace HashPDF.Updater
{
    internal sealed class UpdateManifest
    {
        public string version { get; set; }

        public UpdateManifestFile[] files { get; set; }
    }

    internal sealed class UpdateManifestFile
    {
        public string path { get; set; }

        public string assetName { get; set; }

        public string sha256 { get; set; }

        public long size { get; set; }
    }
}
