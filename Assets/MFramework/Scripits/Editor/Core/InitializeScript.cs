using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFramework
{
    public class InitializeScript
    {
        #region 欢迎界面
        [InitializeOnLoadMethod]
        public static void InitializeWelcomePage()
        {
            bool state = EditorPrefs.GetBool(EditorPrefsData.WelcomePageState, true);
            if (state)
            {
                WelcomePage.Init();
            }
        }
        #endregion

        #region 检查MCore是否在Scene中
        [InitializeOnLoadMethod]
        public static void InitializeSceneOpen()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }
        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (EditorPrefs.GetBool(EditorPrefsData.EnableCheckMCoreExist, true))
            {
                CheckMCoreExist(scene);
            }
        }
        private static void CheckMCoreExist(Scene scene)
        {
            GameObject[] rootGOs = SceneManager.GetActiveScene().GetRootGameObjects();

            //检查表层中有无MCore
            foreach (GameObject go in rootGOs)
            {
                if (go.name == "MCore")
                {
                    return;
                }
            }

            //检查完整Hierarchy中有无MCore
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.name == "MCore")
                {
                    MLog.Print($"{typeof(InitializeScript)}.{nameof(CheckMCoreExist)}：核心组件MCore不处于表层，请检查", MLogType.Warning);
                    return;
                }
            }

            //添加MCore
            GameObject MCore = new GameObject("MCore");
            MCore.transform.SetAsFirstSibling();
            MCore.AddComponent<MCore>();
            GameObjectUtility.SetParentAndAlign(MCore, null);
            Selection.activeGameObject = MCore;
            EditorUtility.SetDirty(MCore);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            EditorGUIUtility.PingObject(MCore);
            MLog.Print($"已为{scene.name}添加核心组件MCore");
        }
        #endregion

        #region 检查重要文件夹是否存在
        [InitializeOnLoadMethod]
        public static void InitializeFolder()
        {
            //Resources
            bool flag = MPathUtility.CreateFolderIfNotExist($@"{Application.dataPath}/Resources");
            if (flag)
            {
                MLog.Print("已初始化生成Resources文件夹");
            }
            //StreamingAssets
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
                MLog.Print("已初始化生成StreamingAssets文件夹");
            }
        }
        #endregion

        #region ExcelGenerator操作(创建BIN文件)
        [InitializeOnLoadMethod]
        public static void InitializeAfterAssemblyReload()
        {
            //概要：一键生成CS文件和BIN文件时，由于CS文件创建后立即创建BIN文件导致未成功加载，
            //需要**在域重载后进行BIN文件的创建**
            AssemblyReloadEvents.afterAssemblyReload += GenerateBIN;
        }
        private static void GenerateBIN()
        {
            if (EditorPrefs.GetBool(EditorPrefsData.ExcelBINGenerationState, false))
            {
                EditorPrefs.SetBool(EditorPrefsData.ExcelBINGenerationState, false);

                string BINFolder = MSettings.ExcelBINPath;//默认.byte文件存放位置---StreamingAssets文件夹内部
                List<string> fileList = MPathUtility.GetFolderFiles(MConfigurableSettings.ExcelPath, ".xlsx");//获取所有文件名

                ExcelGenerator.CreateAllBIN(BINFolder, fileList);
                EditorDelayExecute.Instance.DelayRefresh();//延迟执行Refresh(否则无法刷新成功)
            }
        }
        #endregion

        /// <summary>
        /// 检测当前电脑是否为一台
        /// </summary>
        [InitializeOnLoadMethod]
        public static void CheckComputerUniqueID()
        {
            string curID = SystemInfo.deviceUniqueIdentifier;
            string preID = EditorPrefs.GetString(EditorPrefsData.DeviceUniqueID, curID);
            if (curID != preID)
            {
                MLog.Print($"{typeof(InitializeScript)}.{nameof(CheckComputerUniqueID)}：注意！当前设备已切换，请打开EditorSettingsConfigurator查看路径是否配置正确", MLogType.Error);
            }
            EditorPrefs.SetString(EditorPrefsData.DeviceUniqueID, curID);
        }
    }
}