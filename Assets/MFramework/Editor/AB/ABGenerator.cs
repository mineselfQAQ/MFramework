using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

using MFramework.Util;
using MFramework.Core;
namespace MFramework
{
    public class ABGenerator : EditorWindow
    {
        [MenuItem("MFramework/BuildAB _F9", priority = 222)]
        public static async void Build()
        {
            bool flag = await ABBuilder.SwitchPlatform();
            if (!flag) return;

            ABBuilder.BuildInternal(MPathCache.AB_BUILD_SETTING_PATH);
        }

        [MenuItem("MFramework/ABGenerator", priority = 221)]
        public static void Init()
        {
            ABGenerator window = GetWindow<ABGenerator>(true, "ABGenerator", false);
            window.minSize = new Vector2(200, 400);
            window.maxSize = new Vector2(200, 400);
            window.Show();
        }

        private Object obj;

        private void OnGUI()
        {
            // Tip:XML文件路径---"项目/XmlSettings/CORE/XMLBuildSetting.xml"

            MEditorGUIUtility.DrawH2("工具");
            DrawCheckXMLBtn();
            DrawCheckABBtn();

            EditorGUILayout.Space(30);

            MEditorGUIUtility.DrawH2("XML生成");
            DrawXMLGenerator();

            EditorGUILayout.Space(10);

            MEditorGUIUtility.DrawH2("构建AB包");
            DrawABGenerator();

            EditorGUILayout.Space(10);

            MEditorGUIUtility.DrawH2("AB包加密");
            DrawABAES();
        }

        private void DrawXMLGenerator()
        {
            if (GUILayout.Button("生成默认XML"))
            {
                DrawDefaultGenerator();
                MLog.Default?.D("AB debug.");
                AssetDatabase.Refresh();
            }
            EditorGUILayout.LabelField("自定义生成：");
            if (GUILayout.Button("生成"))
            {
                // TODO:自由配置存放路径/格式...
                // DrawCustomGenerator();
                MLog.Default?.W("AB warning.");
                AssetDatabase.Refresh();
            }
        }

        private void DrawABGenerator()
        {
            BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            EditorGUILayout.LabelField($"当期平台: {activeBuildTarget}");
            MBuildTarget buildTarget =
                activeBuildTarget == BuildTarget.StandaloneWindows64 ? MBuildTarget.WINDOWS :
                activeBuildTarget == BuildTarget.Android ? MBuildTarget.ANDROID : MBuildTarget.IOS;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("切换平台：", GUILayout.Width(70));

                MBuildTarget newBuildTarget = (MBuildTarget)EditorGUILayout.EnumPopup(buildTarget, GUILayout.Width(120));
                if (buildTarget != newBuildTarget)
                {
                    if (newBuildTarget == MBuildTarget.WINDOWS)
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
                    else if (newBuildTarget == MBuildTarget.ANDROID)
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    else if (newBuildTarget == MBuildTarget.IOS)
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

                    return;
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("构建"))
            {
                ABBuilder.BuildInternal(MPathCache.AB_BUILD_SETTING_PATH);
                AssetDatabase.Refresh();
            }
            // GUILayout.BeginHorizontal();
            // {
            //    //TODO：一个场景够吗？
            //    obj = EditorGUILayout.ObjectField(obj, typeof(Object), false);
            //    if （GUILayout.Button（"构建初始化"））
            //    {
            //        string objPath = AssetDatabase.GetAssetPath(obj);
            //        ABBuilder.BuildInitInternal(MPathCache.AB_BUILD_SETTING_PATH, objPath);

            //        AssetDatabase.Refresh();
            //    }
            // }
            // GUILayout.EndHorizontal();
        }

        private void DrawCheckXMLBtn()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("查看XML"))
            {
                string fileName = MPathCache.AB_BUILD_SETTING_PATH;
                if (!File.Exists(fileName))
                {
                    MLog.Default?.W("AB warning.");
                    return;
                }

                MEditorUtility.OpenFile(fileName);
            }
            // if （GUILayout.Button（"查看初始包XML"））
            // {
            //    string fileName = "CORE/AB/ABBuildInitSetting.xml";
            //    if (!File.Exists(fileName))
            //    {
            //        MLog.Default?.W（"ABBuildInitSetting.xml未创建，请先创建后再查看"）;
            //        return;
            //    }

            //    MEditorUtility.OpenFile(fileName);
            // }
            GUILayout.EndHorizontal();
        }
        private void DrawCheckABBtn()
        {
            if (GUILayout.Button("查看AB包"))
            {
                string settingPath = MPathCache.AB_BUILD_SETTING_PATH;
                if (!File.Exists(settingPath))
                {
                    MLog.Default?.W("AB warning.");
                    return;
                }

                var buildSetting = MSerializationUtil.ReadFromXml<BuildSetting>(settingPath);
                if (buildSetting == null)
                {
                    MLog.Default?.W("AB warning.");
                    return;
                }

                string resPath = buildSetting.buildRoot;
                resPath = Path.GetFullPath(resPath).ReplaceSlash();
                // 鐗规畩澶勭悊{ProjectName}
                resPath = resPath.Replace("{ProjectName}", Application.productName);
                if (!Directory.Exists(resPath))
                {
                    MLog.Default?.D("AB debug.");
                    return;
                }
                MEditorUtility.OpenFolder(resPath);
            }
        }

        private void DrawDefaultGenerator()
        {
            string defaultSavePath = Application.dataPath.CD();

            string abPath = $"{defaultSavePath}/Assets/AssetBundle";
            // CreateABDirectoryIfNotExist(abPath);

            string projectPath = Application.dataPath;
            projectPath = projectPath.Substring(0, projectPath.Length - "Assets".Length); // "Assets"之前的路径
            abPath = abPath.Replace(projectPath, ""); // 以"Assets"开头的abPath
            // CreateDefaultXML(abPath, "CORE/AB/ABBuildInitSetting.xml");
            CreateDefaultXML(abPath, MPathCache.AB_BUILD_SETTING_PATH);

            MEditorUtility.OpenFolder(MPathCache.AB_BUILD_SETTING_PATH.CD());
        }
        private void CreateABDirectoryIfNotExist(string rootPath)
        {
            Directory.CreateDirectory(rootPath);

            Directory.CreateDirectory($"{rootPath}/Common");
            Directory.CreateDirectory($"{rootPath}/Atlas");
            Directory.CreateDirectory($"{rootPath}/Background");
            Directory.CreateDirectory($"{rootPath}/Icon");
            Directory.CreateDirectory($"{rootPath}/Model");
            Directory.CreateDirectory($"{rootPath}/Shader");
            Directory.CreateDirectory($"{rootPath}/UI");
        }
        private void CreateDefaultXML(string abPath, string fileName)
        {
            string code = ABXMLCODE;

            string productName = Application.productName;
            code = code.Replace("{PROJECTNAME}", productName);
            code = code.Replace("{BUILDROOT}", $"../{productName}_AssetBundle");

            string buildItemsCode = GenerateBuildItemsCode(abPath);
            code = code.Replace("{BUILDITEM}", buildItemsCode);

            if (File.Exists(fileName))
            {
                MLog.Default?.W("AB warning.");
                return;
            }
            MSerializationUtil.SaveToFile(fileName, code);
            EditorUtility.RevealInFinder(fileName);
        }
        private string GenerateBuildItemsCode(string abPath)
        {
            StringBuilder res = new StringBuilder();

            AppendBuildItem(res, $"{abPath}/Common/",     "Direct",  "File",       ".renderTexture");
            AppendBuildItem(res, $"{abPath}/Atlas/",      "Direct",  "Directory",  ".png|.spriteatlas");
            AppendBuildItem(res, $"{abPath}/Background/", "Direct",  "File",       ".png");
            AppendBuildItem(res, $"{abPath}/Icon/",       "Direct",  "Directory",  ".png");
            AppendBuildItem(res, $"{abPath}/Model/",      "Direct",  "Directory",  ".prefab");
            AppendBuildItem(res, $"{abPath}/Shader/",     "Direct",  "Directory",  ".shader");
            AppendBuildItem(res, $"{abPath}/UI/",         "Direct",  "File",       ".prefab", false);

            return res.ToString();
        }
        private void AppendBuildItem(StringBuilder sb, string assetPath, string resourceType, string bundleType, string suffix, bool newLine = true)
        {
            string buildItemCode = $"\t{BUILDITEMCODE}";
            buildItemCode = buildItemCode.Replace("{ASSETPATH}", assetPath);
            buildItemCode = buildItemCode.Replace("{RESOURCETYPE}", resourceType);
            buildItemCode = buildItemCode.Replace("{BUNDLETYPE}", bundleType);
            buildItemCode = buildItemCode.Replace("{SUFFIX}", suffix);
            if (newLine) buildItemCode += "\n";

            sb.Append(buildItemCode);
        }

        private void DrawABAES()
        {
            if (GUILayout.Button("加密"))
            {
                ABAESBuilder.EncryptAB();
            }
            if (GUILayout.Button("解密"))
            {
                ABAESBuilder.DecryptAB();
            }
        }

        private const string ABXMLCODE =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<BuildSetting ProjectName=""{PROJECTNAME}"" BuildRoot=""{BUILDROOT}"">
{BUILDITEM}
</BuildSetting>";

        private const string BUILDITEMCODE =
@"<BuildItem AssetPath=""{ASSETPATH}"" ResourceType=""{RESOURCETYPE}"" BundleType=""{BUNDLETYPE}"" Suffix=""{SUFFIX}"" />";
    }
}
