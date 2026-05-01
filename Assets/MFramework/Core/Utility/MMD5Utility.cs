using System;
using System.IO;
using System.Security.Cryptography;

namespace MFramework.Util
{
    public static class MMD5Utility
    {
        public static string GetMD5(string path)
        {
            if (!File.Exists(path)) return null;

            using FileStream stream = File.OpenRead(path);
            using MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
