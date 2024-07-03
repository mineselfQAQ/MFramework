#if UNITY_EDITOR
using MFramework;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Test_FindScriptPath : MonoBehaviour
{
    private void Start()
    {
        //Ѱ�Ҹýű��洢��ʵ��λ��
        string name = this.name;
        string[] guids = AssetDatabase.FindAssets(name);
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));

            string secondPath = "";
            //secondPath = AssetDatabase.GetAssetPath(obj); //�ȼ�ʽ
            if (obj)
            {
                secondPath = path;
                string fullPath = Path.GetFullPath(secondPath);
                MLog.Print(fullPath);
                //EditorGUIUtility.PingObject(obj); //����Ping�������ڷ�����ʱ������Ч
                break;
            }
        }
    }
}
#endif
