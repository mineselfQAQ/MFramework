using UnityEngine;

namespace MFramework
{
    public class MCore : MonoBehaviour
    {
        public bool logCallbackOn;

        private void Start()
        {
            if (logCallbackOn)
            {
                Log.Init();
            }
        }

        private void OnApplicationQuit()
        {
            if (logCallbackOn)
            {
                Log.Quit();
            }
        }
    }
}