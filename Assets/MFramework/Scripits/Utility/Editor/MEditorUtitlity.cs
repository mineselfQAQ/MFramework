using System;
using UnityEditor;
using UnityEngine;

internal static class MEditorUtitlity
{
    internal static Texture GetIcon(Type type)
    {
        Texture systemIcon = EditorGUIUtility.ObjectContent(null, type).image;
        Texture customIcon = null;
        Texture csScriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image;

        if (type == typeof(TMPro.TMP_InputField))
        {
            customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Input Field Icon.psd");
        }
        else if (type == typeof(TMPro.TMP_Dropdown))
        {
            customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Dropdown Icon.psd");
        }
        else if (type == typeof(TMPro.TextMeshProUGUI))
        {
            customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Text Component Icon.psd");
        }
        //...

        return systemIcon ?? customIcon ?? csScriptIcon;
    }

    /// <summary>
    /// 深度优先遍历
    /// </summary>
    internal static void DFS(GameObject go, Action<GameObject> onDeal)
    {
        onDeal(go);
        foreach (Transform child in go.transform)
        {
            DFS(child.gameObject, onDeal);
        }
    }
}
