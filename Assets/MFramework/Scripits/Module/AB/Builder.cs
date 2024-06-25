using UnityEditor;
using System.IO;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// AB包构建核心类
    /// </summary>
    public class Builder
    {
        [MenuItem("MFramework/BuildAssetBundle", false, 201)]
        public static void Build()
        {
            BuildInternal();
        }

        private static readonly Profiler ms_BuildProfiler = new Profiler(nameof(Builder));
        private static readonly Profiler ms_LoadBuildSettingProfiler = ms_BuildProfiler.CreateChild(nameof(LoadSetting));

        public static BuildTarget Platform 
        {
            get { return EditorUserBuildSettings.activeBuildTarget; }
        }

        /// <summary>
        /// 打包配置
        /// </summary>
        public readonly static string BuildSettingPath = Path.GetFullPath("BuildSetting.xml").Replace("\\", "/");
        
        /// <summary>
        /// 打包设置
        /// </summary>
        public static BuildSetting buildSetting { get; private set; }

        /// <summary>
        /// 打包目录
        /// </summary>
        public static string buildPath { get; set; }

        private static void BuildInternal()
        {
            SwitchPlatform();

            ms_BuildProfiler.Start();

            ms_LoadBuildSettingProfiler.Start();
            Debug.Log(BuildSettingPath);
            buildSetting = LoadSetting(BuildSettingPath);
            ms_LoadBuildSettingProfiler.Stop();

            ////搜集bundle信息
            //ms_CollectProfiler.Start();
            //Dictionary<string, List<string>> bundleDic = Collect();
            //ms_CollectProfiler.Stop();

            ////打包assetbundle
            //ms_BuildBundleProfiler.Start();
            //BuildBundle(bundleDic);
            //ms_BuildBundleProfiler.Stop();

            ////清空多余文件
            //ms_ClearBundleProfiler.Start();
            //ClearAssetBundle(buildPath, bundleDic);
            //ms_ClearBundleProfiler.Stop();

            ////把描述文件打包bundle
            //ms_BuildManifestBundleProfiler.Start();
            //BuildManifest();
            //ms_BuildManifestBundleProfiler.Stop();

            //EditorUtility.ClearProgressBar();

            ms_BuildProfiler.Stop();

            MLog.Print($"打包完成{ms_BuildProfiler}");
        }

        public static void SwitchPlatform()
        {
            int platformInt = EditorUtility.DisplayDialogComplex("Switch Platform",
            "请选择平台：", "Windows", "iOS", "Android");

            if (platformInt == 0)//Windows
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64) return;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            }
            else if (platformInt == 1)//iOS
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) return;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            }
            else if (platformInt == 2)//Android
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) return;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }
        }

        private static BuildSetting LoadSetting(string settingPath)
        {
            buildSetting = MSerializationUtility.ReadFromXml<BuildSetting>(settingPath);
            if (buildSetting == null)
            {
                MLog.Print($"{typeof(Builder)}：路径{settingPath}加载失败，请检查", MLogType.Warning);
                return null;
            }
            buildSetting.Init();

            buildPath = Path.GetFullPath(buildSetting.buildRoot).Replace("\\", "/");
            if (buildPath.Length > 0 && buildPath[buildPath.Length - 1] != '/')
            {
                buildPath += "/";
            }
            buildPath += $"{GetPlatform()}/";

            return buildSetting;
        }
        private static string GetPlatform()
        {
            switch (Platform)
            {
                case BuildTarget.StandaloneWindows64:
                    return "WINDOWS";
                case BuildTarget.Android:
                    return "ANDROID";
                case BuildTarget.iOS:
                    return "IOS";
                default:
                    return null;
            }
        }
    }
}