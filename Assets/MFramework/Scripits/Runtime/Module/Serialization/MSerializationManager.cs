using System;
using System.IO;
using UnityEngine;

namespace MFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable, "#MSerializationManager#")]
    public class MSerializationManager : MonoSingleton<MSerializationManager>
    {
        public string settingsPath;
        public CoreSettings coreSettings => MSerializationUtility.ReadFromJson<CoreSettings>(settingsPath);

        private void Awake()
        {
            //创建或读取CoreSettings
            settingsPath = $"{MSettings.PersistentDataPath}/CoreSettings.json";
            if (!File.Exists(settingsPath))
            {
                MSerializationUtility.SaveToJson<CoreSettings>(settingsPath, new CoreSettings(), true);
                MLog.Print($"{typeof(MLocalizationManager)}：已初始化CoreSettings文件，路径:<{settingsPath}>");
            }
        }
    }
}
