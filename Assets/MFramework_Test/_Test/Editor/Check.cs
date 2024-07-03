using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class Check
{
    [MenuItem("Tools/Check Menu Priority")]
    private static void CheckMenuPriority()
    {
        // 获取所有带有MenuItem特性的静态方法
        var methods = typeof(Editor).Assembly.GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(m => m.GetCustomAttributes(typeof(MenuItem), false).Length > 0);

        foreach (var method in methods)
        {
            var attributes = method.GetCustomAttributes(typeof(MenuItem), false);
            foreach (MenuItem attribute in attributes)
            {
                Debug.Log($"{attribute.menuItem} - {attribute.priority} - {attribute.secondaryPriority}");
            }
        }
    }
}
