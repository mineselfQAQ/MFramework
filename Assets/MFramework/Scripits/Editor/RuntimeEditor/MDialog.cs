using System;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class MDialog : EditorWindow
    {
        private string[] btnsName;

        private string message;

        private bool isClickCustomBtn = false; 
        
        private Action<int> onBtnClick;

        public static void ShowDialog(string title, string message, Action<int> onButtonClick, params string[] names)
        {
            MDialog window = ScriptableObject.CreateInstance<MDialog>();
            window.minSize = new Vector2(300, 50);
            window.maxSize = new Vector2(300, 50);
            window.ShowUtility();
            window.titleContent = new GUIContent(title);
            window.btnsName = names;
            window.message = message;
            window.onBtnClick = onButtonClick;
            window.Show();
        }

        private void OnDestroy()
        {
            //逻辑：如果是自定义按钮，会在关闭前提前设为true，关闭后设为false，那么只有右上红叉会触发
            if (!isClickCustomBtn)//点击红叉触发关闭
            {
                onBtnClick(-1);
                MLog.Print("已取消");
            }
        }

        private void OnGUI()
        {
            GUILayout.Label(message);

            EditorGUILayout.BeginHorizontal();
            {
                for (int i = 0; i < btnsName.Length; i++)
                {
                    if (GUILayout.Button(btnsName[i]))
                    {
                        isClickCustomBtn = true;
                        onBtnClick(i);
                        this.Close();
                        isClickCustomBtn = false;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}