using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventSystem;

namespace MFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable, "#UIManager#")]
    public class UIManager : MonoSingleton<UIManager>
    {
        private bool dontDestroyOnLoad = false;

        public Canvas UICanvas { private set; get; }
        public Camera UICamera { private set; get; }
        public Dictionary<string, UIRoot> RootDic { private set; get; }

        private void Awake()
        {
            UICanvas = GameObject.Find(MSettings.UICanvasName).GetComponent<Canvas>();
            UICamera = GameObject.Find(MSettings.UICameraName).GetComponent<Camera>();
            if (UICanvas == null && UICamera == null)
            {
                MLog.Print($"{typeof(UIManager)}��û����Ϊ{MSettings.UICanvasName}��Canvas��Ҳû����Ϊ{MSettings.UICameraName}��Camera�����޸Ļ򴴽�������", MLogType.Error);
                return;
            }
            else if (UICanvas == null)
            {
                MLog.Print($"{typeof(UIManager)}��û����Ϊ{MSettings.UICanvasName}��Canvas�����޸Ļ򴴽�������", MLogType.Error);
                return;
            }
            else if (UICamera == null)
            {
                MLog.Print($"{typeof(UIManager)}��û����Ϊ{MSettings.UICameraName}��Camera�����޸Ļ򴴽�������", MLogType.Error);
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

        public void SetDontDestroyOnLoad()
        {
            if (dontDestroyOnLoad) return;//����ɣ������ٴν���

            dontDestroyOnLoad = true;
            GameObject go = new GameObject("#UIPanel#");
            UICamera.gameObject.SetParent(go);
            UICanvas.gameObject.SetParent(go);
            DontDestroyOnLoad(go);
        }

        public UIRoot CreateRoot(string id, int start, int end)
        {
            if (RootDic.ContainsKey(id)) 
            {
                MLog.Print($"{typeof(UIManager)}��Root-<{id}>�Ѵ��ڣ������ظ�����", MLogType.Warning);
                return null;
            }
            if (start < 0 || end < start)
            {
                MLog.Print($"{typeof(UIManager)}��Root-<{id}>�ķ�Χ����ȷ([{start},[{end}])������", MLogType.Warning);
                return null;
            }

            UIRoot uiRoot = new UIRoot(id, start, end);
            RootDic.Add(id, uiRoot);

            return uiRoot;
        }

        /// <summary>
        /// ������ϲ�Panel������Focus
        /// </summary>
        private void ClickDetect()
        {
            if (current == null) return;

            PointerEventData eventData = new PointerEventData(current);
            eventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            current.RaycastAll(eventData, results);

            UIPanel newTopPanel = GetTopItemCanvas(results);
            //û�л�ȡ/��ǰPanel�������ϲ�Panel/������Focus���
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
            UIRoot root = newTopPanel.parentRoot;//����Root
            UIPanel topPanel = root.topPanel;//ԭTopPanel
            topPanel.SetFocus(false);
            newTopPanel.SetFocus(true);

            int order = topPanel.sortingOrder + topPanel.panelBehaviour.Thickness;
            newTopPanel.SetSortingOrder(order);
            if (order > root.endOrder) UIPanelUtility.ResetOrder(root);

            newTopPanel.parentRoot.topPanel = newTopPanel;
        }
    }
}