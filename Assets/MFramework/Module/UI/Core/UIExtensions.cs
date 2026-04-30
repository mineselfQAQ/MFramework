using UnityEngine;

namespace MFramework.UI
{
    internal static class UIExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent(out T component)
                ? component
                : gameObject.AddComponent<T>();
        }

        public static void SetParent(this GameObject gameObject, GameObject parent)
        {
            gameObject.transform.SetParent(parent.transform, false);
        }
    }
}
