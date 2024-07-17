using UnityEngine;

namespace MFramework
{
    public static class ObjectExtension
    {
        public static T GetOrAddComponent<T>(this Object obj) where T : Component
        {
            return obj.GetComponent<T>() ?? obj.AddComponent<T>();
        }

        public static T GetComponent<T>(this Object obj)
        {
            if (obj is GameObject)
            {
                return ((GameObject)obj).GetComponent<T>();
            }

            if (obj is Component)
            {
                return ((Component)obj).GetComponent<T>();
            }

            MLog.Print($"{typeof(ObjectExtension)}.{nameof(GetComponent)}：未支持的Object<{obj}>", MLogType.Error);
            return default(T);
        }
        public static T AddComponent<T>(this Object obj) where T : Component
        {
            if (obj is GameObject)
            {
                return ((GameObject)obj).AddComponent<T>();
            }

            if (obj is Component)
            {
                return ((Component)obj).gameObject.AddComponent<T>();
            }

            MLog.Print($"{typeof(ObjectExtension)}.{nameof(AddComponent)}：未支持的Object<{obj}>", MLogType.Error);
            return default(T);
        }
    }
}
