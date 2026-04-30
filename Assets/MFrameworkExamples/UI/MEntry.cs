using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Core.Tracker;
using MFramework.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MFrameworkExamples.UI
{
    public class MEntry : MEntryBase
    {
        private Canvas _canvas;
        private Camera _camera;
        private GameObject _eventSystem;
        private UIRoot _root;
        private SimpleFadePanel _panel;

        protected override IModule[] ConfigureModules()
        {
            EnsureUISceneObjects();
            return new IModule[]
            {
                new UIModule(_canvas, _camera, _eventSystem),
            };
        }

        protected override void OnInitialized(TrackerStoppedEvent e)
        {
            MUIManager uiManager = Core.Container.Resolve<MUIManager>();
            _root = uiManager.CreateRoot("ExampleRoot", 0, 1000);

            UIPanelBehaviour panelBehaviour = CreatePanelBehaviour("ExamplePanel", new Vector2(360f, 180f), new Color(0.16f, 0.19f, 0.23f, 0.96f));
            _panel = _root.CreatePanel<SimpleFadePanel>("ExamplePanel", panelBehaviour, autoEnter: true);

            MLog.Default.D($"UI example created panel: {_panel.PanelID}, order: {_panel.SortingOrder}");
        }

        protected override void OnUnityUpdate()
        {
            if (_root == null || !_root.ExistPanel("ExamplePanel")) return;

            if (Input.GetKeyDown(KeyCode.O)) _root.OpenPanel("ExamplePanel", pinToTop: true);
            if (Input.GetKeyDown(KeyCode.C)) _root.ClosePanel("ExamplePanel");
        }

        private void EnsureUISceneObjects()
        {
            if (_camera == null)
            {
                GameObject cameraObject = new(MUIManager.DefaultCameraName);
                _camera = cameraObject.AddComponent<Camera>();
                _camera.clearFlags = CameraClearFlags.Depth;
                _camera.orthographic = true;
            }

            if (_canvas == null)
            {
                GameObject canvasObject = new(MUIManager.DefaultCanvasName);
                _canvas = canvasObject.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceCamera;
                _canvas.worldCamera = _camera;
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            if (_eventSystem == null)
            {
                _eventSystem = new GameObject("EventSystem");
                _eventSystem.AddComponent<EventSystem>();
                _eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        private UIPanelBehaviour CreatePanelBehaviour(string panelName, Vector2 size, Color color)
        {
            GameObject panelObject = new(panelName);
            panelObject.transform.SetParent(_canvas.transform, false);

            RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = size;

            Image image = panelObject.AddComponent<Image>();
            image.color = color;

            Text label = new GameObject("Title").AddComponent<Text>();
            label.transform.SetParent(panelObject.transform, false);
            label.text = "MFramework UI";
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            RectTransform labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            return panelObject.AddComponent<UIPanelBehaviour>();
        }
    }
}
