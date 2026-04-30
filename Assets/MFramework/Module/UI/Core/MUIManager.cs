using System.Collections.Generic;

using MFramework.Core;
using MFramework.Coroutines;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MFramework.UI
{
    public class MUIManager
    {
        public const string DefaultCanvasName = "UICanvas";
        public const string DefaultCameraName = "UICamera";

        private readonly string _canvasName;
        private readonly string _cameraName;
        private bool _dontDestroyOnLoad;
        private GameObject _blocker;
        private Canvas _blockerCanvas;
        private CanvasGroup _blockerCanvasGroup;
        private bool _blockerOpen;

        public Canvas UICanvas { private set; get; }
        public Camera UICamera { private set; get; }
        public GameObject EventSystem { private set; get; }
        public Dictionary<string, UIRoot> RootDic { get; } = new();
        public IUIPrefabLoader PrefabLoader { get; }
        public MCoroutineManager CoroutineManager { get; }

        public MUIManager(
            Canvas uiCanvas = null,
            Camera uiCamera = null,
            GameObject eventSystem = null,
            IUIPrefabLoader prefabLoader = null,
            MCoroutineManager coroutineManager = null,
            string canvasName = DefaultCanvasName,
            string cameraName = DefaultCameraName)
        {
            UICanvas = uiCanvas;
            UICamera = uiCamera;
            EventSystem = eventSystem;
            PrefabLoader = prefabLoader ?? new ResourcesUIPrefabLoader();
            CoroutineManager = coroutineManager;
            _canvasName = canvasName;
            _cameraName = cameraName;
        }

        public void Initialize()
        {
            UICanvas ??= GameObject.Find(_canvasName)?.GetComponent<Canvas>();
            UICamera ??= GameObject.Find(_cameraName)?.GetComponent<Camera>();
            EventSystem ??= UnityEngine.EventSystems.EventSystem.current != null
                ? UnityEngine.EventSystems.EventSystem.current.gameObject
                : GameObject.Find("EventSystem");

            if (UICanvas == null) MLog.Default.E($"{nameof(MUIManager)}: UI canvas '{_canvasName}' not found.");
            if (UICamera == null) MLog.Default.W($"{nameof(MUIManager)}: UI camera '{_cameraName}' not found.");
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ClickDetect();
            }
        }

        public void Shutdown()
        {
            foreach (UIRoot root in RootDic.Values)
            {
                List<string> panelIds = new(root.PanelDic.Keys);
                foreach (string panelId in panelIds)
                {
                    root.DestroyPanel(panelId);
                }
            }

            RootDic.Clear();
            StopBlocker();
        }

        public void SetDontDestroyOnLoad()
        {
            if (_dontDestroyOnLoad) return;

            _dontDestroyOnLoad = true;
            GameObject root = new("#UIPANEL#");
            UICamera?.gameObject.SetParent(root);
            UICanvas?.gameObject.SetParent(root);
            EventSystem?.SetParent(root);
            Object.DontDestroyOnLoad(root);
        }

        public UIRoot CreateRoot(string id, int start, int end)
        {
            if (RootDic.ContainsKey(id))
            {
                MLog.Default.W($"{nameof(MUIManager)}: root '{id}' already exists.");
                return null;
            }

            if (start < 0 || end < start)
            {
                MLog.Default.W($"{nameof(MUIManager)}: root '{id}' order range [{start}, {end}] is invalid.");
                return null;
            }

            UIRoot uiRoot = new(id, start, end, this);
            RootDic.Add(id, uiRoot);
            return uiRoot;
        }

        public void StartBlocker(int order = 9999)
        {
            if (_blockerOpen || UICanvas == null) return;

            if (_blocker == null)
            {
                _blocker = new GameObject("Blocker");
                _blocker.SetParent(UICanvas.gameObject);
                _blocker.layer = UICanvas.gameObject.layer;

                RectTransform rectTransform = _blocker.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;

                _blockerCanvas = _blocker.AddComponent<Canvas>();
                _blockerCanvas.overrideSorting = true;
                _blockerCanvas.sortingOrder = order;

                _blockerCanvasGroup = _blocker.AddComponent<CanvasGroup>();
                _blockerCanvasGroup.interactable = false;
                _blocker.AddComponent<Image>().color = Color.clear;
                _blocker.AddComponent<GraphicRaycaster>();
            }
            else
            {
                _blockerCanvas.sortingOrder = order;
            }

            _blockerCanvasGroup.alpha = 1f;
            _blockerCanvasGroup.blocksRaycasts = true;
            _blockerOpen = true;
        }

        public void StopBlocker()
        {
            if (!_blockerOpen || _blockerCanvasGroup == null) return;

            _blockerCanvasGroup.alpha = 0f;
            _blockerCanvasGroup.blocksRaycasts = false;
            _blockerOpen = false;
        }

        private void ClickDetect()
        {
            EventSystem current = UnityEngine.EventSystems.EventSystem.current;
            if (current == null || UICanvas == null) return;

            PointerEventData eventData = new(current)
            {
                position = Input.mousePosition,
            };

            List<RaycastResult> results = new();
            current.RaycastAll(eventData, results);

            UIPanel newTopPanel = GetTopItemCanvas(results);
            if (newTopPanel == null ||
                newTopPanel == newTopPanel.ParentRoot.TopPanel ||
                newTopPanel.PanelBehaviour.FocusMode == UIPanelFocusMode.Disabled)
            {
                return;
            }

            SetFocus(newTopPanel);
        }

        private UIPanel GetTopItemCanvas(List<RaycastResult> results)
        {
            int maxSortingOrder = int.MinValue;
            GameObject topItemCanvas = null;

            foreach (RaycastResult result in results)
            {
                Canvas canvas = result.gameObject.GetComponentInParent<Canvas>();
                if (canvas != null && canvas != UICanvas && canvas.sortingOrder > maxSortingOrder)
                {
                    maxSortingOrder = canvas.sortingOrder;
                    topItemCanvas = canvas.gameObject;
                }
            }

            if (topItemCanvas == null || topItemCanvas.name == "Blocker") return null;
            return topItemCanvas.GetComponent<UIPanelBehaviour>()?.View as UIPanel;
        }

        private void SetFocus(UIPanel newTopPanel)
        {
            UIRoot root = newTopPanel.ParentRoot;
            UIPanel topPanel = root.TopPanel;
            topPanel?.SetFocus(false);
            newTopPanel.SetFocus(true);

            int order = (topPanel?.SortingOrder ?? root.StartOrder) + newTopPanel.PanelBehaviour.Thickness;
            newTopPanel.SetSortingOrder(order);
            if (order > root.EndOrder) UIPanelUtility.ResetOrder(root);

            root.TopPanel = newTopPanel;
        }
    }
}
