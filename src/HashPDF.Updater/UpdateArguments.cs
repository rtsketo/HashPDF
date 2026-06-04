using System;
using System.Collections.Generic;
using System.Text;

namespace HashPDF.Updater
{
    internal sealed class UpdateArguments
    {
        public int ParentProcessId { get; private set; }

        public string InstallDirectory { get; private set; }

        public string TargetVersion { get; private set; }

        public string TagName { get; private set; }

        public string Language { get; private set; }

        public bool Elevated { get; private set; }

        public static bool TryParse(string[] args, out UpdateArguments result)
        {
            Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bool elevated = false;

            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                if (key.Equals("--elevated", StringComparison.OrdinalIgnoreCase))
                {
                    elevated = true;
                    continue;
                }

                if (!key.StartsWith("--", StringComparison.Ordinal))
                {
                    result = null;
                    return false;
                }

                if (i + 1 >= args.Length)
                {
                    result = null;
                    return false;
                }

                values[key.Substring(2)] = args[++i];
            }

            int parentProcessId;
            if (!values.ContainsKey("parent-pid")
                || !int.TryParse(values["parent-pid"], out parentProcessId)
                || parentProcessId < 0
                || !values.ContainsKey("install-dir")
                || string.IsNullOrEmpty(values["install-dir"])
                || !values.ContainsKey("version")
                || string.IsNullOrEmpty(values["version"])
                || !values.ContainsKey("tag")
                || string.IsNullOrEmpty(values["tag"]))
            {
                result = null;
                return false;
            }

            result = new UpdateArguments();
            result.ParentProcessId = parentProcessId;
            result.InstallDirectory = values["install-dir"];
            result.TargetVersion = values["version"];
            result.TagName = values["tag"];
            result.Language = values.ContainsKey("language") ? values["language"] : "greek";
            result.Elevated = elevated;
            return true;
        }

        public string ToCommandLine(bool elevated)
        {
            List<string> parts = new List<string>();
            AddArgument(parts, "--parent-pid", ParentProcessId.ToString());
            AddArgument(parts, "--install-dir", InstallDirectory);
            AddArgument(parts, "--version", TargetVersion);
            AddArgument(parts, "--tag", TagName);
            AddArgument(parts, "--language", Language);
            if (elevated)
            {
                parts.Add("--elevated");
            }

            return string.Join(" ", parts.ToArray());
        }

        private static void AddArgument(List<string> parts, string key, string value)
        {
            parts.Add(key);
            parts.Add(Quote(value));
        }

        private static string Quote(string value)
        {
            if (value == null)
            {
                return "\"\"";
            }

            StringBuilder builder = new StringBuilder();
            builder.Append('"');
            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                if (character == '\\' || character == '"')
                {
                    builder.Append('\\');
                }

                builder.Append(character);
            }

            builder.Append('"');
            return builder.ToString();
        }
    }
}
