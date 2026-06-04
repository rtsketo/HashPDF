using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using HashPDF.WinForms.Exceptions;
using HashPDF.WinForms.Models;

namespace HashPDF.WinForms.Services
{
    public static class DxfConversionService
    {
        private const string KaekLayerName = "PST_KAEK";
        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static DxfConversionResult ConvertMTextToText(DxfConversionRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.SourceFilePath))
            {
                throw new DxfConversionException(DxfConversionErrorCode.MissingFile, "A DXF file path is required.");
            }

            if (!File.Exists(request.SourceFilePath))
            {
                throw new DxfConversionException(DxfConversionErrorCode.MissingFile, "The selected DXF file could not be found.");
            }

            if (!Path.GetExtension(request.SourceFilePath).Equals(".dxf", StringComparison.OrdinalIgnoreCase))
            {
                throw new DxfConversionException(DxfConversionErrorCode.UnsupportedFileType, "The selected file is not a DXF file.");
            }

            string sourceDirectory = Path.GetDirectoryName(request.SourceFilePath);
            if (string.IsNullOrEmpty(sourceDirectory))
            {
                throw new DxfConversionException(DxfConversionErrorCode.SourceDirectoryUnavailable, "The source directory could not be resolved.");
            }

            byte[] bytes = File.ReadAllBytes(request.SourceFilePath);
            if (IsBinaryDxf(bytes))
            {
                throw new DxfConversionException(DxfConversionErrorCode.UnsupportedBinaryDxf, "Binary DXF files are not supported.");
            }

            Encoding encoding = DetectEncoding(bytes);
            string newline = DetectNewLine(bytes);
            string content = encoding.GetString(bytes);
            List<string> lines = SplitLines(content);
            if (lines.Count > 0 && lines[0].Length > 0 && lines[0][0] == '\uFEFF')
            {
                lines[0] = lines[0].Substring(1);
            }

            if (lines.Count < 2 || lines.Count % 2 != 0)
            {
                throw new DxfConversionException(DxfConversionErrorCode.MalformedDxf, "The DXF file does not contain valid code/value pairs.");
            }

            List<DxfPair> pairs = BuildPairs(lines);
            if (!LooksLikeDxf(pairs))
            {
                throw new DxfConversionException(DxfConversionErrorCode.MalformedDxf, "The selected file does not look like an ASCII DXF file.");
            }

            int nextHandle = FindNextHandle(pairs);
            List<DxfPair> outputPairs = new List<DxfPair>(pairs.Count);
            int convertedEntityCount = 0;

            for (int index = 0; index < pairs.Count;)
            {
                if (IsEntityStart(pairs[index], "MTEXT"))
                {
                    int entityEndIndex = FindEntityEnd(pairs, index + 1);
                    List<DxfPair> entityPairs = pairs.GetRange(index, entityEndIndex - index);
                    if (ShouldConvertEntity(entityPairs, request.Mode))
                    {
                        outputPairs.AddRange(ConvertMTextEntity(entityPairs, ref nextHandle));
                        convertedEntityCount++;
                    }
                    else
                    {
                        outputPairs.AddRange(ClonePairs(entityPairs));
                    }

                    index = entityEndIndex;
                    continue;
                }

                outputPairs.Add(pairs[index].Clone());
                index++;
            }

            if (convertedEntityCount == 0)
            {
                throw new DxfConversionException(DxfConversionErrorCode.NoMatchingMText, "No matching MTEXT entities were found.");
            }

            UpdateHandSeed(outputPairs, nextHandle);

            string outputFilePath = BuildOutputFilePath(request.SourceFilePath, request.Mode);
            try
            {
                File.WriteAllText(outputFilePath, SerializePairs(outputPairs, newline), encoding);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new DxfConversionException(DxfConversionErrorCode.CannotWriteDxf, "The converted DXF could not be written.", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new DxfConversionException(DxfConversionErrorCode.SourceDirectoryUnavailable, "The source directory is no longer available.", ex);
            }
            catch (IOException ex)
            {
                throw new DxfConversionException(DxfConversionErrorCode.CannotWriteDxf, "The converted DXF could not be written.", ex);
            }

            return new DxfConversionResult(
                request.SourceFilePath,
                outputFilePath,
                request.Mode,
                convertedEntityCount);
        }

        private static bool IsBinaryDxf(byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return false;
            }

            string prefix = Encoding.ASCII.GetString(bytes, 0, Math.Min(bytes.Length, 22));
            if (prefix.StartsWith("AutoCAD Binary DXF", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (HasUnicodeBom(bytes))
            {
                return false;
            }

            for (int index = 0; index < bytes.Length; index++)
            {
                if (bytes[index] == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasUnicodeBom(byte[] bytes)
        {
            return bytes.Length >= 2
                && ((bytes[0] == 0xFF && bytes[1] == 0xFE)
                    || (bytes[0] == 0xFE && bytes[1] == 0xFF)
                    || (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF));
        }

        private static Encoding DetectEncoding(byte[] bytes)
        {
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                return new UTF8Encoding(true);
            }

            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            {
                return Encoding.Unicode;
            }

            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode;
            }

            string codePage = FindDwgCodePage(bytes);
            if (!string.IsNullOrEmpty(codePage))
            {
                Encoding dxfEncoding = TryGetDwgCodePageEncoding(codePage);
                if (dxfEncoding != null)
                {
                    return dxfEncoding;
                }
            }

            return Encoding.Default;
        }

        private static string FindDwgCodePage(byte[] bytes)
        {
            string asciiContent = Encoding.ASCII.GetString(bytes);
            List<string> lines = SplitLines(asciiContent);
            for (int index = 0; index < lines.Count - 2; index++)
            {
                if (lines[index].Trim().Equals("$DWGCODEPAGE", StringComparison.OrdinalIgnoreCase))
                {
                    return lines[index + 2].Trim();
                }
            }

            return null;
        }

        private static Encoding TryGetDwgCodePageEncoding(string codePage)
        {
            try
            {
                if (codePage.StartsWith("ANSI_", StringComparison.OrdinalIgnoreCase))
                {
                    int pageNumber;
                    if (int.TryParse(codePage.Substring(5), NumberStyles.Integer, InvariantCulture, out pageNumber))
                    {
                        return Encoding.GetEncoding(pageNumber);
                    }
                }

                if (codePage.Equals("UTF-8", StringComparison.OrdinalIgnoreCase)
                    || codePage.Equals("UTF8", StringComparison.OrdinalIgnoreCase))
                {
                    return new UTF8Encoding(false);
                }

                return Encoding.GetEncoding(codePage);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static string DetectNewLine(byte[] bytes)
        {
            for (int index = 0; index < bytes.Length; index++)
            {
                if (bytes[index] == 0x0A)
                {
                    return index > 0 && bytes[index - 1] == 0x0D ? "\r\n" : "\n";
                }
            }

            return Environment.NewLine;
        }

        private static List<string> SplitLines(string content)
        {
            List<string> lines = new List<string>();
            using (StringReader reader = new StringReader(content))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

        private static List<DxfPair> BuildPairs(List<string> lines)
        {
            List<DxfPair> pairs = new List<DxfPair>(lines.Count / 2);
            for (int index = 0; index < lines.Count; index += 2)
            {
                pairs.Add(new DxfPair(lines[index], lines[index + 1]));
            }

            return pairs;
        }

        private static bool LooksLikeDxf(List<DxfPair> pairs)
        {
            for (int index = 0; index < Math.Min(pairs.Count, 16); index++)
            {
                if (IsEntityStart(pairs[index], "SECTION"))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsEntityStart(DxfPair pair, string entityType)
        {
            return pair.Code.Trim().Equals("0", StringComparison.OrdinalIgnoreCase)
                && pair.Value.Trim().Equals(entityType, StringComparison.OrdinalIgnoreCase);
        }

        private static int FindEntityEnd(List<DxfPair> pairs, int startIndex)
        {
            for (int index = startIndex; index < pairs.Count; index++)
            {
                if (pairs[index].Code.Trim().Equals("0", StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }

            return pairs.Count;
        }

        private static bool ShouldConvertEntity(List<DxfPair> entityPairs, DxfConversionMode mode)
        {
            if (mode == DxfConversionMode.AllLayers)
            {
                return true;
            }

            string layerName = GetFirstValue(entityPairs, "8");
            return layerName != null && layerName.Equals(KaekLayerName, StringComparison.OrdinalIgnoreCase);
        }

        private static List<DxfPair> ConvertMTextEntity(List<DxfPair> entityPairs, ref int nextHandle)
        {
            string rawContent = ExtractMTextContent(entityPairs);
            string plainContent = StripMTextFormatting(rawContent);
            List<string> textLines = SplitPlainTextLines(plainContent);
            if (textLines.Count == 0)
            {
                textLines.Add(string.Empty);
            }

            string widthScale = ExtractWidthScale(rawContent);
            string textStyle = GetFirstValue(entityPairs, "7");
            double textHeight = GetDouble(entityPairs, "40", 1.0D);
            if (textHeight <= 0)
            {
                textHeight = 1.0D;
            }

            double lineSpacingFactor = GetDouble(entityPairs, "44", 1.0D);
            if (lineSpacingFactor <= 0)
            {
                lineSpacingFactor = 1.0D;
            }

            double insertionX = GetDouble(entityPairs, "10", 0D);
            double insertionY = GetDouble(entityPairs, "20", 0D);
            double insertionZ = GetDouble(entityPairs, "30", 0D);

            bool hasRotation;
            double rotationDegrees = GetRotationDegrees(entityPairs, out hasRotation);
            double rotationRadians = rotationDegrees * Math.PI / 180D;
            double downX = Math.Sin(rotationRadians);
            double downY = -Math.Cos(rotationRadians);
            double lineAdvance = textHeight * lineSpacingFactor;

            List<DxfPair> convertedPairs = new List<DxfPair>();
            for (int lineIndex = 0; lineIndex < textLines.Count; lineIndex++)
            {
                string handle = lineIndex == 0 ? GetFirstValue(entityPairs, "5") : null;
                if (string.IsNullOrEmpty(handle))
                {
                    handle = GenerateHandle(ref nextHandle);
                }

                double baselineOffset = textHeight + (lineIndex * lineAdvance);
                double textX = insertionX + (downX * baselineOffset);
                double textY = insertionY + (downY * baselineOffset);

                convertedPairs.Add(new DxfPair(FormatCode(0), "TEXT"));
                convertedPairs.AddRange(BuildCommonEntityPairs(entityPairs, handle));
                convertedPairs.Add(new DxfPair(FormatCode(100), "AcDbText"));
                convertedPairs.Add(new DxfPair(FormatCode(10), FormatDouble(textX)));
                convertedPairs.Add(new DxfPair(FormatCode(20), FormatDouble(textY)));
                convertedPairs.Add(new DxfPair(FormatCode(30), FormatDouble(insertionZ)));
                convertedPairs.Add(new DxfPair(FormatCode(40), FormatDouble(textHeight)));
                convertedPairs.Add(new DxfPair(FormatCode(1), textLines[lineIndex]));

                if (!string.IsNullOrEmpty(textStyle))
                {
                    convertedPairs.Add(new DxfPair(FormatCode(7), textStyle));
                }

                if (!string.IsNullOrEmpty(widthScale))
                {
                    convertedPairs.Add(new DxfPair(FormatCode(41), widthScale));
                }

                if (hasRotation && Math.Abs(rotationDegrees) > 0.0000001D)
                {
                    convertedPairs.Add(new DxfPair(FormatCode(50), FormatDouble(rotationDegrees)));
                }

                convertedPairs.Add(new DxfPair(FormatCode(11), FormatDouble(textX)));
                convertedPairs.Add(new DxfPair(FormatCode(21), FormatDouble(textY)));
                convertedPairs.Add(new DxfPair(FormatCode(31), FormatDouble(insertionZ)));
                convertedPairs.Add(new DxfPair(FormatCode(100), "AcDbText"));
            }

            return convertedPairs;
        }

        private static List<DxfPair> BuildCommonEntityPairs(List<DxfPair> entityPairs, string handle)
        {
            List<DxfPair> commonPairs = new List<DxfPair>();
            bool hasHandle = false;
            bool hasEntitySubclass = false;

            for (int index = 1; index < entityPairs.Count; index++)
            {
                DxfPair pair = entityPairs[index];
                if (pair.Code.Trim().Equals("100", StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Trim().Equals("AcDbMText", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (pair.Code.Trim().Equals("5", StringComparison.OrdinalIgnoreCase))
                {
                    commonPairs.Add(new DxfPair(pair.Code, handle));
                    hasHandle = true;
                    continue;
                }

                if (pair.Code.Trim().Equals("100", StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Trim().Equals("AcDbEntity", StringComparison.OrdinalIgnoreCase))
                {
                    hasEntitySubclass = true;
                }

                commonPairs.Add(pair.Clone());
            }

            if (!hasHandle)
            {
                commonPairs.Insert(0, new DxfPair(FormatCode(5), handle));
            }

            if (!hasEntitySubclass)
            {
                int insertIndex = hasHandle ? 1 : 0;
                commonPairs.Insert(insertIndex, new DxfPair(FormatCode(100), "AcDbEntity"));
            }

            return commonPairs;
        }

        private static string ExtractMTextContent(List<DxfPair> entityPairs)
        {
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < entityPairs.Count; index++)
            {
                string code = entityPairs[index].Code.Trim();
                if (code.Equals("1", StringComparison.OrdinalIgnoreCase)
                    || code.Equals("3", StringComparison.OrdinalIgnoreCase))
                {
                    builder.Append(entityPairs[index].Value);
                }
            }

            return builder.ToString();
        }

        private static string ExtractWidthScale(string rawContent)
        {
            Match match = Regex.Match(
                rawContent,
                @"\\[Ww]([+-]?(?:\d+(?:\.\d*)?|\.\d+));",
                RegexOptions.CultureInvariant);
            if (!match.Success)
            {
                return null;
            }

            double parsedValue;
            if (!double.TryParse(match.Groups[1].Value, NumberStyles.Float, InvariantCulture, out parsedValue))
            {
                return null;
            }

            if (parsedValue <= 0)
            {
                return null;
            }

            return match.Groups[1].Value;
        }

        private static string StripMTextFormatting(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(value.Length);
            int index = 0;
            while (index < value.Length)
            {
                char current = value[index];
                if (current == '{' || current == '}')
                {
                    index++;
                    continue;
                }

                if (current != '\\' || index == value.Length - 1)
                {
                    builder.Append(current);
                    index++;
                    continue;
                }

                char command = value[index + 1];
                switch (command)
                {
                    case 'P':
                    case 'p':
                        builder.Append(Environment.NewLine);
                        index += 2;
                        break;
                    case '~':
                        builder.Append(' ');
                        index += 2;
                        break;
                    case '\\':
                        builder.Append('\\');
                        index += 2;
                        break;
                    case 'U':
                    case 'u':
                        if (TryAppendUnicodeEscape(value, index, builder, out index))
                        {
                            break;
                        }

                        index += 2;
                        break;
                    case 'S':
                    case 's':
                        index = AppendStackedText(value, index + 2, builder);
                        break;
                    case 'A':
                    case 'a':
                    case 'C':
                    case 'c':
                    case 'F':
                    case 'f':
                    case 'H':
                    case 'h':
                    case 'Q':
                    case 'q':
                    case 'T':
                    case 't':
                    case 'W':
                    case 'w':
                        index = SkipFormatCommand(value, index + 2);
                        break;
                    case 'K':
                    case 'k':
                    case 'L':
                    case 'l':
                    case 'O':
                    case 'o':
                        index += 2;
                        break;
                    default:
                        index += 2;
                        break;
                }
            }

            return builder.ToString().Trim();
        }

        private static bool TryAppendUnicodeEscape(string value, int slashIndex, StringBuilder builder, out int nextIndex)
        {
            nextIndex = slashIndex;
            if (slashIndex + 6 >= value.Length || value[slashIndex + 2] != '+')
            {
                return false;
            }

            string hex = value.Substring(slashIndex + 3, 4);
            int codePoint;
            if (!int.TryParse(hex, NumberStyles.HexNumber, InvariantCulture, out codePoint))
            {
                return false;
            }

            builder.Append((char)codePoint);
            nextIndex = slashIndex + 7;
            return true;
        }

        private static int AppendStackedText(string value, int startIndex, StringBuilder builder)
        {
            int index = startIndex;
            while (index < value.Length && value[index] != ';')
            {
                char current = value[index];
                builder.Append(current == '^' || current == '#' ? '/' : current);
                index++;
            }

            return index < value.Length ? index + 1 : index;
        }

        private static int SkipFormatCommand(string value, int startIndex)
        {
            int index = startIndex;
            while (index < value.Length && value[index] != ';')
            {
                index++;
            }

            return index < value.Length ? index + 1 : index;
        }

        private static List<string> SplitPlainTextLines(string text)
        {
            List<string> result = new List<string>();
            string normalized = (text ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');
            string[] lines = normalized.Split(new[] { '\n' });
            for (int index = 0; index < lines.Length; index++)
            {
                string line = lines[index].Trim();
                if (line.Length > 0 || lines.Length == 1)
                {
                    result.Add(line);
                }
            }

            return result;
        }

        private static string GetFirstValue(List<DxfPair> pairs, string code)
        {
            for (int index = 0; index < pairs.Count; index++)
            {
                if (pairs[index].Code.Trim().Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    return pairs[index].Value.Trim();
                }
            }

            return null;
        }

        private static double GetDouble(List<DxfPair> pairs, string code, double defaultValue)
        {
            string value = GetFirstValue(pairs, code);
            double parsedValue;
            if (value == null || !double.TryParse(value, NumberStyles.Float, InvariantCulture, out parsedValue))
            {
                return defaultValue;
            }

            return parsedValue;
        }

        private static double GetRotationDegrees(List<DxfPair> entityPairs, out bool hasRotation)
        {
            string directionXText = GetFirstValue(entityPairs, "11");
            string directionYText = GetFirstValue(entityPairs, "21");
            double directionX;
            double directionY;
            if (directionXText != null
                && directionYText != null
                && double.TryParse(directionXText, NumberStyles.Float, InvariantCulture, out directionX)
                && double.TryParse(directionYText, NumberStyles.Float, InvariantCulture, out directionY)
                && (Math.Abs(directionX) > 0.0000001D || Math.Abs(directionY) > 0.0000001D))
            {
                hasRotation = true;
                return Math.Atan2(directionY, directionX) * 180D / Math.PI;
            }

            string rotationText = GetFirstValue(entityPairs, "50");
            double rotationDegrees;
            if (rotationText != null
                && double.TryParse(rotationText, NumberStyles.Float, InvariantCulture, out rotationDegrees))
            {
                hasRotation = true;
                return rotationDegrees;
            }

            hasRotation = false;
            return 0D;
        }

        private static int FindNextHandle(List<DxfPair> pairs)
        {
            int maxHandle = 0;
            for (int index = 0; index < pairs.Count; index++)
            {
                if (!pairs[index].Code.Trim().Equals("5", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int parsedHandle;
                if (int.TryParse(pairs[index].Value.Trim(), NumberStyles.HexNumber, InvariantCulture, out parsedHandle)
                    && parsedHandle > maxHandle)
                {
                    maxHandle = parsedHandle;
                }
            }

            return maxHandle + 1;
        }

        private static string GenerateHandle(ref int nextHandle)
        {
            string handle = nextHandle.ToString("X", InvariantCulture);
            nextHandle++;
            return handle;
        }

        private static void UpdateHandSeed(List<DxfPair> pairs, int nextHandle)
        {
            for (int index = 0; index < pairs.Count - 1; index++)
            {
                if (pairs[index].Code.Trim().Equals("9", StringComparison.OrdinalIgnoreCase)
                    && pairs[index].Value.Trim().Equals("$HANDSEED", StringComparison.OrdinalIgnoreCase)
                    && pairs[index + 1].Code.Trim().Equals("5", StringComparison.OrdinalIgnoreCase))
                {
                    pairs[index + 1] = new DxfPair(pairs[index + 1].Code, nextHandle.ToString("X", InvariantCulture));
                    return;
                }
            }
        }

        private static string BuildOutputFilePath(string sourceFilePath, DxfConversionMode mode)
        {
            string directory = Path.GetDirectoryName(sourceFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFilePath);
            string suffix = mode == DxfConversionMode.AllLayers ? "_ALL" : "_KAEK";
            string outputPath = Path.Combine(directory, fileNameWithoutExtension + suffix + ".dxf");
            if (outputPath.Equals(sourceFilePath, StringComparison.OrdinalIgnoreCase))
            {
                outputPath = Path.Combine(directory, fileNameWithoutExtension + suffix + "_converted.dxf");
            }

            return outputPath;
        }

        private static string SerializePairs(List<DxfPair> pairs, string newline)
        {
            StringBuilder builder = new StringBuilder(pairs.Count * 12);
            for (int index = 0; index < pairs.Count; index++)
            {
                builder.Append(pairs[index].Code);
                builder.Append(newline);
                builder.Append(pairs[index].Value);
                builder.Append(newline);
            }

            return builder.ToString();
        }

        private static List<DxfPair> ClonePairs(List<DxfPair> pairs)
        {
            List<DxfPair> clonedPairs = new List<DxfPair>(pairs.Count);
            for (int index = 0; index < pairs.Count; index++)
            {
                clonedPairs.Add(pairs[index].Clone());
            }

            return clonedPairs;
        }

        private static string FormatCode(int code)
        {
            return code.ToString(InvariantCulture).PadLeft(3);
        }

        private static string FormatDouble(double value)
        {
            return value.ToString("G17", InvariantCulture);
        }

        private sealed class DxfPair
        {
            public DxfPair(string code, string value)
            {
                Code = code;
                Value = value;
            }

            public string Code { get; private set; }

            public string Value { get; private set; }

            public DxfPair Clone()
            {
                return new DxfPair(Code, Value);
            }
        }
    }
}
