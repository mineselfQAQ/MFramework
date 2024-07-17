using UnityEngine;

namespace MFramework
{
    public static class ComponentExtension
    {
        public static T GetOrAddComponent<T>(this Component co) where T : Component
        {
            bool exist = co.TryGetComponent<T>(out T comp);
            if (!exist)
            {
                comp = co.AddComponent<T>();
            }
            return comp;
        }
    }
}