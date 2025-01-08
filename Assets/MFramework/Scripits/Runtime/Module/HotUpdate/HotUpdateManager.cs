using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class HotUpdateManager : MonoSingleton<HotUpdateManager>
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static string url = "http://127.0.0.1:5858";
#elif UNITY_ANDROID
    public static string url = null;
#elif UNITY_IPHONE
    public static string url = null;
#endif

        private void Awake()
        {
            123
        }
    }
}
