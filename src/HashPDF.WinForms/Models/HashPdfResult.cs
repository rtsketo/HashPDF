namespace HashPDF.WinForms.Models
{
    public sealed class HashPdfResult
    {
        public HashPdfResult(string sourceFilePath, string outputPdfPath, string hashValue)
        {
            SourceFilePath = sourceFilePath;
            OutputPdfPath = outputPdfPath;
            HashValue = hashValue;
        }

        public string SourceFilePath { get; private set; }

        public string OutputPdfPath { get; private set; }

        public string HashValue { get; private set; }
    }
}
