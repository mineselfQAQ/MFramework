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
                    MLog.Default?.E($"AB构建根目录获取失败：构建配置读取失败，path={MPathCache.AB_BUILD_SETTING_PATH}");
                    return null;
                }
                buildSetting.GetBuildRoot();
                BuildRoot = buildSetting.buildRoot;
            }

            return BuildRoot;
        }
    }
}
