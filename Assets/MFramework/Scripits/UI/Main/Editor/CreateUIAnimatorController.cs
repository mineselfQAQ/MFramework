using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public static class CreateUIAnimatorController
    {
        [MenuItem("Assets/CreateUIAnimatorController", false, 9)]
        public static void Create()
        {
            if (CheckAvailability())
            {
                Debug.Log("OK");
            }
        }

        private static bool CheckAvailability()
        {
            var objs = Selection.objects;
            if (objs.Length != 1)
            {
                MLog.Print("请勿多选资源，请重试", MLogType.Warning);
                return false;
            }
            if (!PrefabUtility.IsPartOfAnyPrefab(objs[0]))
            {
                MLog.Print($"{objs[0].name}并非prefab，无法进行创建操作，请重试", MLogType.Warning);
                return false;
            }
            GameObject prefab = objs[0] as GameObject;
            bool flag = false;
            var comps = prefab.GetComponents<Component>();
            foreach (var comp in comps)
            {
                MonoScript monoScript = MonoScript.FromMonoBehaviour(comp as MonoBehaviour);
                if (monoScript != null && monoScript.name == "UIPanelBehaviour")
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                MLog.Print($"{objs[0].name}中没有UIPanelBehaviour脚本，无法进行创建操作，请重试", MLogType.Warning);
                return false;
            }

            return true;
        }
    }
}