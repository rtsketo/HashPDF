namespace HashPDF.WinForms.Models
{
    public sealed class DxfConversionRequest
    {
        public DxfConversionRequest(string sourceFilePath, DxfConversionMode mode)
        {
            SourceFilePath = sourceFilePath;
            Mode = mode;
        }

        public string SourceFilePath { get; private set; }

        public DxfConversionMode Mode { get; private set; }
    }
}
