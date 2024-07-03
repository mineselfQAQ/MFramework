using System;
using UnityEngine;

namespace MFramework
{

    public class MEditorPersistentSettings
    {
        public static readonly string RootPath = Application.dataPath.CD();
        public static readonly string TempAssetBasePath = $"{Application.dataPath}/MTemp";
        public static readonly string TempABBuildPath = $"{RootPath}/MTemp";

        public static readonly string DefaultXMLPath = @$"{Environment.CurrentDirectory}/XmlSettings";
        public static readonly string DefaultJSONPath = @$"{Environment.CurrentDirectory}/JsonSettings";
        public static readonly string DefaultBYTEPath = @$"{Environment.CurrentDirectory}/ByteSettings";

        public static readonly string XMLBuildSettingPath = @$"{DefaultXMLPath}/CORE/ABBuildSetting.xml";
    }
}
