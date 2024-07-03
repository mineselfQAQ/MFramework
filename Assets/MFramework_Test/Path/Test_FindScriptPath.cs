#if UNITY_EDITOR
using MFramework;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Test_FindScriptPath : MonoBehaviour
{
    private void Start()
    {
        //寻找该脚本存储的实际位置
        string name = this.name;
        string[] guids = AssetDatabase.FindAssets(name);
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));

            string secondPath = "";
            //secondPath = AssetDatabase.GetAssetPath(obj); //等价式
            if (obj)
            {
                secondPath = path;
                string fullPath = Path.GetFullPath(secondPath);
                MLog.Print(fullPath);
                //EditorGUIUtility.PingObject(obj); //不能Ping，必须在非运行时才能起效
                break;
            }
        }
    }
}
#endif
