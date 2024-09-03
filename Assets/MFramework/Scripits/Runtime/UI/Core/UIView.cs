using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework
{
    /// <summary>
    /// UI组件的基类，作为一个UI组件，其中包含作为一个UI组件必备的内容
    /// </summary>
    public class UIView
    {
        internal string viewID;//UI组件ID，或者说是它的名字
        public GameObject gameObject;//该物体GameObject
        public RectTransform rectTransform;//该物体Transform
        public Transform parentTrans;//父物体Transform，用于设置自身的父亲
        protected UIViewBehaviour viewBehaviour;//通过Inspector挂载收集的信息

        public string prefabName { private set; get; }//预制体名字
        public bool isOpen { protected set; get; }//Panel/Widget开启状态

        private CanvasGroup canvasGroup;
        public CanvasGroup CanvasGroup 
        {
            internal set
            {
                canvasGroup = value;
            }
            get 
            {
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
                }
                return canvasGroup;
            } 
        }

        protected Dictionary<string, UIWidget> widgetDic { private set; get; }

        private UIShowState showState = UIShowState.Off;
        public UIShowState ShowState
        {
            protected set
            {
                showState = value;
            }
            get
            {
                if (viewBehaviour.AnimSwitch == UIAnimSwitch.On)
                {
                    //MLog.Print($"UI：<{viewID}>已开启动画，请调用AnimState检查", MLogType.Warning);
                    return UIShowState.None;
                }
                return showState;
            }
        }
        private UIAnimState animState = UIAnimState.Idle;
        public UIAnimState AnimState
        {
            protected set
            {
                animState = value;
            }
            get
            {
                if (viewBehaviour.AnimSwitch != UIAnimSwitch.On)
                {
                    //MLog.Print($"UI：<{viewID}>未开启动画，请调用ShowState检查", MLogType.Warning);
                    return UIAnimState.None;
                }
                return animState;
            }
        }

        #region 基类操作
        protected void Create(string id, Transform parent, string prefabPath)
        {
            InstantiateAndCollectFields(id, parent, prefabPath, null);

            CreatingInternal();//内部创建
            OnBindCompsAndEvents();//绑定(由Base类完成)
            OnCreating();//创建时事件

            CreatedInternal();//内部构建
            OnCreated();//创建后事件
        }
        protected void Create(string id, Transform parent, UIViewBehaviour behaviour)
        {
            InstantiateAndCollectFields(id, parent, null, behaviour);

            CreatingInternal();//内部创建
            OnBindCompsAndEvents();//绑定(由Base类完成)
            OnCreating();//创建时事件

            CreatedInternal();//内部构建
            OnCreated();//创建后事件
        }

        protected void Destroy()
        {
            OnDestroying();//删除时事件
            OnUnbindCompsAndEvents();//解绑(由Base类完成)
            DestroyingInternal();//内部销毁

            DestroyedInternal();//内部销毁
            OnDestroyed();//删除后事件
        }

        private bool InstantiateAndCollectFields(string id, Transform parent, string prefabPath, UIViewBehaviour inputBehaviour)
        {
            //信息收集
            viewID = id;
            parentTrans = parent;

            UIViewBehaviour behaviour = null;
            //实例化
            if (prefabPath != null)//提供路径模式
            {
                GameObject prefab = null;
#if UNITY_EDITOR
                if (MCore.Instance.UICustomLoadState)//开启UI自定义加载
                {
                    prefab = LoadPrefab(prefabPath);
                    if (prefab == null) { MLog.Print($"{typeof(UIView)}：未获取到{prefabPath}下的Prefab，请检查路径与重写的LoadPrefab()是否正确", MLogType.Error); return false; }
                }
                else
                {
                    prefabPath = prefabPath.Replace('\\', '/');
                    prefabPath = DealEditorPath(prefabPath);
                    prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    if (prefab == null) { MLog.Print($"{typeof(UIView)}：未获取到{prefabPath}下的Prefab，请检查", MLogType.Error); return false; }
                }
#else
                prefab = LoadPrefab(prefabPath);
                if (prefab == null) { MLog.Print($"{typeof(UIView)}：未获取到{prefabPath}下的Prefab，请检查路径与重写的LoadPrefab()是否正确", MLogType.Error); return false; }
#endif
                GameObject go = GameObject.Instantiate(prefab, parentTrans, false);
                //检测
                behaviour = go.GetComponent<UIViewBehaviour>();
                if (behaviour == null) { MLog.Print($"{typeof(UIView)}：\"{id}\"上未挂载Behaviour组件，请检查", MLogType.Error); return false; }
            }
            else if (inputBehaviour != null)//提供UIViewBehaviour模式
            {
                behaviour = inputBehaviour;
            }

            //信息收集
            viewBehaviour = behaviour;
            rectTransform = viewBehaviour.gameObject.GetComponent<RectTransform>();
            gameObject = viewBehaviour.gameObject;

            if (prefabPath != null) prefabName = Path.GetFileNameWithoutExtension(prefabPath);
            else prefabName = inputBehaviour.gameObject.name;//备用选项，物体名字

            return true;
        }
        /// <summary>
        /// 支持2种输入：完整路径/基于Assets的路径
        /// </summary>
        /// <param playerName="path"></param>
        private string DealEditorPath(string path)
        {
            if (Path.IsPathRooted(path))//完整路径
            {
                string firstPath = Application.dataPath;
                if (path.StartsWith(firstPath))
                {
                    path = path.Substring(firstPath.Length - "Assets".Length);
                }
            }
            else
            {
                if (!path.StartsWith("Assets"))//非基于Assets的路径
                {
                    MLog.Print($"{typeof(UIView)}：路径{path}不正确，请提供|完整路径/基于Assets的路径|其中之一", MLogType.Error);
                    return null;
                }
            }

            if (!path.EndsWith(".prefab"))
            {
                path += ".prefab";
            }

            return path;
        }

        /// <summary>
        /// 加载资源方式，默认使用Resource.Load()进行加载
        /// </summary>
        protected virtual GameObject LoadPrefab(string prefabPath)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            return prefab;
        }
        #endregion

        //之所以需要将Widget的创建写在这里，是因为：
        //无论是Panel还是Widget，都是能创建/删除Widget的
        //对于Panel，就是创建第一个内部Widget，对于Widget，就是创建该Widget的子Widget
        //也就是说**UIPanel类和UIWidget类都需要操作Widget**
        #region Widget操作接口
        //不提供id---使用T类型名(只可用于唯一Wdiget情况)
        //不提供parent---使用该UIView的transform(这意味着将生成为一级物体)
        //提供prefabPath---Prefab路径形式
        //提供UIWidgetBehaviour---挂载GameObject形式
        public T CreateWidget<T>(string id, Transform parent, string prefabPath, bool autoEnter = false) where T : UIWidget
        {
            T widget = Activator.CreateInstance<T>() as T;
            widget.Create(id, parent, prefabPath, this, autoEnter);

            if (widgetDic == null) widgetDic = new Dictionary<string, UIWidget>();
            widgetDic.Add(id, widget);

            return widget;
        }
        public T CreateWidget<T>(string id, string prefabPath, bool autoEnter = false) where T : UIWidget
        {
            return CreateWidget<T>(id, rectTransform, prefabPath, autoEnter);
        }
        public T CreateWidget<T>(Transform parent, string prefabPath, bool autoEnter = false) where T : UIWidget
        {
            if (widgetDic != null && widgetDic.ContainsKey(typeof(T).Name))
            {
                MLog.Print($"{typeof(UIView)}：无id方法只能用于一对一情况，如有复用，请传入id", MLogType.Warning);
                return null;
            }
            return CreateWidget<T>(typeof(T).Name, parent, prefabPath, autoEnter);
        }
        public T CreateWidget<T>(string prefabPath, bool autoEnter = false) where T : UIWidget
        {
            if (widgetDic != null && widgetDic.ContainsKey(typeof(T).Name))
            {
                MLog.Print($"{typeof(UIView)}：无id方法只能用于一对一情况，如有复用，请传入id", MLogType.Warning);
                return null;
            }
            return CreateWidget<T>(typeof(T).Name, rectTransform, prefabPath, autoEnter);
        }

        public T CreateWidget<T>(string id, UIWidgetBehaviour behaviour, bool autoEnter = false) where T : UIWidget
        {
            T widget = Activator.CreateInstance<T>() as T;
            widget.Create(id, behaviour.transform.parent, behaviour, this, autoEnter);

            if (widgetDic == null) widgetDic = new Dictionary<string, UIWidget>();
            widgetDic.Add(id, widget);

            return widget;
        }
        public T CreateWidget<T>(UIWidgetBehaviour behaviour, bool autoEnter = false) where T : UIWidget
        {
            if (widgetDic != null && widgetDic.ContainsKey(typeof(T).Name))
            {
                MLog.Print($"{typeof(UIView)}：无id方法只能用于一对一情况，如有复用，请传入id", MLogType.Warning);
                return null;
            }
            return CreateWidget<T>(typeof(T).Name, behaviour, autoEnter);
        }

        public bool DestroyWidget(string id)
        {
            if (widgetDic == null) 
            {
                MLog.Print($"{typeof(UIView)}：ID-<{viewID}>下没有子Widget，删除失败，请检查", MLogType.Warning);
                return false;
            }
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}：ID-<{viewID}>下没有子ID-<{id}>，删除失败，请检查", MLogType.Warning);
                return false;
            } 


            UIWidget widget = widgetDic[id];
            widgetDic.Remove(id);

            widget.Destroy();
            widget = null;
            return true;
        }
        public bool DestroyWidget<T>() where T : UIWidget
        {
            return DestroyWidget(typeof(T).Name);
        }
        public void DestroyAllWidgets()
        {
            if (widgetDic == null || widgetDic.Count <= 0) { return; }

            //删除一级内容(由于每一个Widget的Destroy()都会把自身删除所以不需要再去管子物体了)
            //注意：由于foreach进行时数组不能发生变动，所以需要先将id取出才能进行
            List<string> ids = new List<string>();
            foreach (string id in widgetDic.Keys)
            {
                ids.Add(id);
            }
            foreach (string id in ids)
            {
                DestroyWidget(id);
            }

            widgetDic = null;
        }

        public T GetWidget<T>(string id) where T : UIWidget
        {
            if (widgetDic == null)
            {
                MLog.Print($"{typeof(UIView)}：ID-<{viewID}>下没有子Widget，获取失败，请检查", MLogType.Warning);
                return null;
            }
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}：ID-<{viewID}>下没有子ID-<{id}>，获取失败，请检查", MLogType.Warning);
                return null;
            }

            return (T)widgetDic[id];
        }
        public T GetWidget<T>() where T : UIWidget
        {
            return GetWidget<T>(typeof(T).Name);
        }

        public bool ExistWidget(string id)
        {
            if (widgetDic == null || !widgetDic.ContainsKey(id)) 
            {
                return false;
            }

            return true;
        }

        //不需要SetVisible()操作，Open()/Close()就包含了这一操作
        //public void SetPanelVisible(string id, bool visible)
        //{
        //    if (!widgetDic.ContainsKey(id))
        //    {
        //        MLog.Print($"UI：View-{viewID}下没有<{id}>，设置失败，请检查", MLogType.Warning);
        //        return;
        //    }

        //    UIWidget widget = widgetDic[id];
        //    widget.SetVisible(visible);
        //}
        //public void SetPanelVisible<T>(bool visible)
        //{
        //    SetPanelVisible(typeof(T).Name, visible);
        //}

        public void OpenWidget(string id, Action onFinish = null)
        {
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}：ID-<{viewID}>下没有子ID-<{id}>，打开失败，请检查", MLogType.Warning);
                return;
            }

            UIWidget widget = widgetDic[id];
            widget.Open(onFinish);
        }
        public void OpenWidget<T>(Action onFinish = null)
        {
            OpenWidget(typeof(T).Name, onFinish);
        }

        public void CloseWidget(string id, Action onFinish = null)
        {
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}：ID-<{viewID}>下没有子ID-<{id}>，关闭失败，请检查", MLogType.Warning);
                return;
            }

            UIWidget widget = widgetDic[id];
            widget.Close(onFinish);
        }
        public void CloseWidget<T>(Action onFinish = null)
        {
            CloseWidget(typeof(T).Name, onFinish);
        }

        public void SetWidgetSibiling(string id, SiblingMode mode)
        {
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}：ID-<{viewID}>下没有子ID-<{id}>，设置失败，请检查", MLogType.Warning);
                return;
            }

            UIWidget widget = widgetDic[id];
            widget.SetSibling(mode);
        }
        public void SetWidgetSibiling<T>(SiblingMode mode)
        {
            SetWidgetSibiling(typeof(T).Name, mode);
        }

        public void SetSortingOrder(string id, int order)
        {
            if (!widgetDic.ContainsKey(id))
            {
                MLog.Print($"{typeof(UIView)}：ID-<{viewID}>下没有子ID-<{id}>，设置失败，请检查", MLogType.Warning);
                return;
            }

            UIWidget widget = widgetDic[id];
            widget.SetSortingOrder(id, order);
        }
        public void SetSortingOrder<T>(int order)
        {
            SetSortingOrder(typeof(T).Name, order);
        }
        #endregion

        #region 内部生命周期
        protected internal virtual void CreatingInternal() { }
        protected internal virtual void DestroyingInternal()
        {
            GameObject.Destroy(gameObject);
            viewID = null;
            gameObject = null;
            rectTransform = null;
            parentTrans = null;
            viewBehaviour = null;
            widgetDic = null;
        }
        protected internal virtual void CreatedInternal() 
        {
            isOpen = true;
        }
        protected internal virtual void DestroyedInternal() { }
        #endregion

        #region 子类生命周期
        public virtual void Init() { }
        public virtual void Update() { }

        protected virtual void OnCreating() { }
        protected virtual void OnCreated() { }
        protected virtual void OnDestroying() { }
        protected virtual void OnDestroyed() { }
        protected virtual void OnBindCompsAndEvents() { }
        protected virtual void OnUnbindCompsAndEvents() { }
        protected virtual void OnClicked(Button button) { }
        protected virtual void OnValueChanged(Toggle toggle, bool value) { }
        protected virtual void OnValueChanged(Dropdown dropdown, int value) { }
        protected virtual void OnValueChanged(InputField inputField, string value) { }
        protected virtual void OnValueChanged(Slider slider, float value) { }
        protected virtual void OnValueChanged(Scrollbar scrollbar, float value) { }
        protected virtual void OnValueChanged(ScrollRect scrollRect, Vector2 value) { }
        protected virtual void OnValueChanged(TMP_Dropdown dropdown, int value) { }
        protected virtual void OnValueChanged(TMP_InputField inputField, string value) { }

        protected virtual void OnVisibleChanged(bool visible) { }
        #endregion

        #region 组件事件绑定
        protected void BindEvent(Button button)
        {
            button.onClick.AddListener(() =>
            {
                OnClicked(button);
            });
        }
        protected void BindEvent(Toggle toggle)
        {
            toggle.onValueChanged.AddListener((value) =>
            {
                OnValueChanged(toggle, value);
            });
        }
        protected void BindEvent(Dropdown dropdown)
        {
            dropdown.onValueChanged.AddListener((value) =>
            {
                OnValueChanged(dropdown, value);
            });
        }
        protected void BindEvent(TMP_Dropdown tmpDropdown)
        {
            tmpDropdown.onValueChanged.AddListener((value) =>
            {
                OnValueChanged(tmpDropdown, value);
            });
        }
        protected void BindEvent(InputField inputField)
        {
            inputField.onValueChanged.AddListener((value) =>
            {
                OnValueChanged(inputField, value);
            });
        }
        protected void BindEvent(TMP_InputField tmpInputField)
        {
            tmpInputField.onValueChanged.AddListener((value) => { OnValueChanged(tmpInputField, value); });
        }
        protected void BindEvent(Slider slider)
        {
            slider.onValueChanged.AddListener((value) => 
            { 
                OnValueChanged(slider, value);
            });
        }
        protected void BindEvent(Scrollbar scrollbar)
        {
            scrollbar.onValueChanged.AddListener((value) => 
            {
                OnValueChanged(scrollbar, value); 
            });
        }
        protected void BindEvent(ScrollRect scrollRect)
        {
            scrollRect.onValueChanged.AddListener((value) =>
            { 
                OnValueChanged(scrollRect, value); 
            });
        }
        protected void UnbindEvent(Button button)
        {
            button.onClick.RemoveAllListeners();
        }
        protected void UnbindEvent(Toggle toggle)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(Dropdown dropdown)
        {
            dropdown.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(TMP_Dropdown tmpDropdown)
        {
            tmpDropdown.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(InputField inputField)
        {
            inputField.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(TMP_InputField tmpInputField)
        {
            tmpInputField.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(Slider slider)
        {
            slider.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(Scrollbar scrollbar)
        {
            scrollbar.onValueChanged.RemoveAllListeners();
        }
        protected void UnbindEvent(ScrollRect scrollRect)
        {
            scrollRect.onValueChanged.RemoveAllListeners();
        }
        #endregion
    }

    public enum SiblingMode
    {
        Top,
        Bottom
    }
}