using System;
using System.Collections.Generic;
using TMPro;
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

        protected void Create(string id, Transform parent, string prefabPath)
        {
            InstantiateAndCollectFields(id, parent, prefabPath);

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

        private void InstantiateAndCollectFields(string id, Transform parent, string prefabPath)
        {
            //信息收集
            viewID = id;
            parentTrans = parent;

            //TODO:路径需要处理，Editor下至少是从Assets开始的
            //实例化
#if UNITY_EDITOR
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) MLog.Print($"UI：未获取到{prefabPath}下的Prefab，请检查路径是否正确", MLogType.Warning);
#else
            GameObject prefab = LoadPrefab(prefabPath);
            if (prefab == null) MLog.Print($"UI：未获取到{prefabPath}下的Prefab，请检查路径与重写的LoadPrefab()是否正确");
#endif
            GameObject go = GameObject.Instantiate(prefab, parentTrans, false);
            UIViewBehaviour behaviour = go.GetComponent<UIViewBehaviour>();
            if (behaviour == null) MLog.Print($"UI：\"{id}\"上未挂载Behaviour组件，请检查", MLogType.Warning);

            //信息收集
            viewBehaviour = behaviour;
            trans = viewBehaviour.gameObject.GetComponent<RectTransform>();
            gameObject = viewBehaviour.gameObject;
        }

        /// <summary>
        /// 加载资源方式，默认使用Resource.Load()进行加载
        /// </summary>
        protected virtual GameObject LoadPrefab(string prefabPath)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            return prefab;
        }

        //之所以需要将Widget的创建写在这里，是因为：
        //无论是Panel还是Widget，都是能创建/删除Widget的
        //对于Panel，就是创建第一个内部Widget，对于Widget，就是创建该Widget的子Widget
        //也就是说**UIPanel类和UIWidget类都需要操作Widget**
        #region Widget操作接口
        protected T CreateWidget<T>(string id, Transform parent, string prefabPath) where T : UIWidget
        {
            //parent必须是该View下的根物体才能创建，有点局限？
            //UIViewBehaviour parentBehaviour = parent.GetComponentInParent<UIViewBehaviour>();
            //if (viewBehaviour != parentBehaviour) MLog.Print("UI：parent并非根物体");

            T widget = Activator.CreateInstance<T>() as T;
            //widget.Create();

            if (widgetDic == null) widgetDic = new Dictionary<string, UIWidget>();
            widgetDic.Add(id, widget);
            return widget;
        }

        protected bool DestroyWidget(string id)
        {
            UIWidget widget = widgetDic[id];
            widgetDic.Remove(id);

            //widget.destroy();
            return true;
        }

        protected UIWidget GetWidget(string id)
        {
            if (widgetDic == null) return null;
            return widgetDic[id];
        }

        protected bool ExistWidget(string id)
        {
            if (widgetDic == null) return false;
            return widgetDic.ContainsKey(id);
        }
        #endregion

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
        protected virtual void OnValueChanged(TMP_Dropdown dropdown, int value) { }
        protected virtual void OnValueChanged(TMP_InputField inputField, string value) { }
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
}