using UnityEngine;

namespace MFramework
{
    public class MCore : MonoBehaviour
    {
        public bool logCallbackOn;//在发布版本中输出Log文件

        private void Start()
        {
            if (logCallbackOn)
            {
                MLog.Init();
            }
        }

        private void OnApplicationQuit()
        {
            if (logCallbackOn)
            {
                MLog.Quit();
            }
        }
    }
}