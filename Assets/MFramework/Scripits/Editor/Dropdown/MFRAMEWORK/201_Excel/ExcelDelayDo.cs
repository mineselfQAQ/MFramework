using System.Collections.Generic;
using UnityEditor;

namespace MFramework
{
    public static class ExcelDelayDo
    {
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
                List<string> fileList = MPathUtility.GetFiles(MConfigurableSettings.ExcelPath, null, ".xlsx");//获取所有文件名

                ExcelGenerator.CreateAllBIN(BINFolder, fileList);
                EditorDelayExecute.Instance.DelayRefresh();//延迟执行Refresh(否则无法刷新成功)
            }
        }

    }
}
