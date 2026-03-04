using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace HashPDF.WinForms.Services
{
    public static class PdfDocumentWriter
    {
        public static void WriteHashProof(string outputPdfPath, string hashValue)
        {
            string contentStream = BuildContentStream(hashValue);
            List<string> objects = new List<string>();
            objects.Add("<< /Type /Catalog /Pages 2 0 R >>");
            objects.Add("<< /Type /Pages /Count 1 /Kids [3 0 R] >>");
            objects.Add("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>");
            objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Courier >>");
            objects.Add(string.Format(CultureInfo.InvariantCulture, "<< /Length {0} >>\nstream\n{1}endstream", contentStream.Length, contentStream));

            WritePdf(outputPdfPath, objects);
        }

        private static void WritePdf(string outputPdfPath, IList<string> objects)
        {
            Encoding encoding = Encoding.ASCII;

            using (FileStream stream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (StreamWriter writer = new StreamWriter(stream, encoding))
            {
                List<long> offsets = new List<long>();

                writer.WriteLine("%PDF-1.4");

                for (int index = 0; index < objects.Count; index++)
                {
                    writer.Flush();
                    offsets.Add(stream.Position);
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} 0 obj", index + 1));
                    writer.WriteLine(objects[index]);
                    writer.WriteLine("endobj");
                }

                writer.Flush();
                long xrefPosition = stream.Position;

                writer.WriteLine("xref");
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "0 {0}", objects.Count + 1));
                writer.WriteLine("0000000000 65535 f ");

                for (int index = 0; index < offsets.Count; index++)
                {
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0000000000} 00000 n ", offsets[index]));
                }

                writer.WriteLine("trailer");
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "<< /Size {0} /Root 1 0 R >>", objects.Count + 1));
                writer.WriteLine("startxref");
                writer.WriteLine(xrefPosition.ToString(CultureInfo.InvariantCulture));
                writer.WriteLine("%%EOF");
            }
        }

        private static string BuildContentStream(string hashValue)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("BT\n");
            builder.Append("/F1 7 Tf\n");
            builder.Append("1 0 0 1 28 790 Tm\n");
            builder.Append("(");
            builder.Append(EscapePdfText(hashValue));
            builder.Append(") Tj\n");
            builder.Append("ET\n");
            return builder.ToString();
        }

        private static string EscapePdfText(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)");
        }
    }
}
