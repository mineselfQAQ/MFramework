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
        #region ��ӭ����
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

        #region ���MCore�Ƿ���Scene��
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

            //�����������MCore
            foreach (GameObject go in rootGOs)
            {
                if (go.name == "MCore")
                {
                    return;
                }
            }

            //�������Hierarchy������MCore
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.name == "MCore")
                {
                    MLog.Print($"{typeof(InitializeScript)}.{nameof(CheckMCoreExist)}���������MCore�����ڱ�㣬����", MLogType.Warning);
                    return;
                }
            }

            //���MCore
            GameObject MCore = new GameObject("MCore");
            MCore.transform.SetAsFirstSibling();
            MCore.AddComponent<MCore>();
            GameObjectUtility.SetParentAndAlign(MCore, null);
            Selection.activeGameObject = MCore;
            EditorUtility.SetDirty(MCore);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            EditorGUIUtility.PingObject(MCore);
            MLog.Print($"��Ϊ{scene.name}��Ӻ������MCore");
        }
        #endregion

        #region �����Ҫ�ļ����Ƿ����
        [InitializeOnLoadMethod]
        public static void InitializeFolder()
        {
            //Resources
            bool flag = MPathUtility.CreateFolderIfNotExist($@"{Application.dataPath}/Resources");
            if (flag)
            {
                MLog.Print("�ѳ�ʼ������Resources�ļ���");
            }
            //StreamingAssets
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
                MLog.Print("�ѳ�ʼ������StreamingAssets�ļ���");
            }
        }
        #endregion

        #region ExcelGenerator����(����BIN�ļ�)
        [InitializeOnLoadMethod]
        public static void InitializeAfterAssemblyReload()
        {
            //��Ҫ��һ������CS�ļ���BIN�ļ�ʱ������CS�ļ���������������BIN�ļ�����δ�ɹ����أ�
            //��Ҫ**�������غ����BIN�ļ��Ĵ���**
            AssemblyReloadEvents.afterAssemblyReload += GenerateBIN;
        }
        private static void GenerateBIN()
        {
            if (EditorPrefs.GetBool(EditorPrefsData.ExcelBINGenerationState, false))
            {
                EditorPrefs.SetBool(EditorPrefsData.ExcelBINGenerationState, false);

                string BINFolder = MSettings.ExcelBINPath;//Ĭ��.byte�ļ����λ��---StreamingAssets�ļ����ڲ�
                List<string> fileList = MPathUtility.GetFolderFiles(MConfigurableSettings.ExcelPath, ".xlsx");//��ȡ�����ļ���

                ExcelGenerator.CreateAllBIN(BINFolder, fileList);
                EditorDelayExecute.Instance.DelayRefresh();//�ӳ�ִ��Refresh(�����޷�ˢ�³ɹ�)
            }
        }
        #endregion

        /// <summary>
        /// ��⵱ǰ�����Ƿ�Ϊһ̨
        /// </summary>
        [InitializeOnLoadMethod]
        public static void CheckComputerUniqueID()
        {
            string curID = SystemInfo.deviceUniqueIdentifier;
            string preID = EditorPrefs.GetString(EditorPrefsData.DeviceUniqueID, curID);
            if (curID != preID)
            {
                MLog.Print($"{typeof(InitializeScript)}.{nameof(CheckComputerUniqueID)}��ע�⣡��ǰ�豸���л������EditorSettingsConfigurator�鿴·���Ƿ�������ȷ", MLogType.Error);
            }
            EditorPrefs.SetString(EditorPrefsData.DeviceUniqueID, curID);
        }
    }
}