using MFramework.UI;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class ChangeFontAsset : EditorWindow
    {
        private Object folderObj;
        private Object fontAssetObj;

        [MenuItem("MFramework/ChangeFontAsset", false, 905)]
        public static void Init()
        {
            ChangeFontAsset window = GetWindow<ChangeFontAsset>(true, "ChangeFontAsset");
            window.minSize = new Vector2(200, 200);
            window.maxSize = new Vector2(200, 200);
            window.Show();
        }

        private void OnGUI()
        {
            folderObj = EditorGUILayout.ObjectField(folderObj, typeof(DefaultAsset), false);
            fontAssetObj = EditorGUILayout.ObjectField(fontAssetObj, typeof(TMP_FontAsset), false);
            string path = AssetDatabase.GetAssetPath(folderObj);
            TMP_FontAsset fontAsset = (TMP_FontAsset)fontAssetObj;

            if (GUILayout.Button("更改文件夹下所有FontAssets"))
            {
                string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { path });
                foreach (string guid in guids)
                {
                    string prefabPath = AssetDatabase.GUIDToAssetPath(guid);

                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    if (prefab != null)
                    {
                        var texts = prefab.GetComponentsInChildren<MText>(true);
                        foreach (var text in texts)
                        {
                            text.font = fontAsset;
                        }
                        EditorUtility.SetDirty(prefab);
                    }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("更改当前Scene下所有FontAssets"))
            {
                var texts = GameObject.FindObjectsOfType<MText>(true);

                foreach (var text in texts)
                {
                    text.font = fontAsset;
                }
            }
        }
    }
}
