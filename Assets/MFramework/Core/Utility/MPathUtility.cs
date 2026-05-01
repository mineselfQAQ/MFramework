using System;
using System.Collections.Generic;
using System.IO;

namespace MFramework.Util
{
    public static class MPathUtility
    {
        public static List<string> GetFiles(string path, string[] prefixes = null, string[] suffixes = null)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return result;

            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string normalized = file.ReplaceSlash();
                if (normalized.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)) continue;

                string fileName = Path.GetFileName(normalized);
                string extension = Path.GetExtension(normalized).ToLowerInvariant();
                if (prefixes != null && prefixes.Length > 0 && !Array.Exists(prefixes, prefix => fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))) continue;
                if (suffixes != null && suffixes.Length > 0 && !Array.Exists(suffixes, suffix => extension == suffix.ToLowerInvariant())) continue;

                result.Add(normalized);
            }

            return result;
        }

        public static void CreateFolderIfNotExist(string path, bool isFilePath = false)
        {
            string directory = isFilePath ? Path.GetDirectoryName(path) : path;
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static bool CheckFolderHaveFile(string path)
        {
            return Directory.Exists(path) && Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length > 0;
        }

        public static void DeleteFileWithExtension(string path, string extension)
        {
            if (!Directory.Exists(path)) return;

            foreach (string file in Directory.GetFiles(path, $"*{extension}", SearchOption.AllDirectories))
            {
                File.Delete(file);
            }
        }

        public static string HTTPConvert(string url)
        {
            return string.IsNullOrEmpty(url) ? url : url.Replace("\\", "/");
        }
    }
}
