using System.Collections.Generic;
using UnityEngine;

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
    }
}
