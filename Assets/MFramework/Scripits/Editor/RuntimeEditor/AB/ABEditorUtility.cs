namespace MFramework
{
    internal static class ABEditorUtility
    {
        private static string BuildRoot { get; set; }

        internal static string GetBuildRootPath()
        {
            if (BuildRoot == null)
            {
                var buildSetting = MSerializationUtility.ReadFromXml<BuildSetting>(MSettings.ABBuildSettingName);
                if (buildSetting == null)
                {
                    MLog.Print($"{typeof(ABEditorUtility)}��·��{MSettings.ABBuildSettingName}����ʧ�ܣ�����", MLogType.Error);
                    return null;
                }
                buildSetting.GetBuildRoot();
                BuildRoot = buildSetting.buildRoot;
            }

            return BuildRoot;
        }
    }
}
