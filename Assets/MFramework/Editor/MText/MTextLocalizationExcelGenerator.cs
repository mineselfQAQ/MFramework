using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MFramework.Editor.Excel;
using MFramework.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFramework.Editor.MText
{
    public sealed class MTextLocalizationExcelGenerator : EditorWindow
    {
        private const string DefaultExportRelativePath = "CORE/Localization/MTextLocalization.xlsx";
        private const string DefaultGeneratedCsFolder = "Assets/MFramework/Module/MText/Generated";
        private const string DefaultGeneratedByteFolder = "Assets/StreamingAssets";

        private Vector2 _scroll;
        private List<LocalizationSceneInfo> _sceneInfos;

        [MenuItem("MFramework/Localization Excel Generator", priority = 203)]
        public static void Open()
        {
            var window = GetWindow<MTextLocalizationExcelGenerator>(true, "Localization Excel Generator", true);
            window.minSize = new Vector2(540f, 520f);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Localization Excel Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space(6f);

            DrawPaths();
            EditorGUILayout.Space(8f);
            DrawImportExport();
            EditorGUILayout.Space(8f);
            DrawSceneScanner();
        }

        private void DrawPaths()
        {
            EditorGUILayout.LabelField("Files", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("Excel", GetLocalizationExcelPath());
            }
        }

        private void DrawImportExport()
        {
            EditorGUILayout.LabelField("Generate", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Generate .xlsx File", GUILayout.Height(30f)))
                {
                    GenerateExcel();
                }

                if (GUILayout.Button("Generate .cs + .byte File", GUILayout.Height(30f)))
                {
                    GenerateCSAndByte();
                }
            }

            EditorGUILayout.HelpBox(
                "Generate Excel scans enabled Build Settings scenes. CS/BYTE generation calls the generic ExcelGenerator with MText default folders.",
                MessageType.Info);
        }

        private void DrawSceneScanner()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Scene MText", EditorStyles.boldLabel);
                if (GUILayout.Button("Refresh", GUILayout.Width(96f)))
                {
                    RefreshSceneInfos();
                }
            }

            if (_sceneInfos == null)
            {
                RefreshSceneInfos();
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (LocalizationSceneInfo info in _sceneInfos)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(info.Key, EditorStyles.boldLabel);
                    EditorGUILayout.ObjectField("Object", info.GameObject, typeof(GameObject), true);
                    EditorGUILayout.LabelField("Scene", info.SceneName);
                    EditorGUILayout.LabelField("Prefab", info.PrefabName);
                    EditorGUILayout.LabelField("Text", info.Text);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void GenerateCSAndByte()
        {
            string inputFolder = Path.GetDirectoryName(GetLocalizationExcelPath())?.Replace('\\', '/');

            bool success = ExcelGenerator.CreateAllCSAndDelayBIN(inputFolder, DefaultGeneratedCsFolder, DefaultGeneratedByteFolder);
            if (!success)
            {
                EditorUtility.DisplayDialog("Generate Failed", "No valid xlsx files were generated. Check paths and console logs.", "OK");
                return;
            }

            EditorUtility.DisplayDialog("Generate Started", $".cs files were generated into:\n{DefaultGeneratedCsFolder}\n\n.byte files will be generated after Unity compiles into:\n{DefaultGeneratedByteFolder}", "OK");
        }

        private void GenerateExcel()
        {
            string targetPath = GetLocalizationExcelPath();
            List<string[]> rows = BuildAllBuildSceneTemplateRows();
            ExcelGenerator.WriteXlsx(targetPath, rows);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Generate Complete", $"Excel generated:\n{targetPath}", "OK");
        }

        private static string GetLocalizationExcelPath()
        {
            return Path.Combine(GetProjectRootPath(), DefaultExportRelativePath).Replace('\\', '/');
        }

        private static string GetProjectRootPath()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..")).Replace('\\', '/');
        }

        private void RefreshSceneInfos()
        {
            _sceneInfos = new List<LocalizationSceneInfo>();
            Scene scene = EditorSceneManager.GetActiveScene();

            AddSceneInfos(scene, _sceneInfos, keepObjectReference: true);

            _sceneInfos = _sceneInfos.OrderBy(i => i.Key, StringComparer.Ordinal).ToList();
        }

        private List<string[]> BuildAllBuildSceneTemplateRows()
        {
            List<LocalizationSceneInfo> infos = CollectBuildSceneInfos();
            var rows = new List<string[]>
            {
                new[] { "Key", "SceneName", "PrefabName", "GOName", "Desc", "Chinese", "English" },
                new[] { "KEY", "SceneName", "PrefabName", "GOName", "Desc", "CHINESE", "ENGLISH" },
                new[] { "string", "none", "none", "none", "none", "string", "string" },
            };

            foreach (LocalizationSceneInfo info in infos.Where(i => !string.IsNullOrWhiteSpace(i.Key)))
            {
                rows.Add(new[]
                {
                    info.Key,
                    info.SceneName,
                    info.PrefabName,
                    !string.IsNullOrWhiteSpace(info.GameObjectName) ? info.GameObjectName : string.Empty,
                    string.Empty,
                    info.Text,
                    info.Text,
                });
            }

            return rows;
        }

        private static List<LocalizationSceneInfo> CollectBuildSceneInfos()
        {
            var infos = new List<LocalizationSceneInfo>();
            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();
            if (buildScenes.Length == 0)
            {
                AddSceneInfos(EditorSceneManager.GetActiveScene(), infos, keepObjectReference: false);
                return infos.OrderBy(i => i.Key, StringComparer.Ordinal).ToList();
            }

            SceneSetup[] setup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                foreach (EditorBuildSettingsScene buildScene in buildScenes)
                {
                    Scene scene = FindLoadedScene(buildScene.path);
                    if (!scene.IsValid())
                    {
                        scene = EditorSceneManager.OpenScene(buildScene.path, OpenSceneMode.Additive);
                    }

                    AddSceneInfos(scene, infos, keepObjectReference: false);
                }
            }
            finally
            {
                EditorSceneManager.RestoreSceneManagerSetup(setup);
            }

            return infos.OrderBy(i => i.Key, StringComparer.Ordinal).ToList();
        }

        private static Scene FindLoadedScene(string scenePath)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (string.Equals(scene.path, scenePath, StringComparison.OrdinalIgnoreCase))
                {
                    return scene;
                }
            }

            return default;
        }

        private static void AddSceneInfos(Scene scene, List<LocalizationSceneInfo> infos, bool keepObjectReference)
        {
            if (!scene.IsValid()) return;

            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                foreach (MLocalization localization in rootObject.GetComponentsInChildren<MLocalization>(true))
                {
                    if (localization == null) continue;
                    if (localization.Mode == MTextLocalizationMode.Off) continue;

                    var text = localization.GetComponent<MFramework.Text.MText>();
                    GameObject prefabRoot = PrefabUtility.GetNearestPrefabInstanceRoot(localization.gameObject);
                    infos.Add(new LocalizationSceneInfo
                    {
                        Key = localization.Key,
                        GameObject = keepObjectReference ? localization.gameObject : null,
                        GameObjectName = localization.gameObject.name,
                        SceneName = scene.name,
                        PrefabName = prefabRoot != null ? prefabRoot.name : string.Empty,
                        Text = text != null ? text.text : string.Empty,
                    });
                }
            }
        }

        private sealed class LocalizationSceneInfo
        {
            public string Key;
            public GameObject GameObject;
            public string GameObjectName;
            public string SceneName;
            public string PrefabName;
            public string Text;
        }
    }
}
