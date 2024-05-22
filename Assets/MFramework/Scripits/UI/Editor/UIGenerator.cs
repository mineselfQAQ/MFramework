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
        [MenuItem("GameObject/MFramework/UI/MBackground", priority = 1, secondaryPriority = 0)]
        public static void GenerateMBackground()
        {
            CommonGenerator(CreateUIType.MBackground, "MBackground", false);
        }

        [MenuItem("GameObject/MFramework/UI/MText", priority = 1, secondaryPriority = 100)]
        public static void GenerateMText()
        {
            CommonGenerator(CreateUIType.MText);
        }

        [MenuItem("GameObject/MFramework/UI/MImage", priority = 1, secondaryPriority = 101)]
        public static void GenerateMImage()
        {
            CommonGenerator(CreateUIType.MImage);
        }

        [MenuItem("GameObject/MFramework/UI/MButton", priority = 1, secondaryPriority = 102)]
        public static void GenerateMButton()
        {
            CommonGenerator(CreateUIType.MButton);
        }

        [MenuItem("GameObject/MFramework/UI/MButton-WithMText", priority = 1, secondaryPriority = 103)]
        public static void GenerateMButton_WithMText()
        {
            CommonGenerator(CreateUIType.MButton_WithMText);
        }



        private static GameObject CommonGenerator(CreateUIType type, string name = null, bool isTop = true)
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
                        resGO = CreateUIGameObject(type, name, canvasGO);
                    }
                    else//未获取到Canvas组件
                    {
                        GameObject newCanvas = CreateUIGameObject(CreateUIType.Canvas);
                        resGO = CreateUIGameObject(type, name, newCanvas);
                    }
                }
                else if (selectedAmount == 1)//选择情况
                {
                    GameObject go = Selection.gameObjects[0];
                    if (CheckParentIsCanvas(go))//Canvas子物体情况
                    {
                        resGO = CreateUIGameObject(type, name, go);
                    }
                    else//非Canvas子物体情况
                    {
                        GameObject newCanvas = CreateUIGameObject(CreateUIType.Canvas, null, go);
                        resGO = CreateUIGameObject(type, name, newCanvas);
                    }
                }

                if (!isTop)
                {
                    resGO.transform.SetAsFirstSibling();//置底
                }

                AddEventSystemIfNotExist();

                //EditorGUIUtility.PingObject(mText);//高亮物体
                Selection.activeGameObject = resGO;//选择并进入改名状态
                return resGO;
            }
            return null;
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
                        SetCenterMode(trans, new Vector2(400, 200));

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
                        SetCenterMode(trans, new Vector2(300, 300));

                        return mImageGO;
                    }
                case CreateUIType.MBackground:
                    {
                        GameObject mBackgroundGO = new GameObject(name, typeof(MImage));
                        mBackgroundGO.SetParent(parent);

                        var trans = mBackgroundGO.GetComponent<RectTransform>();
                        SetRectStretchMode(trans);

                        var image = mBackgroundGO.GetComponent<MImage>();
                        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(EditorResourcesPath.SampleWhitePath);

                        return mBackgroundGO;
                    }
                case CreateUIType.MButton:
                    {
                        GameObject mButtonGO = new GameObject(name, typeof(MImage), typeof(MButton));
                        mButtonGO.SetParent(parent);

                        var trans = mButtonGO.GetComponent<RectTransform>();
                        SetCenterMode(trans, new Vector2(300, 75));

                        var image = mButtonGO.GetComponent<MImage>();
                        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                        image.type = Image.Type.Sliced;

                        return mButtonGO;
                    }
                case CreateUIType.MButton_WithMText:
                    {
                        //---MButton---
                        GameObject mButtonGO = new GameObject(name, typeof(MImage), typeof(MButton));
                        mButtonGO.SetParent(parent);

                        var trans = mButtonGO.GetComponent<RectTransform>();
                        SetCenterMode(trans, new Vector2(300, 75));

                        var image = mButtonGO.GetComponent<MImage>();
                        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                        image.type = Image.Type.Sliced;

                        //---MText---
                        GameObject mTextGO = new GameObject("MText", typeof(MText));
                        mTextGO.SetParent(mButtonGO);

                        var trans2 = mTextGO.GetComponent<RectTransform>();
                        SetRectStretchMode(trans2);

                        var text = mTextGO.GetComponent<MText>();
                        text.text = "XXX";
                        text.color = Color.black;
                        text.fontSize = 36;
                        text.alignment = TMPro.TextAlignmentOptions.Center;

                        return mButtonGO;
                    }
                default:
                    return null;
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

        private static void AddEventSystemIfNotExist()
        {
            GameObject checker = GameObject.Find("EventSystem");
            if (checker == null)
            {
                CreateUIGameObject(CreateUIType.EventSystem, "EventSystem");
            }
        }

        private static void SetRectStretchMode(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.localScale = Vector3.one;
        }
        private static void SetCenterMode(RectTransform rectTransform, Vector2 size)
        {
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.localScale = Vector3.one;
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
                case CreateUIType.MBackground:
                    return typeof(MImage);
                case CreateUIType.MButton:
                    return typeof(MButton);
                case CreateUIType.MButton_WithMText:
                    return typeof(MButton);
                default:
                    return null;
            }
        }

        private enum CreateUIType
        {
            Canvas,
            EventSystem,

            MText,
            MImage,
            MBackground,
            MButton,
            MButton_WithMText,
        }
    }
}