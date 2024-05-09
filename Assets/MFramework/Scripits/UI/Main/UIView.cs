using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework
{
    /// <summary>
    /// UI组件的基类，作为一个UI组件，其中包含作为一个UI组件必备的内容
    /// </summary>
    public class UIView
    {
        protected string viewID;//UI组件ID，或者说是它的名字
        protected GameObject gameObject;//该物体GameObject
        protected RectTransform trans;//该物体Transform
        protected Transform parentTrans;//父物体Transform，用于设置自身的父亲
        protected UIViewBehaviour viewBehaviour;//收集的信息

        protected Dictionary<string, UIWidget> widgetDic { private set; get; }

        protected static event Action<Button> onButtonClickedEvent;

        protected void Create(string id, Transform parent, string prefabPath)
        {
            InstantiateAndCollectFields(id, parent, prefabPath);

            OnBindCompsAndEvents();//绑定(由Base类完成)
            OnCreating();//创建时事件
            CreatingInternal();//内部创建

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

        private void InstantiateAndCollectFields(string id, Transform parent, string prefabPath)
        {
            //信息收集
            viewID = id;
            parentTrans = parent;

            //实例化
#if UNITY_EDITOR
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) MLog.Print($"UI：未获取到{prefabPath}下的Prefab，请检查路径是否正确");
#else
            GameObject prefab = LoadPrefab(prefabPath);
            if (prefab == null) MLog.Print($"UI：未获取到{prefabPath}下的Prefab，请检查路径与重写的LoadPrefab()是否正确");
#endif
            GameObject go = GameObject.Instantiate(prefab, parentTrans, false);
            UIViewBehaviour behaviour = go.GetComponent<UIViewBehaviour>();
            if (behaviour == null) MLog.Print($"UI：\"{id}\"上未挂载Behaviour组件，请检查");

            //信息收集
            viewBehaviour = behaviour;
            trans = viewBehaviour.gameObject.GetComponent<RectTransform>();
            gameObject = viewBehaviour.gameObject;
        }

        public virtual GameObject LoadPrefab(string prefabPath)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            return prefab;
        }

        #region 内部生命周期
        protected internal virtual void CreatingInternal() { }
        protected internal virtual void DestroyingInternal()
        {
            GameObject.Destroy(gameObject);
            viewID = null;
            gameObject = null;
            trans = null;
            parentTrans = null;
            viewBehaviour = null;
            widgetDic = null;
        }
        protected internal virtual void CreatedInternal() { }
        protected internal virtual void DestroyedInternal() { }
        #endregion

        #region 子类生命周期
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
        protected void BindEvent(InputField inputField)
        {
            inputField.onValueChanged.AddListener((value) =>
            {
                OnValueChanged(inputField, value);
            });
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
        protected void UnbindEvent(InputField inputField)
        {
            inputField.onValueChanged.RemoveAllListeners();
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
}