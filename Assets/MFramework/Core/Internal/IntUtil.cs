using UnityEngine;

namespace MFramework.Core.Internal
{
    /// <summary>
    /// 内置功能(仅供框架使用)
    /// </summary>
    internal static partial class IntUtil
    {
        # region 颜色
        internal static string Col(object message, Color color)
        {
            string htmlColor = ColorUtility.ToHtmlStringRGBA(color);
            string resultStr = $"<color=#{htmlColor}>{message}</color>";

            return resultStr;
        }
        # endregion
    }
}