using MFramework.UI;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFramework
{
    public static class UIGenerator
    {
        [MenuItem("GameObject/MFramework/UI/MText", false, 0)]
        public static void GenerateMText()
        {
            if (CheckAvailability())//合法情况
            {
                int selectedAmount = Selection.gameObjects.Length;
                if (selectedAmount == 0)//未选择情况
                {

                }
                if (selectedAmount == 1)//选择情况
                {
                    GameObject go = Selection.gameObjects[0];
                    if (CheckParentIsCanvas(go))//Canvas子物体情况
                    {
                        GameObject MText = new GameObject("MText", typeof(MText));
                        MText.SetParent(go);
                    }
                    else//非Canvas子物体情况
                    {

                    }
                }
            }






            Scene scene = EditorSceneManager.GetActiveScene();
            GameObject[] gos = scene.GetRootGameObjects();
            GameObject canvasGO = null;
            foreach (var go in gos)
            {
                Canvas canvas = go.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvasGO = go;
                }
            }

            if (canvasGO != null)//获取到Canvas组件
            {
                
            }
            else
            {

            }
        }

        private static bool CheckParentIsCanvas(GameObject go)
        {
            Canvas canvas = go.GetComponentInParent<Canvas>();
            if (canvas == null) return false;
            return true;
        }

        private static bool CheckAvailability()
        {
            var objs = Selection.gameObjects;

            if (objs.Length > 1)
            {
                MLog.Print("请勿多选物体，请重试", MLogType.Warning);
                return false;
            }

            return true;
        }
    }
}