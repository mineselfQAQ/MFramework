using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using static MFramework.MConfigurableSettingsBase;

namespace MFramework
{
    public class EditorSettingsConfigurator : EditorWindow
    {
        private Vector2 scrollPos1;
        private Vector2 scrollPos2;

        [MenuItem("MFramework/EditorSettingsConfigurator", false, 101)]
        public static void Init()
        {
            EditorSettingsConfigurator window = GetWindow<EditorSettingsConfigurator>();
            window.minSize = new Vector2(600, 300);
            window.maxSize = new Vector2(1000, 300);
            window.Show();
        }

        private void OnGUI()
        {
            //==========标题==========
            MEditorGUIUtility.DrawH1("编辑器配置器");

            //==========Excel==========
            MEditorGUIUtility.DrawH2("Excel部分");
            //TODO:使用下拉列表选择需要显示的部分(Excel部分/Json部分)，并显示相应内容(节省空间)
            scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1);
            {
                //------已废弃，使用MSettings中的固定路径------
                DrawPathWidget("Excel表生成路径：", MConfigurableSettings.ExcelPath,
                    GetPathName(MConfigurableName.ExcelGenerationPath));
                DrawPathWidget("Excel表CS文件生成路径：", MConfigurableSettings.ExcelCSPath,
                    GetPathName(MConfigurableName.ExcelCSGenerationPath));
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            //==========功能==========
            EditorGUILayout.BeginHorizontal();
            {
                DrawResetBtn();
                DrawCheckCSBtn();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            MEditorGUIUtility.DrawH2("Bool值");
            DrawEnableCheckMCoreExistBool();

            EditorGUILayout.Space(5);
        }

        private void DrawPathWidget(string title, string path, string originName)
        {
            EditorGUILayout.LabelField(title, MEditorGUIStyleUtility.BoldStyle);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(path);
                //GUILayout.FlexibleSpace();
                if (GUILayout.Button("查看", GUILayout.Width(50)))
                {
                    System.Diagnostics.Process.Start(path);
                }
                if (GUILayout.Button("更改", GUILayout.Width(50)))
                {
                    ChangePath(originName);
                    AssetDatabase.Refresh();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawResetBtn()
        {
            if (GUILayout.Button("重置为默认设置"))
            {
                EnsureFolderExist();
                RebuildAllEditorSettings();

                AssetDatabase.Refresh();
            }
        }
        private void RebuildAllEditorSettings()
        {
            string editorSettingsFilePath = GetEditorSettingsFilePath();//获取EditorSettings路径

            string code = EDITORSETTINGSCODE;

            string settings = GenerateSettings();
            code = code.Replace("{Settings}", settings);

            if (editorSettingsFilePath != null)
            {
                File.WriteAllText(editorSettingsFilePath, code);
            }
            else
            {
                MLog.Print($"未找到EditorSettings文件，现在请选择文件夹重新创建", MLogType.Warning);
                string newDirectoryPath = MEditorUtility.ChangePath();
                string newFilePath = Path.Combine(newDirectoryPath, "MConfigurableSettings.cs");
                File.WriteAllText(newFilePath, code);
            }
        }

        private void EnsureFolderExist()
        {
            foreach (var pair in pathDic)
            {
                MPathUtility.CreateFolderIfNotExist(pair.Value);
            }
        }

        private string GenerateSettings()
        {
            StringBuilder res = new StringBuilder();

            foreach (var pair in pathDic)
            {
                string tempLine = SETTINGSBASECODE;
                tempLine = tempLine.Replace("{ConstantName}", pair.Key);
                tempLine = tempLine.Replace("{Path}", pair.Value);

                res.Append(tempLine + "\n\t");
            }
            string resStr = res.ToString();
            resStr = resStr.TrimEnd('\t', '\n');

            return resStr;
        }

        private void DrawCheckCSBtn()
        {
            if (GUILayout.Button("查看EditorSettings脚本"))
            {
                string editorSettingsFilePath = GetEditorSettingsFilePath();
                if (editorSettingsFilePath == null)
                {
                    MLog.Print("EditorSettings脚本不存在，请检查", MLogType.Warning);
                    return;
                }
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(editorSettingsFilePath, 3);
            }
        }

        private void DrawEnableCheckMCoreExistBool()
        {
            EditorGUILayout.BeginHorizontal();
            {
                bool flag = EditorPrefs.GetBool(EditorPrefsData.EnableCheckMCore, true);
                EditorGUILayout.LabelField($"是否强制添加MCore:  {flag}");
                if (GUILayout.Button("更改"))
                {
                    EditorPrefs.SetBool(EditorPrefsData.EnableCheckMCore, !flag);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool ResetPath(string originName, string newPath)
        {
            string editorSettingsFilePath = GetEditorSettingsFilePath();//获取EditorSettings路径
            if (editorSettingsFilePath != null)
            {
                //新位置写入
                string str = File.ReadAllText(editorSettingsFilePath);
                string newStr = ReplacePath(str, originName, newPath);
                if (newStr != null) File.WriteAllText(editorSettingsFilePath, newStr);
                else { MLog.Print("未替换成功", MLogType.Error); return false; }
            }
            else
            {
                MLog.Print("未找到路径", MLogType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 通过文件夹选择更换EditorSettings中的变量名
        /// </summary>
        /// <param playerName="originName">EditorSettings中的变量名</param>
        public static bool ChangePath(string originName)
        {
            string guideFolder = Path.GetDirectoryName(Application.dataPath);
            string newPath = EditorUtility.OpenFolderPanel("请选择Excel存储路径", guideFolder, "");
            if (newPath == "")
            {
                MLog.Print("取消更改", MLogType.Warning);
                return false;
            }

            string editorSettingsFilePath = GetEditorSettingsFilePath();//获取EditorSettings路径
            if (editorSettingsFilePath != null)
            {
                //新位置写入
                string str = File.ReadAllText(editorSettingsFilePath);
                string newStr = ReplacePath(str, originName, newPath);
                if (newStr != null) File.WriteAllText(editorSettingsFilePath, newStr);
                else { MLog.Print("未替换成功", MLogType.Error); return false; }

                AssetDatabase.Refresh();
            }
            else
            {
                MLog.Print("未找到路径", MLogType.Error);
                return false;
            }
            return true;
        }

        private static string ReplacePath(string str, string originName, string newPath)
        {
            char initials = originName[0];
            string oldPath = "";
            int i = 0;
            bool flag = false;

            //遍历整个字符串
            while (i < str.Length)
            {
                //发现与originName首字符匹配的索引
                if (str[i] == initials)
                {
                    //判断是否与originName完全一致
                    flag = true;
                    for (int j = 1; j < originName.Length; j++)
                    {
                        if (str[j + i] != originName[j])
                        {
                            flag = false;
                            break;
                        }
                    }
                }

                //找到originName后
                if (flag)
                {
                    i += originName.Length;//将i放置在变量名后
                    int firstIndex;//首字母索引

                    while (i < str.Length)
                    {
                        //寻找左引号
                        if (str[i] == '\"')
                        {
                            firstIndex = i + 1;
                            int count = 0, index = i + 1;
                            while (str[index++] != '\"') count++;//统计左引号到右引号之间的数量
                            oldPath = str.Substring(firstIndex, count);//获取路径
                            break;
                        }
                        i++;
                    }
                    //添加前缀(防止路径一致导致全部替换情况)
                    oldPath = $@"{originName} = @""{oldPath}""";
                    newPath = $@"{originName} = @""{newPath}""";
                    return str.Replace(oldPath, newPath);
                }
                i++;
            }
            return null;
        }

        private static string GetEditorSettingsFilePath()
        {
            string[] guids = AssetDatabase.FindAssets("MConfigurableSettings");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                //Tip：FindAssets()只要名字部分匹配即可,如"EditorSettings2"也可以匹配，需要重新验证名字
                if (obj && obj.name == "MConfigurableSettings")
                {
                    string resPath = AssetDatabase.GetAssetPath(obj);
                    return resPath;
                }
            }
            return null;
        }

        private const string EDITORSETTINGSCODE =
@"public static class MConfigurableSettings
{
    {Settings}
}";
        private const string SETTINGSBASECODE = "public const string {ConstantName} = @\"{Path}\";";
    }
}