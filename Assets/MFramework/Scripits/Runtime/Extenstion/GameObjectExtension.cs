using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFramework
{
    public static class GameObjectExtension
    {
        public static GameObject FindChildByName(this GameObject parent, string name)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }

                //递归查找子对象
                var found = FindChildByName(child.gameObject, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

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

        public static void SetParent(this GameObject go, GameObject parent, bool worldPositionStays = false)
        {
            if (parent == null) return;
            go.transform.SetParent(parent.transform, worldPositionStays);
        }
        public static void SetParent(this GameObject go, Transform parent, bool worldPositionStays = false)
        {
            if (parent == null) return;
            go.transform.SetParent(parent, worldPositionStays);
        }

        /// <summary>
        /// 置于物体上侧
        /// </summary>
        public static void PlaceAbove(this GameObject src, GameObject below)
        {
            src.transform.PlaceAbove(below.transform);
        }
        public static void PlaceAbove(this GameObject src, Transform below)
        {
            src.transform.PlaceAbove(below);
        }
        /// <summary>
        /// 置于物体下侧
        /// </summary>
        public static void PlaceBelow(this GameObject src, GameObject above)
        {
            src.transform.PlaceBelow(above.transform);
        }
        public static void PlaceBelow(this GameObject src, Transform above)
        {
            src.transform.PlaceBelow(above);
        }
    }
}

