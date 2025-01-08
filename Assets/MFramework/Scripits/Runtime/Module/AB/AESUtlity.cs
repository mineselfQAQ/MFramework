using System.Security.Cryptography;
using System.IO;
using System;
using System.Linq;

namespace MFramework
{
    public class AESUtlity
    {
        private static AesCryptoServiceProvider AESCSP;

        //默认Key与IV(向量)  注意：提供**ASCII字符**(因为1个1byte)
        private const string KEY = "mineself0817QAQx";
        private const string IV = "xQAQ0817mineself";

        public static int AESEncryptFile(string filePath, string outputPath, string key = KEY, string iv = IV)
        {
            CreateAESCSP(key, iv);
            ICryptoTransform trans = AESCSP.CreateEncryptor();

            return AESFile(trans, filePath, outputPath, key, iv);
        }
        public static int AESDecryptFile(string filePath, string outputPath, string key = KEY, string iv = IV)
        {
            CreateAESCSP(key, iv);
            ICryptoTransform trans = AESCSP.CreateDecryptor();

            return AESFile(trans, filePath, outputPath, key, iv);
        }
        public static byte[] AESDecryptFileToStream(string filePath, string key = KEY, string iv = IV)
        {
            CreateAESCSP(key, iv);
            ICryptoTransform trans = AESCSP.CreateDecryptor();

            return AESFileToStream(trans, filePath, key, iv);
        }

        private static byte[] AESFileToStream(ICryptoTransform trans, string filePath, string key = KEY, string iv = IV)
        {
            if (!File.Exists(filePath))
            {
                MLog.Print($"{typeof(AESUtlity)}：不存在AES加密的文件，请检查", MLogType.Warning);
                return null;
            }

            int keyCount = key.Length;
            int ivCount = iv.Length;
            if (keyCount < 7 || keyCount > 16 || ivCount < 7 || ivCount > 16)
            {
                MLog.Print($"{typeof(AESUtlity)}：AES错误，Key与IV长度必须是8到16位");
                return null;
            }

            return AESDecryptStream(filePath, trans);
        }

        private static byte[] AESDecryptStream(string filePath, ICryptoTransform trans)
        {
            try
            {
                byte[] outputBytes = null;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] inputBytes = null;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, trans, CryptoStreamMode.Write))
                        {
                            using (BinaryReader binaryReader = new BinaryReader(fileStream))
                            {
                                inputBytes = new byte[fileStream.Length];
                                binaryReader.Read(inputBytes, 0, inputBytes.Length);
                            }

                            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                            cryptoStream.FlushFinalBlock();

                            outputBytes = memoryStream.ToArray();
                        }
                    }
                }

                return outputBytes;
            }
            catch (Exception ex)
            {
                MLog.Print($"{typeof(AESUtlity)}：AES加密失败---{ex.Message}", MLogType.Warning);
                return null;
            }
        }


        private static int AESFile(ICryptoTransform trans, string filePath, string outputPath, string key = KEY, string iv = IV)
        {
            if (!File.Exists(filePath))
            {
                MLog.Print($"{typeof(AESUtlity)}：不存在AES加密的文件，请检查", MLogType.Warning);
                return -1;
            }

            int keyCount = key.Length;
            int ivCount = iv.Length;
            if (keyCount < 7 || keyCount > 16 || ivCount < 7 || ivCount > 16)
            {
                MLog.Print($"{typeof(AESUtlity)}：AES错误，Key与IV长度必须是8到16位");
                return -1;
            }

            if (AESEncryptOrDecrypt(filePath, outputPath, trans) == -1)
            {
                return -1;
            }

            return 1;
        }

        private static AesCryptoServiceProvider CreateAESCSP(string key, string iv)
        {
            //已存在无需创建
            if (AESCSP != null)
            {
                return AESCSP;
            }

            //Tip：ASCII字符为1bytes，那么16个以内一定<16bytes
            byte[] keyBytes = System.Text.Encoding.ASCII.GetBytes(key);
            byte[] ivBytes = System.Text.Encoding.ASCII.GetBytes(iv);
            //需要128bits的Key与IV(由Provider定义)
            byte[] keyBytes2 = new byte[16];
            byte[] ivBytes2 = new byte[16];
            Array.Copy(keyBytes, keyBytes2, keyBytes.Length);
            Array.Copy(ivBytes, ivBytes2, ivBytes.Length);

            //创建Provider
            AESCSP = new AesCryptoServiceProvider()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes2,
                IV = ivBytes2,
            };

            return AESCSP;
        }

        /// <summary>
        /// 加密或解密AES文件
        /// </summary>
        private static int AESEncryptOrDecrypt(string filePath, string outputPath, ICryptoTransform trans)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] inputBytes = null;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, trans, CryptoStreamMode.Write))
                        {
                            using (BinaryReader binaryReader = new BinaryReader(fileStream))
                            {
                                inputBytes = new byte[fileStream.Length];
                                binaryReader.Read(inputBytes, 0, inputBytes.Length);
                            }

                            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                            cryptoStream.FlushFinalBlock();

                            //路径不一定存在需要创建(FileMode.OpenOrCreate不行(Create指的是文件创建而不是前面的文件夹))
                            string name = Path.GetFileName(outputPath);
                            //排除WINDOWS之类的文件(无后缀，特殊处理)
                            var targets = new MBuildTarget[] { MBuildTarget.WINDOWS, MBuildTarget.ANDROID, MBuildTarget.IOS };
                            if (!targets.Any(target => name.Contains(target.ToString())))
                            {
                                MPathUtility.CreateFolderIfNotExist(outputPath);
                            }

                            using (FileStream fileStream2 = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write))
                            {
                                memoryStream.WriteTo(fileStream2);
                            }
                        }
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                MLog.Print($"{typeof(AESUtlity)}：AES加密失败---{ex.Message}", MLogType.Warning);
                return -1;
            }
        }
    }
}
