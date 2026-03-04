using System;

namespace HashPDF.WinForms.Exceptions
{
    public enum HashPdfErrorCode
    {
        Unknown = 0,
        MissingFile = 1,
        SourceDirectoryUnavailable = 2,
        CannotWritePdf = 3
    }

    public sealed class HashPdfException : Exception
    {
        public HashPdfException(HashPdfErrorCode code, string message)
            : base(message)
        {
            Code = code;
        }

        public HashPdfException(HashPdfErrorCode code, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }

        public HashPdfErrorCode Code { get; private set; }
    }
}
