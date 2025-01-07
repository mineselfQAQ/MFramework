using System.Security.Cryptography;
using System.IO;
using System;

namespace MFramework
{
    public class AESManager : Singleton<AESManager>
    {
        private static AesCryptoServiceProvider AESCSP;

        //Key��IV(����)  ע�⣺�ṩ**ASCII�ַ�**
        private const string KEY = "mineself0817QAQx";
        private const string IV = "xQAQ0817mineself";

        public static int AESEncryptFile(string filePath, string outputPath, string key = KEY, string iv = IV)
        {
            if (!File.Exists(filePath))
            {
                MLog.Print($"{typeof(AESManager)}��������AES���ܵ��ļ�������", MLogType.Warning);
                return -1;
            }

            int keyCount = key.Length;
            int ivCount = iv.Length;
            if (keyCount < 7 || keyCount > 16 || ivCount < 7 || ivCount > 16)
            {
                MLog.Print($"{typeof(AESManager)}��AES������ԿsKey��sIV���ȱ�����8��16λ");
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
            //�Ѵ������贴��
            if (AESCSP != null)
            {
                return AESCSP;
            }

            //Tip��ASCII�ַ�Ϊ1bytes����ô16������һ��<16bytes
            byte[] keyBytes = System.Text.Encoding.ASCII.GetBytes(key);
            byte[] ivBytes = System.Text.Encoding.ASCII.GetBytes(iv);
            //��Ҫ128bits��Key��IV(��Provider����)
            byte[] keyBytes2 = new byte[16];
            byte[] ivBytes2 = new byte[16];
            Array.Copy(keyBytes, keyBytes2, keyBytes.Length);
            Array.Copy(ivBytes, ivBytes2, ivBytes.Length);

            //����Provider
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
        /// ���ܻ����AES�ļ�
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

                            //TODO���ļ���д���ļ�����
                            //·����һ��������Ҫ����(FileMode.OpenOrCreate����)
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
                MLog.Print($"{typeof(AESManager)}��AES����ʧ��---{ex.Message}", MLogType.Warning);
                return -1;
            }
        }
    }
}
