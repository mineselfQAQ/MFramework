using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Selectionµƒ¿©≥‰¿‡
    /// </summary>
    public static class MSelection
    {
        public static Object[] projectObjects
        {
            get
            {
                Object[] objs = Selection.objects;

                List<Object> temp = new List<Object>();
                foreach (var obj in objs)
                {
                    if (AssetDatabase.Contains(obj))
                    {
                        temp.Add(obj);
                    }
                }

                return temp.ToArray();
            }
        }

        public static GameObject[] hierarchyObjects
        {
            get
            {
                return Selection.gameObjects;
            }
        }
    }
}
