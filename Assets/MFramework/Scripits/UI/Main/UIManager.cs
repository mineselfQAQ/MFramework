using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventSystem;

namespace MFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable, "#UIManager#")]
    public class UIManager : MonoSingleton<UIManager>
    {
        public Canvas UICanvas { private set; get; }
        public Camera UICamera { private set; get; }
        public Dictionary<string, UIRoot> RootDic { private set; get; }

        private void Awake()
        {
            UICanvas = GameObject.Find(BuildSettings.uiCanvasName).GetComponent<Canvas>();
            UICamera = GameObject.Find(BuildSettings.uiCameraName).GetComponent<Camera>();
            if (UICanvas == null && UICamera == null)
            {
                MLog.Print($"{typeof(UIManager)}：没有名为{BuildSettings.uiCanvasName}的Canvas，也没有名为{BuildSettings.uiCameraName}的Camera，请修改或创建后重试", MLogType.Error);
                return;
            }
            else if (UICanvas == null)
            {
                MLog.Print($"{typeof(UIManager)}：没有名为{BuildSettings.uiCanvasName}的Canvas，请修改或创建后重试", MLogType.Error);
                return;
            }
            else if (UICamera == null)
            {
                MLog.Print($"{typeof(UIManager)}：没有名为{BuildSettings.uiCameraName}的Camera，请修改或创建后重试", MLogType.Error);
                return;
            }

            RootDic = new Dictionary<string, UIRoot>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ClickDetect();
            }
        }

        public UIRoot CreateRoot(string id, int start, int end)
        {
            if (RootDic.ContainsKey(id)) 
            {
                MLog.Print($"{typeof(UIManager)}：Root-<{id}>已存在，请勿重复创建", MLogType.Warning);
                return null;
            }
            if (start < 0 || end < start)
            {
                MLog.Print($"{typeof(UIManager)}：Root-<{id}>的范围不正确([{start},[{end}])，请检查", MLogType.Warning);
                return null;
            }

            UIRoot uiRoot = new UIRoot(id, start, end);
            RootDic.Add(id, uiRoot);

            return uiRoot;
        }

        /// <summary>
        /// 检测最上层Panel并设置Focus
        /// </summary>
        private void ClickDetect()
        {
            PointerEventData eventData = new PointerEventData(current);
            eventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            current.RaycastAll(eventData, results);

            UIPanel newTopPanel = GetTopItemCanvas(results);
            //没有获取/当前Panel就是最上层Panel/不参与Focus检测
            if (newTopPanel == null || newTopPanel == newTopPanel.parentRoot.topPanel 
                || newTopPanel.panelBehaviour.FocusMode == UIPanelFocusMode.Disabled)
            {
                return;
            }
            
            SetFocus(newTopPanel);
        }
        private UIPanel GetTopItemCanvas(List<RaycastResult> results)
        {
            if (results.Count > 0)
            {
                int maxSortingOrder = int.MinValue;
                GameObject topItemCanvas = null;

                foreach (RaycastResult result in results)
                {
                    Canvas canvas = result.gameObject.GetComponentInParent<Canvas>();
                    if (canvas != null && canvas != UICanvas)
                    {
                        if (canvas.sortingOrder > maxSortingOrder)
                        {
                            maxSortingOrder = canvas.sortingOrder;
                            topItemCanvas = canvas.gameObject;
                        }
                    }
                }

                if(topItemCanvas == null) return null;
                return (UIPanel)topItemCanvas.GetComponent<UIPanelBehaviour>().view;
            }

            return null;
        }
        private void SetFocus(UIPanel newTopPanel)
        {
            UIRoot root = newTopPanel.parentRoot;//所在Root
            UIPanel topPanel = root.topPanel;//原TopPanel
            topPanel.SetFocus(false);
            newTopPanel.SetFocus(true);

            int order = topPanel.sortingOrder + topPanel.panelBehaviour.Thickness;
            newTopPanel.SetSortingOrder(order);
            if (order > root.endOrder) UIPanelUtility.ResetOrder(root);

            newTopPanel.parentRoot.topPanel = newTopPanel;
        }
    }
}
