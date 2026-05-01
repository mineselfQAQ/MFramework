using UnityEngine;

namespace MFramework
{
    [System.Serializable]
    public class Injection
    {
        public string name;
        public GameObject value;
    }

    public class MLuaInjection : MonoBehaviour
    {
        [SerializeField]
        private Injection[] injections;

        internal Injection[] Injections
        {
            get
            {
                return injections;
            }
        }
    }
}
