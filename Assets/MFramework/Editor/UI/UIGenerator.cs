using MFramework.Core;
using MFramework.UI;
using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MTextComponent = MFramework.Text.MText;

namespace MFramework.Editor.UI
{
    public static class UIGenerator
    {
        private static readonly Vector2 ReferenceResolution = new(1920f, 1080f);

        [MenuItem("GameObject/MFramework/UI/MPanel", priority = 0)]
        public static void GeneratePanel()
        {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create MPanel");
            GameObject gameObject = CommonGenerator(CreateUIType.MPanel);
            Selection.activeGameObject = gameObject;
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/MWidget", priority = 1)]
        public static void GenerateWidget()
        {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create MWidget");
            GameObject gameObject = CommonGenerator(CreateUIType.MWidget);
            Selection.activeGameObject = gameObject;
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/MText", priority = 2)]
        public static void GenerateMText()
        {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create MText");
            GameObject gameObject = CommonGenerator(CreateUIType.MText);
            Selection.activeGameObject = gameObject;
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/MImage", priority = 3)]
        public static void GenerateImage()
        {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create MImage");
            GameObject gameObject = CommonGenerator(CreateUIType.Image);
            Selection.activeGameObject = gameObject;
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/MButton", priority = 4)]
        public static void GenerateButton()
        {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create MButton");
            GameObject gameObject = CommonGenerator(CreateUIType.Button);
            Selection.activeGameObject = gameObject;
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/MButton With MText", priority = 5)]
        public static void GenerateButtonWithMText()
        {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create MButton With MText");
            GameObject gameObject = CommonGenerator(CreateUIType.ButtonWithMText);
            Selection.activeGameObject = gameObject;
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/Background", priority = 6)]
        public static void GenerateBackground()
        {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create UI Background");
            GameObject gameObject = CommonGenerator(CreateUIType.Background, "Background", false);
            Selection.activeGameObject = gameObject;
            Undo.CollapseUndoOperations(undoGroup);
        }

        [MenuItem("GameObject/MFramework/UI/UICanvas", priority = 7)]
        public static void GenerateUICanvas()
        {
            if (GameObject.Find(MUIManager.DefaultCanvasName) != null)
            {
                MLog.Default?.E("UICanvas already exists.");
                return;
            }

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create UICanvas");
            GameObject canvas = CreateUIGameObject(CreateUIType.CanvasUI);
            AddEventSystemIfNotExist();
            EditorGUIUtility.PingObject(canvas);
            Selection.activeGameObject = canvas;
            Undo.CollapseUndoOperations(undoGroup);
        }

        private static GameObject CommonGenerator(CreateUIType type, string name = null, bool isTop = true)
        {
            if (!CheckAvailability()) return null;

            GameObject result = null;
            int selectedAmount = Selection.gameObjects.Length;
            if (selectedAmount == 0)
            {
                GameObject canvas = GameObject.Find(MUIManager.DefaultCanvasName) ?? CreateUIGameObject(CreateUIType.CanvasUI);
                result = CreateUIGameObject(type, name, canvas);
            }
            else if (selectedAmount == 1)
            {
                GameObject selected = Selection.gameObjects[0];
                int parentCanvasType = CheckParentIsCanvas(selected);
                if (parentCanvasType > 0)
                {
                    result = CreateUIGameObject(type, name, selected);
                }
                else
                {
                    GameObject canvas = CreateUIGameObject(CreateUIType.CanvasCommon, null, selected);
                    result = CreateUIGameObject(type, name, canvas);
                }
            }

            if (result != null && !isTop)
            {
                result.transform.SetAsFirstSibling();
            }

            AddEventSystemIfNotExist();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            return result;
        }

        private static GameObject CreateUIGameObject(CreateUIType type, string name = null, GameObject parent = null)
        {
            name ??= GetDefaultName(type);

            switch (type)
            {
                case CreateUIType.CameraUI:
                    return CreateUICamera(parent);
                case CreateUIType.CanvasUI:
                    return CreateUICanvas(parent);
                case CreateUIType.CanvasCommon:
                    return CreateCommonCanvas(name, parent);
                case CreateUIType.EventSystem:
                    return CreateEventSystem(parent);
                case CreateUIType.MText:
                    return CreateMText(name, parent);
                case CreateUIType.MPanel:
                    return CreatePanel(name, parent);
                case CreateUIType.MWidget:
                    return CreateWidget(name, parent);
                case CreateUIType.Image:
                    return CreateImage(name, parent);
                case CreateUIType.Background:
                    return CreateBackground(name, parent);
                case CreateUIType.Button:
                    return CreateButton(name, parent);
                case CreateUIType.ButtonWithMText:
                    return CreateButtonWithMText(name, parent);
                default:
                    return null;
            }
        }

        private static GameObject CreateUICamera(GameObject parent)
        {
            GameObject cameraObject = new(MUIManager.DefaultCameraName, typeof(Camera));
            SetParent(cameraObject, parent);
            cameraObject.transform.position = new Vector3(0f, 10000f, 0f);

            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Depth;
            camera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            camera.orthographic = true;
            camera.orthographicSize = 1f;
            camera.nearClipPlane = -1f;
            camera.farClipPlane = 1f;
            camera.depth = 10f;

            Undo.RegisterCreatedObjectUndo(cameraObject, "Create UICamera");
            return cameraObject;
        }

        private static GameObject CreateUICanvas(GameObject parent)
        {
            GameObject existing = GameObject.Find(MUIManager.DefaultCanvasName);
            if (existing != null) return existing;

            GameObject canvasObject = new(MUIManager.DefaultCanvasName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            SetParent(canvasObject, parent);
            canvasObject.layer = LayerMask.NameToLayer("UI");

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            GameObject cameraObject = GameObject.Find(MUIManager.DefaultCameraName) ?? CreateUIGameObject(CreateUIType.CameraUI);
            PlaceAbove(cameraObject, canvasObject);
            canvas.worldCamera = cameraObject.GetComponent<Camera>();
            canvas.planeDistance = 0f;
            canvas.additionalShaderChannels =
                AdditionalCanvasShaderChannels.TexCoord1 |
                AdditionalCanvasShaderChannels.Normal |
                AdditionalCanvasShaderChannels.Tangent;

            ConfigureCanvasScaler(canvasObject.GetComponent<CanvasScaler>());
            GameObject background = CreateUIGameObject(CreateUIType.Background, "Background", canvasObject);
            background.transform.SetAsFirstSibling();

            Undo.RegisterCreatedObjectUndo(canvasObject, "Create UICanvas");
            return canvasObject;
        }

        private static GameObject CreateCommonCanvas(string name, GameObject parent)
        {
            GameObject canvasObject = new(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            SetParent(canvasObject, parent);

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            ConfigureCanvasScaler(canvasObject.GetComponent<CanvasScaler>());

            Undo.RegisterCreatedObjectUndo(canvasObject, "Create Canvas");
            return canvasObject;
        }

        private static GameObject CreateEventSystem(GameObject parent)
        {
            GameObject eventSystem = new("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            SetParent(eventSystem, parent);
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
            return eventSystem;
        }

        private static GameObject CreateMText(string name, GameObject parent)
        {
            GameObject textObject = new(name, typeof(MTextComponent));
            SetParent(textObject, parent);
            SetCenterMode(textObject.GetComponent<RectTransform>(), new Vector2(400f, 200f));

            MTextComponent text = textObject.GetComponent<MTextComponent>();
            text.text = "XXX";
            text.color = Color.black;
            text.fontSize = 72f;
            text.alignment = TextAlignmentOptions.Top;
            text.raycastTarget = false;

            Undo.RegisterCreatedObjectUndo(textObject, "Create MText");
            return textObject;
        }

        private static GameObject CreatePanel(string name, GameObject parent)
        {
            GameObject panelObject = new(name, typeof(RectTransform), typeof(CanvasGroup), typeof(UIPanelBehaviour));
            SetParent(panelObject, parent);
            SetRectStretchMode(panelObject.GetComponent<RectTransform>());

            Undo.RegisterCreatedObjectUndo(panelObject, "Create MPanel");
            return panelObject;
        }

        private static GameObject CreateWidget(string name, GameObject parent)
        {
            GameObject widgetObject = new(name, typeof(RectTransform), typeof(CanvasGroup), typeof(UIWidgetBehaviour));
            SetParent(widgetObject, parent);
            SetCenterMode(widgetObject.GetComponent<RectTransform>(), new Vector2(300f, 300f));

            Undo.RegisterCreatedObjectUndo(widgetObject, "Create MWidget");
            return widgetObject;
        }

        private static GameObject CreateImage(string name, GameObject parent)
        {
            GameObject imageObject = new(name, GetMImageType());
            SetParent(imageObject, parent);
            SetCenterMode(imageObject.GetComponent<RectTransform>(), new Vector2(300f, 300f));
            Undo.RegisterCreatedObjectUndo(imageObject, "Create Image");
            return imageObject;
        }

        private static GameObject CreateBackground(string name, GameObject parent)
        {
            GameObject imageObject = new(name, GetMImageType());
            SetParent(imageObject, parent);
            SetRectStretchMode(imageObject.GetComponent<RectTransform>());

            Image image = imageObject.GetComponent<Image>();
            image.color = Color.white;
            image.raycastTarget = false;

            Undo.RegisterCreatedObjectUndo(imageObject, "Create Background");
            return imageObject;
        }

        private static GameObject CreateButton(string name, GameObject parent)
        {
            GameObject buttonObject = new(name, GetMImageType(), GetMButtonType());
            SetParent(buttonObject, parent);
            SetCenterMode(buttonObject.GetComponent<RectTransform>(), new Vector2(300f, 75f));

            Image image = buttonObject.GetComponent<Image>();
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            image.type = Image.Type.Sliced;

            Undo.RegisterCreatedObjectUndo(buttonObject, "Create Button");
            return buttonObject;
        }

        private static GameObject CreateButtonWithMText(string name, GameObject parent)
        {
            GameObject buttonObject = CreateButton(name, parent);
            GameObject textObject = new("MText", typeof(MTextComponent));
            SetParent(textObject, buttonObject);
            SetRectStretchMode(textObject.GetComponent<RectTransform>());

            MTextComponent text = textObject.GetComponent<MTextComponent>();
            text.text = "XXX";
            text.color = Color.black;
            text.fontSize = 36f;
            text.alignment = TextAlignmentOptions.Center;

            Undo.RegisterCreatedObjectUndo(textObject, "Create MText");
            return buttonObject;
        }

        private static int CheckParentIsCanvas(GameObject gameObject)
        {
            Canvas[] canvases = gameObject.GetComponentsInParent<Canvas>();
            if (canvases.Length == 0) return -1;

            foreach (Canvas canvas in canvases)
            {
                if (canvas.name == MUIManager.DefaultCanvasName) return 2;
            }

            return 1;
        }

        private static bool CheckAvailability()
        {
            if (Selection.gameObjects.Length <= 1) return true;

            MLog.Default?.W("Select zero or one GameObject when creating UI objects.");
            return false;
        }

        private static void AddEventSystemIfNotExist()
        {
            if (GameObject.Find("EventSystem") == null)
            {
                CreateUIGameObject(CreateUIType.EventSystem, "EventSystem");
            }
        }

        private static void ConfigureCanvasScaler(CanvasScaler scaler)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
        }

        private static void SetRectStretchMode(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
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

        private static void SetParent(GameObject gameObject, GameObject parent)
        {
            if (parent != null)
            {
                gameObject.transform.SetParent(parent.transform, false);
            }
        }

        private static void PlaceAbove(GameObject source, GameObject target)
        {
            source.transform.SetSiblingIndex(target.transform.GetSiblingIndex());
        }

        private static string GetDefaultName(CreateUIType type)
        {
            return type switch
            {
                CreateUIType.CameraUI => MUIManager.DefaultCameraName,
                CreateUIType.CanvasUI => MUIManager.DefaultCanvasName,
                CreateUIType.CanvasCommon => "Canvas",
                CreateUIType.EventSystem => "EventSystem",
                CreateUIType.MPanel => "MPanel",
                CreateUIType.MWidget => "MWidget",
                CreateUIType.MText => "MText",
                CreateUIType.Image => "MImage",
                CreateUIType.Background => "Background",
                CreateUIType.Button => "MButton",
                CreateUIType.ButtonWithMText => "MButton",
                _ => "UIObject",
            };
        }

        private static Type GetMImageType()
        {
            return FindType("MFramework.UI.MImage") ?? typeof(Image);
        }

        private static Type GetMButtonType()
        {
            return FindType("MFramework.UI.MButton") ?? typeof(Button);
        }

        private static Type FindType(string fullName)
        {
            foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName);
                if (type != null) return type;
            }

            return null;
        }

        private enum CreateUIType
        {
            CameraUI,
            CanvasUI,
            CanvasCommon,
            EventSystem,
            MPanel,
            MWidget,
            MText,
            Image,
            Background,
            Button,
            ButtonWithMText,
        }
    }
}
