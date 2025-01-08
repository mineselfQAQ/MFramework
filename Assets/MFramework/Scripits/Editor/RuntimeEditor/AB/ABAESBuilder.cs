using System.IO;
using System.Text;
using UnityEngine;

namespace MFramework
{
    public class ABAESBuilder
    {
        public static string RootPath { get; set; }
        public static string WindowsPath { get { return $"{RootPath}/WINDOWS"; } }
        public static string AndroidPath { get { return $"{RootPath}/ANDROID"; } }
        public static string IOSPath { get { return $"{RootPath}/IOS"; } }
        public static string WindowsEncryptPath { get { return $"{RootPath}/WINDOWS_ENCRYPT"; } }
        public static string AndroidEncryptPath { get { return $"{RootPath}/ANDROID_ENCRYPT"; } }
        public static string IOSEncryptPath { get { return $"{RootPath}/IOS_ENCRYPT"; } }
        public static string WindowsDecryptPath { get { return $"{RootPath}/WINDOWS_DECRYPT"; } }
        public static string AndroidDecryptPath { get { return $"{RootPath}/ANDROID_DECRYPT"; } }
        public static string IOSDecryptPath { get { return $"{RootPath}/IOS_DECRYPT"; } }

        internal static void EncryptAB()
        {
            if(RootPath == null) RootPath = ABEditorUtility.GetBuildRootPath();
            if (!Directory.Exists(RootPath))
            {
                MLog.Print($"{typeof(ABAESBuilder)}：<{RootPath}>未创建，请先生成AB", MLogType.Warning);
                return;
            }

            EncryptAll();
        }
        internal static void DecryptAB()
        {
            if (RootPath == null) RootPath = ABEditorUtility.GetBuildRootPath();
            if (!Directory.Exists(RootPath))
            {
                MLog.Print($"{typeof(ABAESBuilder)}：<{RootPath}>未创建，请先生成AB", MLogType.Warning);
                return;
            }

            DecryptAll();
        }

        /// <returns>是否有加密(任意平台)</returns>
        private static bool EncryptAll()
        {
            var windowsFlag = Encrypt(MBuildTarget.WINDOWS);
            var androidFlag = Encrypt(MBuildTarget.ANDROID);
            var iosFlag = Encrypt(MBuildTarget.IOS);

            if (windowsFlag || androidFlag || iosFlag) return true;
            return false;
        }
        private static bool DecryptAll()
        {
            var windowsFlag = Decrypt(MBuildTarget.WINDOWS);
            var androidFlag = Decrypt(MBuildTarget.ANDROID);
            var iosFlag = Decrypt(MBuildTarget.IOS);

            if (windowsFlag || androidFlag || iosFlag) return true;
            return false;
        }

        private static bool Encrypt(MBuildTarget buildTarget)
        {
            string platform = buildTarget.ToString();
            string oldRootPath = null;
            string newRootPath = null;

            switch (buildTarget)
            {
                case MBuildTarget.WINDOWS:
                    if (!Directory.Exists(WindowsPath)) return false;
                    oldRootPath = WindowsPath;
                    newRootPath = WindowsEncryptPath;
                    break;
                case MBuildTarget.ANDROID:
                    if (!Directory.Exists(AndroidPath)) return false;
                    oldRootPath = AndroidPath;
                    newRootPath = AndroidEncryptPath;
                    break;
                case MBuildTarget.IOS:
                    if (!Directory.Exists(IOSPath)) return false;
                    oldRootPath = IOSPath;
                    newRootPath = WindowsEncryptPath;
                    break;
            }

            if (!Directory.Exists(newRootPath))
            {
                Directory.CreateDirectory(newRootPath);
            }
            else//已存在需要删除重建
            {
                Directory.Delete(newRootPath, true);
                Directory.CreateDirectory(newRootPath);
                MLog.Print($"{typeof(ABAESBuilder)}.{nameof(Encrypt)}：{platform}平台删除，开始重建...");
            }

            StringBuilder sb = new StringBuilder();
            DirectoryInfo directoryInfo = new DirectoryInfo(oldRootPath);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string oldPath = file.FullName.ReplaceSlash();
                string fileName = file.Name;
                string suffixName = oldPath.Substring(oldPath.LastIndexOf(".") + 1);

                string path = oldPath.Substring(oldPath.IndexOf(platform) + platform.Length + 1);
                string newPath = $"{newRootPath}/{path}";

                //对于非.ab文件，不加密直接拷贝
                if (suffixName == "ab")
                {
                    AESUtlity.AESEncryptFile(oldPath, newPath);
                }
                else
                {
                    File.Copy(oldPath, newPath, true);
                }

                //Tip：获取如XXX_AssetBundle文件夹名(存放该项目的AB的根)，与XML中BuildRoot有关
                string abRootName = RootPath.Substring(RootPath.LastIndexOf('/') + 1);
                string fullFileName = newPath.Substring(newPath.IndexOf(abRootName));
                //MD5
                string md5 = MMD5Utility.GetMD5(newPath);
                if (string.IsNullOrEmpty(md5))
                {
                    MLog.Print($"{typeof(ABAESBuilder)}：MD5获取失败，文件<{newPath}>");
                }
                //文件大小
                string size = Mathf.Ceil(file.Length / 1024f).ToString();

                string fileData = $"{fullFileName} {md5} {size}";
                sb.AppendLine(fileData);

            }
            File.WriteAllText($"{newRootPath}/{MSettings.ABInfoFileName}", sb.ToString());

            MLog.Print($"{typeof(ABAESBuilder)}：{platform}平台AB加密生成完成");
            return true;
        }
        private static bool Decrypt(MBuildTarget buildTarget)
        {
            string platform = buildTarget.ToString();
            string oldRootPath = null;
            string newRootPath = null;

            switch (buildTarget)
            {
                case MBuildTarget.WINDOWS:
                    if (!Directory.Exists(WindowsEncryptPath)) return false;
                    oldRootPath = WindowsEncryptPath;
                    newRootPath = WindowsDecryptPath;
                    break;
                case MBuildTarget.ANDROID:
                    if (!Directory.Exists(AndroidEncryptPath)) return false;
                    oldRootPath = AndroidEncryptPath;
                    newRootPath = AndroidDecryptPath;
                    break;
                case MBuildTarget.IOS:
                    if (!Directory.Exists(IOSEncryptPath)) return false;
                    oldRootPath = IOSEncryptPath;
                    newRootPath = IOSDecryptPath;
                    break;
            }

            if (!Directory.Exists(newRootPath))
            {
                Directory.CreateDirectory(newRootPath);
            }
            else//已存在需要删除重建
            {
                Directory.Delete(newRootPath, true);
                Directory.CreateDirectory(newRootPath);
                MLog.Print($"{typeof(ABAESBuilder)}.{nameof(Decrypt)}：{platform}平台删除，开始重建...");
            }

            StringBuilder sb = new StringBuilder();
            DirectoryInfo directoryInfo = new DirectoryInfo(oldRootPath);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string oldPath = file.FullName.ReplaceSlash();
                string fileName = file.Name;
                string suffixName = oldPath.Substring(oldPath.LastIndexOf(".") + 1);

                string path = oldPath.Substring(oldPath.IndexOf(platform) + $"{platform}_Encrypt".Length + 1);
                string newPath = $"{newRootPath}/{path}";

                //MD5文件无需出现在此处
                if (fileName == MSettings.ABInfoFileName) continue;

                //对于非.ab文件，不解密直接拷贝
                if (suffixName == "ab")
                {
                    AESUtlity.AESDecryptFile(oldPath, newPath);
                }
                else
                {
                    File.Copy(oldPath, newPath, true);
                }

                //重新生成未加密时的MD5文件
                //Tip：获取如XXX_AssetBundle文件夹名(存放该项目的AB的根)，与XML中BuildRoot有关
                string abRootName = RootPath.Substring(RootPath.LastIndexOf('/') + 1);
                string fullFileName = newPath.Substring(newPath.IndexOf(abRootName));
                //MD5
                string md5 = MMD5Utility.GetMD5(newPath);
                if (string.IsNullOrEmpty(md5))
                {
                    MLog.Print($"{typeof(ABAESBuilder)}：MD5获取失败，文件<{newPath}>");
                }
                //文件大小
                string size = Mathf.Ceil(file.Length / 1024f).ToString();

                string fileData = $"{fullFileName} {md5} {size}";
                sb.AppendLine(fileData);
            }
            File.WriteAllText($"{newRootPath}/{MSettings.ABInfoFileName}", sb.ToString());

            MLog.Print($"{typeof(ABAESBuilder)}：{platform}平台AB解密生成完成");
            return true;
        }
    }
}