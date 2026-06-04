using System;

namespace HashPDF.WinForms.Exceptions
{
    public enum DxfConversionErrorCode
    {
        Unknown = 0,
        MissingFile = 1,
        SourceDirectoryUnavailable = 2,
        UnsupportedFileType = 3,
        UnsupportedBinaryDxf = 4,
        MalformedDxf = 5,
        CannotWriteDxf = 6,
        NoMatchingMText = 7
    }

    public sealed class DxfConversionException : Exception
    {
        public DxfConversionException(DxfConversionErrorCode code, string message)
            : base(message)
        {
            Code = code;
        }

        public DxfConversionException(DxfConversionErrorCode code, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }

        public DxfConversionErrorCode Code { get; private set; }
    }
}
