using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MFramework
{
    public static class MMD5Utility
    {
        public static string GetMD5(string filePath)
        {
            try
            {
                byte[] retVal;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    retVal = md5.ComputeHash(fileStream);
                }

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                MLog.Print($"{typeof(MMD5Utility)}£ºMD5´´½¨Ê§°Ü<{ex.Message}>", MLogType.Error);
                throw new Exception();
            }
        }
    }
}
