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
        //Tip:Camera无法直接创建，UICanvas中创建的Camera是联动的，要创Camera直接用原始的即可

        [MenuItem("GameObject/MFramework/UI/MBackground", priority = 2, secondaryPriority = 0)]
        public static void GenerateMBackground()
        {
            int undoGroup = MUtility.CreateUndoGroup("Create MBackground");
            GameObject go = CommonGenerator(CreateUIType.MBackground, "MBackground", false);
            Selection.activeGameObject = go;//选择并进入改名状态
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/MText", priority = 2, secondaryPriority = 100)]
        public static void GenerateMText()
        {
            int undoGroup = MUtility.CreateUndoGroup("Create MText");
            GameObject go = CommonGenerator(CreateUIType.MText);
            Selection.activeGameObject = go;//选择并进入改名状态
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/MImage", priority = 2, secondaryPriority = 101)]
        public static void GenerateMImage()
        {
            int undoGroup = MUtility.CreateUndoGroup("Create MImage");
            GameObject go = CommonGenerator(CreateUIType.MImage);
            Selection.activeGameObject = go;//选择并进入改名状态
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/MButton", priority = 2, secondaryPriority = 102)]
        public static void GenerateMButton()
        {
            int undoGroup = MUtility.CreateUndoGroup("Create MButton");
            GameObject go = CommonGenerator(CreateUIType.MButton);
            Selection.activeGameObject = go;//选择并进入改名状态
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/MButton-WithMText", priority = 2, secondaryPriority = 103)]
        public static void GenerateMButton_WithMText()
        {
            int undoGroup = MUtility.CreateUndoGroup("Create MButton_WithText");
            GameObject go = CommonGenerator(CreateUIType.MButton_WithMText);
            Selection.activeGameObject = go;//选择并进入改名状态
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/UICanvas", priority = 2, secondaryPriority = 200)]
        public static void GenerateUICanvas()
        {
            GameObject checker = GameObject.Find(MSettings.UICanvasName);
            if (checker != null)
            {
                MLog.Print("UICanvas已存在，请勿重复创建", MLogType.Error);
                return;
            }

            int undoGroup = MUtility.CreateUndoGroup("Create UICanvas");
            GameObject canvasGO = CreateUIGameObject(CreateUIType.Canvas_UI);
            CreateUIGameObject(CreateUIType.EventSystem);
            EditorGUIUtility.PingObject(canvasGO);//高亮物体
            Undo.CollapseUndoOperations(undoGroup);
        }

        /// <summary>
        /// 总体如下：
        /// -选择0个物体：
        /// ---如果场景中有UICanvas，放入其中
        /// ---如果场景中没有UICanvas，创建UICanvas并放入其中
        /// -选择1个物体：
        /// ---如果物体是UICanvas下的物体，正常创建
        /// ---如果是任意Canvas下的物体，正常创建
        /// ---如果不是Canvas下的物体，创建基础Canvas并正常创建
        /// -选择n个物体：
        /// ---不允许创建
        /// </summary>
        private static GameObject CommonGenerator(CreateUIType type, string name = null, bool isTop = true)
        {
            if (CheckAvailability())//合法情况
            {
                GameObject resGO = null;

                int selectedAmount = Selection.gameObjects.Length;
                if (selectedAmount == 0)//未选择情况
                {
                    GameObject canvasGO = GameObject.Find(MSettings.UICanvasName);

                    if (canvasGO != null)//获取到UICanvas
                    {
                        resGO = CreateUIGameObject(type, name, canvasGO);
                    }
                    else//未获取到UICanvas
                    {
                        GameObject uiCanvas = CreateUIGameObject(CreateUIType.Canvas_UI);
                        resGO = CreateUIGameObject(type, name, uiCanvas);
                    }
                }
                else if (selectedAmount == 1)//选择情况
                {
                    GameObject go = Selection.gameObjects[0];
                    int flag = CheckParentIsCanvas(go);
                    if (flag == 2)//UICanvas子物体情况
                    {
                        resGO = CreateUIGameObject(type, name, go);
                    }
                    else if (flag == 1)//Canvas子物体情况
                    {
                        resGO = CreateUIGameObject(type, name, go);
                    }
                    else if(flag == -1)//非Canvas子物体情况
                    {
                        GameObject newCanvas = CreateUIGameObject(CreateUIType.Canvas_Common, null, go);
                        resGO = CreateUIGameObject(type, name, newCanvas);
                    }
                }

                if (!isTop)
                {
                    resGO.transform.SetAsFirstSibling();//置底
                }

                AddEventSystemIfNotExist();

                return resGO;
            }
            return null;
        }

        private static GameObject CreateUIGameObject(CreateUIType type, string name = null, GameObject parent = null)
        {
            name = name == null ? GetType(type).Name : name;

            switch (type)
            {
                case CreateUIType.Camera_UI:
                    {
                        GameObject cameraGO = new GameObject(MSettings.UICameraName, typeof(Camera));
                        cameraGO.SetParent(parent);

                        var trans = cameraGO.transform;
                        trans.position = new Vector3(0, 10000, 0);

                        var camera = cameraGO.GetComponent<Camera>();
                        int uiLayer = 1 << 5;
                        camera.clearFlags = CameraClearFlags.Depth;
                        camera.cullingMask = uiLayer;
                        camera.orthographic = true;
                        camera.orthographicSize = 1;
                        camera.nearClipPlane = -1;
                        camera.farClipPlane = 1;
                        camera.depth = 10;

                        Undo.RegisterCreatedObjectUndo(cameraGO, "Create UICamera");

                        return cameraGO;
                    }
                case CreateUIType.Canvas_UI:
                    {
                        GameObject checker1 = GameObject.Find(MSettings.UICanvasName);
                        if (checker1 != null) return null;

                        GameObject canvasGO = new GameObject(MSettings.UICanvasName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                        canvasGO.SetParent(parent);
                        canvasGO.layer = 5;

                        var canvas = canvasGO.GetComponent<Canvas>();
                        canvas.renderMode = RenderMode.ScreenSpaceCamera;
                        //UICanvas依赖于UICamera，必须获取或者创建
                        GameObject checker2 = GameObject.Find(MSettings.UICameraName);
                        if (checker2 != null) canvas.worldCamera = checker2.GetComponent<Camera>();
                        else
                        {
                            GameObject uiCameraGO = CreateUIGameObject(CreateUIType.Camera_UI);
                            uiCameraGO.PlaceAbove(canvasGO);
                            canvas.worldCamera = uiCameraGO.GetComponent<Camera>();
                        }
                        canvas.planeDistance = 0;
                        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                            AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;

                        var canvasScaler = canvasGO.GetComponent<CanvasScaler>();
                        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                        //TODO:根据平台设置默认分辨率
                        canvasScaler.referenceResolution = new Vector2(1920, 1080);

                        GameObject backgroundGO = CreateUIGameObject(CreateUIType.MBackground, "MBackground");
                        backgroundGO.SetParent(canvasGO, false);

                        Undo.RegisterCreatedObjectUndo(canvasGO, "Create UICanvas");

                        return canvasGO;
                    }
                case CreateUIType.Canvas_Common:
                    {
                        GameObject canvasGO = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                        canvasGO.SetParent(parent);

                        var canvas = canvasGO.GetComponent<Canvas>();
                        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                        var canvasScaler = canvasGO.GetComponent<CanvasScaler>();
                        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                        //TODO:根据平台设置默认分辨率
                        canvasScaler.referenceResolution = new Vector2(1920, 1080);

                        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");

                        return canvasGO;
                    }
                case CreateUIType.EventSystem:
                    {
                        GameObject eventSystemGO = new GameObject(name, typeof(EventSystem), typeof(StandaloneInputModule));
                        eventSystemGO.SetParent(parent);

                        Undo.RegisterCreatedObjectUndo(eventSystemGO, "Create EventSystem");

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
                        text.raycastTarget = false;

                        Undo.RegisterCreatedObjectUndo(mTextGO, "Create MText");

                        return mTextGO;
                    }
                case CreateUIType.MImage:
                    {
                        GameObject mImageGO = new GameObject(name, typeof(MImage));
                        mImageGO.SetParent(parent);

                        var trans = mImageGO.GetComponent<RectTransform>();
                        SetCenterMode(trans, new Vector2(300, 300));

                        Undo.RegisterCreatedObjectUndo(mImageGO, "Create MImage");

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

                        Undo.RegisterCreatedObjectUndo(mBackgroundGO, "Create MBackground");

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

                        Undo.RegisterCreatedObjectUndo(mButtonGO, "Create MButton");

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

                        Undo.RegisterCreatedObjectUndo(mButtonGO, "Create MButton");
                        Undo.RegisterCreatedObjectUndo(mTextGO, "Create MText");

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

        /// <summary>
        /// |-1|---不是Canvas
        /// |1|---是一般Canvas
        /// |2|---是UICanvas
        /// </summary>
        private static int CheckParentIsCanvas(GameObject go)
        {
            Canvas[] canvas = go.GetComponentsInParent<Canvas>();
            if (canvas.Length == 0) return -1;
            foreach (var c in canvas)
            {
                if (c.name == MSettings.UICanvasName)
                {
                    return 2;
                }
            }
            return 1;
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
                case CreateUIType.Camera_UI:
                    return typeof(Camera);
                case CreateUIType.Canvas_UI:
                    return typeof(Canvas);
                case CreateUIType.Canvas_Common:
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
            Camera_UI,
            Canvas_UI,
            Canvas_Common,
            EventSystem,

            MText,
            MImage,
            MBackground,
            MButton,
            MButton_WithMText,
        }
    }
}