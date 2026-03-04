using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using HashPDF.WinForms.Exceptions;
using HashPDF.WinForms.Models;

namespace HashPDF.WinForms.Services
{
    public static class HashPdfService
    {
        public static HashPdfResult CreateHashProof(string sourceFilePath)
        {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                throw new HashPdfException(HashPdfErrorCode.MissingFile, "A source file path is required.");
            }

            if (!File.Exists(sourceFilePath))
            {
                throw new HashPdfException(HashPdfErrorCode.MissingFile, "The selected file could not be found.");
            }

            string sourceDirectory = Path.GetDirectoryName(sourceFilePath);
            if (string.IsNullOrEmpty(sourceDirectory))
            {
                throw new HashPdfException(HashPdfErrorCode.SourceDirectoryUnavailable, "The source directory could not be resolved.");
            }

            string hashValue = ComputeSha512(sourceFilePath);
            string outputPdfPath = BuildOutputPdfPath(sourceFilePath);
            try
            {
                PdfDocumentWriter.WriteHashProof(outputPdfPath, hashValue);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new HashPdfException(HashPdfErrorCode.CannotWritePdf, "The PDF could not be written to the source file directory.", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new HashPdfException(HashPdfErrorCode.SourceDirectoryUnavailable, "The source directory is no longer available.", ex);
            }
            catch (IOException ex)
            {
                throw new HashPdfException(HashPdfErrorCode.CannotWritePdf, "The PDF could not be written to the source file directory.", ex);
            }

            return new HashPdfResult(sourceFilePath, outputPdfPath, hashValue);
        }

        private static string BuildOutputPdfPath(string sourceFilePath)
        {
            string directory = Path.GetDirectoryName(sourceFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath);
            string safeBaseName = string.IsNullOrEmpty(fileNameWithoutExtension) ? "hash-output" : fileNameWithoutExtension;
            return Path.Combine(directory, safeBaseName + ".hash.pdf");
        }

        private static string ComputeSha512(string sourceFilePath)
        {
            using (FileStream stream = File.OpenRead(sourceFilePath))
            using (SHA512 hashAlgorithm = SHA512.Create())
            {
                byte[] hashBytes = hashAlgorithm.ComputeHash(stream);
                StringBuilder builder = new StringBuilder(hashBytes.Length * 2);
                for (int index = 0; index < hashBytes.Length; index++)
                {
                    builder.Append(hashBytes[index].ToString("X2", CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }
        }
    }
}
