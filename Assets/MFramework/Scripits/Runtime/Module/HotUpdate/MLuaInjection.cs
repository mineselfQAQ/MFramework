using UnityEngine;

namespace MFramework
{
    [System.Serializable]
    public class Injection
    {
        public string name;
        public GameObject value;
    }

    public class MLuaInjection : ComponentSingleton<MLuaInjection>
    {
        public Injection[] injections;
    }
}
