using System;
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
                MLog.Print($"UI：没有名为{BuildSettings.uiCanvasName}的Canvas，也没有名为{BuildSettings.uiCameraName}的Camera，请修改或创建后重试", MLogType.Warning);
                return;
            }
            else if (UICanvas == null)
            {
                MLog.Print($"UI：没有名为{BuildSettings.uiCanvasName}的Canvas，请修改或创建后重试", MLogType.Warning);
                return;
            }
            else if (UICamera == null)
            {
                MLog.Print($"UI：没有名为{BuildSettings.uiCameraName}的Camera，请修改或创建后重试", MLogType.Warning);
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
                MLog.Print($"UI：Root-{id}已存在，请勿重复创建", MLogType.Warning);
                return null;
            }
            if (start < 0 || end < start)
            {
                MLog.Print($"UI：Root-{id}的StartOrder/EndOrder不符合要求，请检查", MLogType.Warning);
                return null;
            }

            UIRoot uiRoot = new UIRoot(id, start, end);
            RootDic.Add(id, uiRoot);

            return uiRoot;
        }

        internal void SetFocus(UIPanel newTopPanel)
        {
            UIPanel topPanel = newTopPanel.parentRoot.topPanel;
            topPanel.SetFocus(false);
            newTopPanel.SetFocus(true);
            newTopPanel.SetSortingOrder(topPanel.sortingOrder + topPanel.panelBehaviour.thinkness);
            newTopPanel.parentRoot.topPanel = newTopPanel;
        }

        private void ClickDetect()
        {
            PointerEventData eventData = new PointerEventData(current);
            eventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            current.RaycastAll(eventData, results);

            UIPanel newTopPanel = GetTopItemCanvas(results);
            if (newTopPanel == null || newTopPanel == newTopPanel.parentRoot.topPanel)//当前Panel就是最上层Panel
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
                    if (canvas != null)
                    {
                        if (canvas.sortingOrder > maxSortingOrder)
                        {
                            maxSortingOrder = canvas.sortingOrder;
                            topItemCanvas = canvas.gameObject;
                        }
                    }
                }

                return topItemCanvas.GetComponent<UIPanelBehaviour>().panel;
            }

            return null;
        }
    }
}
