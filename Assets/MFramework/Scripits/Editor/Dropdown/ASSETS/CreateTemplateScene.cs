using System.IO;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;

namespace MFramework
{
    public class CreateTemplateScene
    {
        [MenuItem("Assets/MCreate/TemplateScene/UITemplateScene", priority = 1, secondaryPriority = 1.0f)]
        public static void CreateUITemplateScene()
        {
            if (CheckAvailability())
            {
                SceneTemplateAsset sceneTemplate = AssetDatabase.LoadAssetAtPath<SceneTemplateAsset>(EditorResourcesPath.SceneFilePath);

                if (sceneTemplate == null)
                {
                    MLog.Print($"路径{EditorResourcesPath.SceneFilePath}未找到UI场景模板，无法创建", MLogType.Warning);
                    return;
                }

                string path = GetFilePath("UITemplateScene.unity");
                if (File.Exists(path))
                {
                    bool flag = EditorUtility.DisplayDialog("警告", 
                        $"{path}处已存在同名场景，是否需要覆盖", "覆盖", "取消");
                    if (!flag) return;
                }
                SceneTemplateService.Instantiate(sceneTemplate, false, path);
            }
        }

        private static string GetFilePath(string fileName)
        {
            Object obj = Selection.objects[0];
            string path = AssetDatabase.GetAssetPath(obj);
            string directoryPath;
            if (File.Exists(path))//选择的是文件
            {
                directoryPath = Path.GetDirectoryName(path);
            }
            else//选择的是文件夹
            {
                directoryPath = path;
            }

            return $"{directoryPath}/{fileName}";
        }

        private static bool CheckAvailability()
        {
            var objs = Selection.objects;

            if (objs.Length != 1)
            {
                MLog.Print($"请选择一个物体", MLogType.Warning);
                return false;
            }

            return true;
        }
    }
}