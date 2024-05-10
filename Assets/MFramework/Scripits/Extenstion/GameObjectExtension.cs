using UnityEngine;

namespace MFramework
{
    public static class GameObjectExtension
    {
        public static void DeleteAllChild(this GameObject root, bool includeSelf = false)
        {
            int count = root.transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                GameObject.Destroy(((root.transform.GetChild(i))).gameObject);
            }

            if (includeSelf) GameObject.Destroy(root);
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            bool exist = go.TryGetComponent<T>(out T comp);
            if (!exist)
            {
                comp = go.AddComponent<T>();
            }
            return comp;
        }
    }
}

