using UnityEngine;

namespace MFramework
{
    public static class TransformExtension
    {
        public static void DeleteAllChild(this Transform root, bool includeSelf = false)
        {
            int count = root.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                GameObject.Destroy((root.GetChild(i)).gameObject);
            }

            if (includeSelf) GameObject.Destroy(root.gameObject);
        }
    }
}