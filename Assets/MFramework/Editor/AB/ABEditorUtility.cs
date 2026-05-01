using MFramework.Core;
using MFramework.Util;

namespace MFramework
{
    internal static class ABEditorUtility
    {
        private static string BuildRoot { get; set; }

        internal static string GetBuildRootPath()
        {
            if (BuildRoot == null)
            {
                var buildSetting = MSerializationUtil.ReadFromXml<BuildSetting>(MPathCache.AB_BUILD_SETTING_PATH);
                if (buildSetting == null)
                {
                    MLog.Default?.E("AB error.");
                    return null;
                }
                buildSetting.GetBuildRoot();
                BuildRoot = buildSetting.buildRoot;
            }

            return BuildRoot;
        }
    }
}