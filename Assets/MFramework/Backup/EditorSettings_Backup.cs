using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class EditorSettings_Backup : ScriptableObject
    {
        private const string k_Path = @"Assets\MFramework\Excel\ExcelSettings.asset";

        private static EditorSettings_Backup ms_Instance = null;
        public static EditorSettings_Backup Instance
        {
            get
            {
                if (ms_Instance == null)
                {
                    ms_Instance = AssetDatabase.LoadAssetAtPath<EditorSettings_Backup>(k_Path);
                    if (ms_Instance == null) 
                    {
                        ms_Instance = CreateInstance<EditorSettings_Backup>();
                        AssetDatabase.CreateAsset(ms_Instance, k_Path);
                        AssetDatabase.Refresh();
                    }
                }
                return ms_Instance;
            }
        }

        [MenuItem("MFramework/FindEditorSettings")]
        public static void Find() 
        {
            Selection.activeObject = Instance;
        }

        //Tip：请使用'\'而非'/'以保证在Windows下的正确运行
        //==========Excel==========
        public string excelGenerationPath;//excel生成路径(不包含文件名)
    }
}