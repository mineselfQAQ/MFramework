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
        #region 欢迎界面初始化
        [InitializeOnLoadMethod]
        public static void InitializeWelcomePage()
        {
            //只允许首次进入时自动打开WelcomePage
            bool state = EditorPrefs.GetBool(EditorPrefsData.WelcomePageState, true);
            if (state)
            {
                EditorPrefs.SetBool(EditorPrefsData.WelcomePageState, false);
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
            //打开场景时，如果要求检查MCore，进行处理
            if (EditorPrefs.GetBool(EditorPrefsData.EnableCheckMCore, true))
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
                if (go.name == MSettings.MCoreName)
                {
                    return;
                }
            }

            //检查完整Hierarchy中有无MCore
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.name == MSettings.MCoreName)
                {
                    MLog.Print($"{typeof(InitializeScript)}.{nameof(CheckMCoreExist)}：核心组件MCore不处于表层，请检查是否需", MLogType.Warning);
                    return;
                }
            }

            //添加MCore
            GameObject MCore = new GameObject(MSettings.MCoreName);
            MCore.transform.SetAsFirstSibling();
            MCore.AddComponent<MCore>();
            GameObjectUtility.SetParentAndAlign(MCore, null);
            Selection.activeGameObject = MCore;
            EditorUtility.SetDirty(MCore);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            EditorGUIUtility.PingObject(MCore);
            MLog.Print($"已为{scene.name}添加核心组件{MSettings.MCoreName}");
        }
        #endregion

        /// <summary>
        /// 检测当前电脑是否为一台
        /// </summary>
        [InitializeOnLoadMethod]
        public static void CheckComputerUniqueID()
        {
            string infoName = $"{MSettings.CorePath}/Prefs/DeviceInfo";

            bool exist;
            if (!File.Exists(infoName))
            {
                Directory.CreateDirectory(infoName.CD());//保证文件夹的创建
                exist = false;
            }
            else
            {
                exist = true;
            }

            string curID = SystemInfo.deviceUniqueIdentifier;
            if (!exist)
            {
                MSerializationUtility.SaveToFile(infoName, curID);
            }
            else
            {
                string preID = MSerializationUtility.ReadFromFile(infoName);
                if (curID != preID)
                {
                    MLog.Print($"{typeof(InitializeScript)}.{nameof(CheckComputerUniqueID)}：注意！当前设备已切换，请打开EditorSettingsConfigurator查看路径是否配置正确", MLogType.Warning);
                }
                MSerializationUtility.SaveToFile(infoName, curID);
            }
        }
    }
}