using System;
using System.Collections;
using UnityEngine;

namespace MFramework
{
    public static class MTimeUtility
    {
        /// <summary>
        /// 获取当前时间戳(1970.1.1---今)
        /// </summary>
        public static long GetNowTime()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
    }
}