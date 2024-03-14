using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class InitializeScript
    {
        public const string ExcelBINGenerationState = "ExcelBINGenerationState";

        [InitializeOnLoadMethod]
        public static void InitializeFolder()
        {
            //Resources
            bool flag = MPathUtility.CreateFolderIfNotExist($@"{Application.dataPath}\Resources");
            if (flag)
            {
                MLog.Print("已初始化生成Resources文件夹.", MLogType.Warning);
            }
            //StreamingAssets
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
                MLog.Print("已初始化生成StreamingAssets文件夹.", MLogType.Warning);
            }
        }

        [InitializeOnLoadMethod]
        public static void InitializeAfterAssemblyReload()
        {
            //概要：一键生成CS文件和BIN文件时，由于CS文件创建后立即创建BIN文件导致未成功加载，
            //需要**在域重载后进行BIN文件的创建**
            AssemblyReloadEvents.afterAssemblyReload += GenerateBIN;
        }

        /// <summary>
        /// 初始化EditorPrefs
        /// </summary>
        [InitializeOnLoadMethod]
        public static void InitializeEditorPrefs()
        {
            if (!EditorPrefs.HasKey(ExcelBINGenerationState))
            {
                EditorPrefs.SetBool(ExcelBINGenerationState, false);
            }
        }

        private static void GenerateBIN()
        {
            if (EditorPrefs.GetBool(ExcelBINGenerationState))
            {
                EditorPrefs.SetBool(ExcelBINGenerationState, false);

                string BINFolder = EditorSettings.excelBINGenerationPath;//默认.byte文件存放位置---Resources文件夹内部
                List<string> fileList = MPathUtility.GetFolderFiles(EditorSettings.excelGenerationPath, ".xlsx");//获取所有文件名

                ExcelGenerator.CreateAllBIN(BINFolder, fileList);
                EditorDelayExecute.Instance.DelayRefresh();//延迟执行Refresh(否则无法刷新成功)
            }
        }
    }
}