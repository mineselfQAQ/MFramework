using System.IO;

namespace MFramework.Util
{
    public static class StringExtension
    {
        public static string CD(this string path, int count = 1)
        {
            if (string.IsNullOrEmpty(path)) return path;

            string result = path.ReplaceSlash();
            for (int i = 0; i < count; i++)
            {
                result = Path.GetDirectoryName(result)?.ReplaceSlash();
                if (string.IsNullOrEmpty(result)) break;
            }

            return result;
        }

        public static string ReplaceSlash(this string path, bool isForward = true)
        {
            if (string.IsNullOrEmpty(path)) return path;
            return isForward ? path.Replace('\\', '/') : path.Replace('/', '\\');
        }
    }
}
