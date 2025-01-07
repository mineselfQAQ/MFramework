using System.Security.Cryptography;
using System.IO;
using System;

namespace MFramework
{
    public class AESManager : Singleton<AESManager>
    {
        private static AesCryptoServiceProvider AESCSP;

        //Key与IV(向量)  注意：提供**ASCII字符**
        private const string KEY = "mineself0817QAQx";
        private const string IV = "xQAQ0817mineself";

        public static int AESEncryptFile(string filePath, string outputPath, string key = KEY, string iv = IV)
        {
            if (!File.Exists(filePath))
            {
                MLog.Print($"{typeof(AESManager)}：不存在AES加密的文件，请检查", MLogType.Warning);
                return -1;
            }

            int keyCount = key.Length;
            int ivCount = iv.Length;
            if (keyCount < 7 || keyCount > 16 || ivCount < 7 || ivCount > 16)
            {
                MLog.Print($"{typeof(AESManager)}：AES错误，秘钥sKey与sIV长度必须是8到16位");
                return -1;
            }

            AesCryptoServiceProvider aesCSP = CreateAESCSP(key, iv);
            ICryptoTransform obj_trans = aesCSP.CreateEncryptor();
            if (AESEncryptOrDecrypt(filePath, outputPath, obj_trans) == -1)
            {
                return -1;
            }

            return 1;
        }

        public static AesCryptoServiceProvider CreateAESCSP(string key, string iv)
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
                            using (BinaryReader obj_binaryReader = new BinaryReader(fileStream))
                            {
                                inputBytes = new byte[fileStream.Length];
                                obj_binaryReader.Read(inputBytes, 0, inputBytes.Length);
                            }

                            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
                            cryptoStream.FlushFinalBlock();

                            //TODO：文件被写成文件夹了
                            //路径不一定存在需要创建(FileMode.OpenOrCreate不行)
                            MPathUtility.CreateFolderIfNotExist(outputPath);
                            using (FileStream fileStream2 = new FileStream(outputPath, FileMode.Open, FileAccess.Write))
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
                MLog.Print($"{typeof(AESManager)}：AES加密失败---{ex.Message}", MLogType.Warning);
                return -1;
            }
        }
    }
}
