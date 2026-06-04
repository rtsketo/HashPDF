namespace HashPDF.WinForms.Models
{
    public sealed class DxfConversionResult
    {
        public DxfConversionResult(
            string sourceFilePath,
            string outputFilePath,
            DxfConversionMode mode,
            int convertedEntityCount)
        {
            SourceFilePath = sourceFilePath;
            OutputFilePath = outputFilePath;
            Mode = mode;
            ConvertedEntityCount = convertedEntityCount;
        }

        public string SourceFilePath { get; private set; }

        public string OutputFilePath { get; private set; }

        public DxfConversionMode Mode { get; private set; }

        public int ConvertedEntityCount { get; private set; }
    }
}
