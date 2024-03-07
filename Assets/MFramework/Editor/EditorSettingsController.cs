using System.IO;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class EditorSettingsController : EditorWindow
    {
        private Vector2 scrollPos;

        [MenuItem("MFramework/EditorSettingsController")]
        public static void Init()
        {
            EditorSettingsController window = GetWindow<EditorSettingsController>();
            window.minSize = new Vector2(600, 400);
            window.maxSize = new Vector2(600, 400);
            window.Show();
        }

        private void OnGUI()
        {
            //标题
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("EditorSettings", GUIStyleUtility.TitleStyle);
            EditorGUILayout.Space(5);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                EditorGUILayout.LabelField("excelGenerationPath路径：", GUIStyleUtility.BoldStyle);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField($"{EditorSettings.excelGenerationPath}");
                    if (GUILayout.Button("更改"))
                    {
                        ChangePath("excelGenerationPath");
                    }
                }
                EditorGUILayout.EndHorizontal();

                //TODO:找到所有的路径并以上述格式编写按钮
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 通过文件夹选择更换EditorSettings中的变量名
        /// </summary>
        /// <param name="originName">EditorSettings中的变量名</param>
        public static void ChangePath(string originName)
        {
            string guideFolder = Path.GetDirectoryName(Application.dataPath);
            string newPath = EditorUtility.OpenFolderPanel("请选择Excel存储路径", guideFolder, "");
            if (newPath == "")
            {
                Log.Print("取消更改", MLogType.Warning);
                return;
            }

            string editorSettingsFilePath = GetEditorSettingsFilePath();//获取EditorSettings路径
            if (editorSettingsFilePath != null)
            {
                //新位置写入
                string str = File.ReadAllText(editorSettingsFilePath);
                string newStr = ReplacePath(str, originName, newPath);
                if (newStr != null) File.WriteAllText(editorSettingsFilePath, newStr);
                else Log.Print("ChangePath：未替换成功", MLogType.Error);

                AssetDatabase.Refresh();
            }
            else
            {
                Log.Print("ChangePath：未找到路径", MLogType.Error);
            }
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
                    return str.Replace(oldPath, newPath);
                }
                i++;
            }
            return null;
        }
        private static string GetEditorSettingsFilePath()
        {
            string[] guids = AssetDatabase.FindAssets("EditorSettings");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                //Tip：FindAssets()只要名字部分匹配即可,如"EditorSettings2"也可以匹配，需要重新验证名字
                if (obj && obj.name == "EditorSettings")
                {
                    string resPath = AssetDatabase.GetAssetPath(obj);
                    return resPath;
                }
            }
            return null;
        }
    }
}