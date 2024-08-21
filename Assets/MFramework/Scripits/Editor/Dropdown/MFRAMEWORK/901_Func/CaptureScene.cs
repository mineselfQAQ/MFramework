using System;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class CaptureScene : MonoBehaviour
    {
        [MenuItem("MFramework/CaptureScene _F7", priority = 906)]
        public static void CaptureSceneFunc()
        {
            string time = DateTime.Now.ToString("yyyyMMdd_HHmm");
            string savePath = $"{MSettings.TempRootPath}/Screenshots/screenshot_{time}.png";
            ScreenCapture.CaptureScreenshot(savePath);
            MLog.Print($"ÒÑ½ØÍ¼£¬Â·¾¶£º<{savePath}>");

            savePath = savePath.Replace("/", "\\");
            System.Diagnostics.Process.Start("explorer", "/select,\"" + savePath + "\"");
        }
    }
}
