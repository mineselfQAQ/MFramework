using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFramework
{
    public static class TransformExtension
    {
        public static Transform FindChildByName(this Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child;
                }

                //递归查找子对象
                var found = FindChildByName(child, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public static void DeleteAllChild(this Transform root, bool includeSelf = false)
        {
            int count = root.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                GameObject.Destroy((root.GetChild(i)).gameObject);
            }

            if (includeSelf) GameObject.Destroy(root.gameObject);
        }


        /// <summary>
        /// 置于物体上侧
        /// </summary>
        public static void PlaceAbove(this Transform src, GameObject below)
        {
            PlaceAbove(src, below.transform);
        }
        public static void PlaceAbove(this Transform src, Transform below)
        {
            int sibling = below.GetSiblingIndex();

            var gos = SceneManager.GetActiveScene().GetRootGameObjects();
            bool canDo = false;
            foreach (var go in gos)//顺序排列
            {
                //从下侧物体开始，所有物体索引+1，留出空位
                //如：
                //1 |2| 3 |4| 5
                //1    |3| 4 |5| 6
                //1 |2||3| 4  6
                if (go.name == below.name)
                {
                    canDo = true;
                }
                if (canDo)
                {
                    go.transform.SetSiblingIndex(go.transform.GetSiblingIndex() + 1);
                }
            }
            src.SetSiblingIndex(sibling);
        }
        /// <summary>
        /// 置于物体下侧
        /// </summary>
        public static void PlaceBelow(this Transform src, GameObject above)
        {
            PlaceBelow(src, above.transform);
        }
        public static void PlaceBelow(this Transform src, Transform above)
        {
            int sibling = above.GetSiblingIndex();

            var gos = SceneManager.GetActiveScene().GetRootGameObjects();
            bool canDo = false;
            foreach (var go in gos)//顺序排列
            {
                //从下侧物体下一个开始，所有物体索引+1，留出空位
                //如：
                //1 |2| 3 |4| 5
                //1 |2|    4 |5| 6
                //1 |2||3| 4  6
                if (go.name == above.name)
                {
                    canDo = true;
                    continue;
                }
                if (canDo)
                {
                    go.transform.SetSiblingIndex(go.transform.GetSiblingIndex() + 1);
                }
            }
            src.SetSiblingIndex(sibling + 1);
        }
    }
}