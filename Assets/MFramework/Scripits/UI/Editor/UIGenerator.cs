using MFramework.UI;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MFramework
{
    public static class UIGenerator
    {
        [MenuItem("GameObject/MFramework/UI/MText", false, 0)]
        public static void GenerateMText()
        {
            CommonGenerator(CreateUIType.MText);
        }

        [MenuItem("GameObject/MFramework/UI/MImage", false, 0)]
        public static void GenerateMImage()
        {
            CommonGenerator(CreateUIType.MImage);
        }

        private static void CommonGenerator(CreateUIType type)
        {
            if (CheckAvailability())//合法情况
            {
                GameObject resGO = null;

                int selectedAmount = Selection.gameObjects.Length;
                if (selectedAmount == 0)//未选择情况
                {
                    GameObject canvasGO = CheckRootCanvas();

                    if (canvasGO != null)//获取到Canvas组件
                    {
                        resGO = CreateUIGameObject(type, null, canvasGO);
                    }
                    else//未获取到Canvas组件
                    {
                        GameObject newCanvas = CreateUIGameObject(CreateUIType.Canvas);
                        resGO = CreateUIGameObject(type, null, newCanvas);
                    }
                }
                if (selectedAmount == 1)//选择情况
                {
                    GameObject go = Selection.gameObjects[0];
                    if (CheckParentIsCanvas(go))//Canvas子物体情况
                    {
                        resGO = CreateUIGameObject(type, null, go);
                    }
                    else//非Canvas子物体情况
                    {
                        GameObject newCanvas = CreateUIGameObject(CreateUIType.Canvas, null, go);
                        resGO = CreateUIGameObject(type, null, newCanvas);
                    }
                }

                AddEventSystemIfNotExist();

                //EditorGUIUtility.PingObject(mText);
                Selection.activeGameObject = resGO;
            }
        }

        private static void AddEventSystemIfNotExist()
        {
            GameObject checker = GameObject.Find("EventSystem");
            if (checker == null)
            {
                CreateUIGameObject(CreateUIType.EventSystem, "EventSystem");
            }
        }

        private static GameObject CheckRootCanvas()
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            GameObject[] gos = scene.GetRootGameObjects();
            foreach (var go in gos)
            {
                Canvas canvas = go.GetComponent<Canvas>();
                if (canvas != null)
                {
                    return go;
                }
            }
            return null;
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

        private static GameObject CreateUIGameObject(CreateUIType type, string name = null, GameObject parent = null)
        {
            name = name == null ? GetType(type).Name : name;

            switch (type)
            {
                case CreateUIType.Canvas:
                {
                    GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                    canvasGO.SetParent(parent);

                    var canvas = canvasGO.GetComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    return canvasGO;
                }
                case CreateUIType.EventSystem:
                {
                    GameObject eventSystemGO = new GameObject(name, typeof(EventSystem), typeof(StandaloneInputModule));
                    eventSystemGO.SetParent(parent);

                    return eventSystemGO;
                }
                case CreateUIType.MText:
                {
                    GameObject mTextGO = new GameObject(name, typeof(MText));
                    mTextGO.SetParent(parent);

                    var trans = mTextGO.GetComponent<RectTransform>();
                    trans.anchoredPosition = Vector2.zero;
                    trans.sizeDelta = new Vector2(400, 200);

                    var text = mTextGO.GetComponent<MText>();
                    text.text = "XXX";
                    text.fontSize = 72;
                    text.alignment = TMPro.TextAlignmentOptions.Top;

                    return mTextGO;
                }
                case CreateUIType.MImage:
                {
                    GameObject mImageGO = new GameObject(name, typeof(MImage));
                    mImageGO.SetParent(parent);

                    var trans = mImageGO.GetComponent<RectTransform>();
                    trans.anchoredPosition = Vector2.zero;
                    trans.sizeDelta = new Vector2(300, 300);
                    trans.SetAsFirstSibling();

                    return mImageGO;
                }
                default:
                    return null;
            }
        }

        private static Type GetType(CreateUIType type)
        {
            switch (type)
            {
                case CreateUIType.Canvas:
                    return typeof(Canvas);
                case CreateUIType.EventSystem:
                    return typeof(EventSystem);
                case CreateUIType.MText:
                    return typeof(MText);
                case CreateUIType.MImage:
                    return typeof(MImage);
                default:
                    return null;
            }
        }

        public enum CreateUIType
        {
            Canvas,
            EventSystem,

            MText,
            MImage
        }
    }
}