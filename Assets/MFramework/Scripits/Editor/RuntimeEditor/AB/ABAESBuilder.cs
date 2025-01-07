using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class ABAESBuilder
    {
        public static readonly string BuildSettingPath = MSettings.ABBuildSettingName;

        public static string RootPath { get; set; }
        public static string WindowsPath { get { return $"{RootPath}/WINDOWS"; } }
        public static string AndroidPath { get { return $"{RootPath}/ANDROID"; } }
        public static string IOSPath { get { return $"{RootPath}/IOS"; } }

        internal static void EncrypABPackVersionFile()
        {
            EncryptAndCreateVersionFile();
        }

        private static void EncryptAndCreateVersionFile()
        {
            RootPath = GetBuildRootPath(BuildSettingPath);

            if (!Directory.Exists(RootPath))
            {
                MLog.Print($"{typeof(ABAESBuilder)}：<{RootPath}>未创建，请先生成AB", MLogType.Warning);
                return;
            }

            var windowsFlag = Encrypt(MBuildTarget.WINDOWS);
            var androidFlag = Encrypt(MBuildTarget.ANDROID);
            var iosFlag = Encrypt(MBuildTarget.IOS);
        }

        private static string GetBuildRootPath(string settingPath)
        {
            var buildSetting = MSerializationUtility.ReadFromXml<BuildSetting>(settingPath);
            if (buildSetting == null)
            {
                MLog.Print($"{typeof(ABBuilder)}：路径{settingPath}加载失败，请检查", MLogType.Error);
                return null;
            }
            buildSetting.GetBuildRoot();

            return buildSetting.buildRoot;
        }

        private static bool Encrypt(MBuildTarget buildTarget)
        {
            string platform = buildTarget.ToString();
            string oldRootPath = null;

            switch (buildTarget)
            {
                case MBuildTarget.WINDOWS:
                    if (!Directory.Exists(WindowsPath)) return false;
                    oldRootPath = WindowsPath;
                    break;
                case MBuildTarget.ANDROID:
                    if (!Directory.Exists(AndroidPath)) return false;
                    oldRootPath = AndroidPath;
                    break;
                case MBuildTarget.IOS:
                    if (!Directory.Exists(IOSPath)) return false;
                    oldRootPath = IOSPath;
                    break;
            }

            string newRootPath = $"{oldRootPath}_ENCRYPT";
            if (!Directory.Exists(newRootPath))
            {
                Directory.CreateDirectory(newRootPath);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(oldRootPath);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string oldPath = file.FullName.ReplaceSlash();
                string fileName = file.Name;
                //string suffixName = filePath.Substring(filePath.LastIndexOf(".") + 1);

                string path = oldPath.Substring(oldPath.IndexOf(platform) + platform.Length + 1);
                string newPath = $"{newRootPath}/{path}";

                AESManager.AESEncryptFile(oldPath, newPath);
            }

            return true;
        }
    }
}