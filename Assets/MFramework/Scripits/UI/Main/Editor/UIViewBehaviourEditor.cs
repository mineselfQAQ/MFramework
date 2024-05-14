using MFramework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(UIViewBehaviour))]
    public abstract class UIViewBehaviourEditor : Editor
    {
        #region 代码生成
        protected void DrawExportBtn()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("输出Base文件"))
                {
                    if (Application.isPlaying)
                    {
                        MLog.Print("UI：请在非运行时导出", MLogType.Warning); 
                        return; 
                    }
                    //RefreshCompCollectionList(compCollectionListRL);
                    //GenerateUIBaseCode();
                }

                if (GUILayout.Button("输出核心文件"))
                {
                    if (Application.isPlaying)
                    {
                        MLog.Print("UI：请在非运行时导出", MLogType.Warning);
                        return;
                    }
                    //RefreshCompCollectionList(compCollectionListRL);
                    //GenerateUIMainCode();
                }
            }
            GUILayout.EndHorizontal();
        }
        #endregion
    }
}