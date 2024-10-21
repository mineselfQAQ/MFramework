using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [InitializeOnLoad]
    public static class NoComponent
    {
        static NoComponent()
        {
            // 监听添加组件的操作
            ObjectFactory.componentWasAdded += OnComponentAdded;
        }

        private static void OnComponentAdded(Component component)
        {
            var componentType = component.GetType();
            var attrs = componentType.GetCustomAttributes(typeof(NoComponentAttribute), true);
            if (attrs.Length == 0)
            {
                return;//并非该特性
            }

            MLog.Print($"不得添加{component.GetType().Name}", MLogType.Warning);
            Object.DestroyImmediate(component);
        }
    }
}
