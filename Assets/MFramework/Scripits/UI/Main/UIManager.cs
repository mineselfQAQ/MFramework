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
            //TODO:*****固定命名，需要设置更改位置
            UICanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
            UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
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
