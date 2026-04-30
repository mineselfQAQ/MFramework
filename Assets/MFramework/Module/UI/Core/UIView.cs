using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using MFramework.Core;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework.UI
{
    public class UIView
    {
        internal string ViewID;
        protected UIViewBehaviour viewBehaviour;

        public GameObject GameObject;
        public RectTransform RectTransform;
        public Transform ParentTrans;
        public string PrefabName { get; private set; }
        public bool IsOpen { protected set; get; }
        public MUIManager UIManager { get; private set; }

        private CanvasGroup _canvasGroup;
        private Dictionary<string, UIWidget> _widgetDic;
        private UIShowState _showState = UIShowState.Off;
        private UIAnimState _animState = UIAnimState.Idle;

        public CanvasGroup CanvasGroup
        {
            internal set => _canvasGroup = value;
            get
            {
                _canvasGroup ??= GameObject.GetOrAddComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }

        public UIShowState ShowState
        {
            protected set => _showState = value;
            get => viewBehaviour.AnimSwitch == UIAnimSwitch.On ? UIShowState.None : _showState;
        }

        public UIAnimState AnimState
        {
            protected set => _animState = value;
            get => viewBehaviour.AnimSwitch == UIAnimSwitch.On ? _animState : UIAnimState.None;
        }

        protected void Create(string id, MUIManager uiManager, Transform parent, string prefabPath)
        {
            UIManager = uiManager;
            InstantiateAndCollectFields(id, parent, prefabPath, null);
            RunCreateLifecycle();
        }

        protected void Create(string id, MUIManager uiManager, Transform parent, UIViewBehaviour behaviour)
        {
            UIManager = uiManager;
            InstantiateAndCollectFields(id, parent, null, behaviour);
            RunCreateLifecycle();
        }

        protected void Destroy()
        {
            OnDestroying();
            OnUnbindCompsAndEvents();
            DestroyingInternal();
            DestroyedInternal();
            OnDestroyed();
        }

        private void RunCreateLifecycle()
        {
            CreatingInternal();
            OnBindCompsAndEvents();
            OnCreating();
            CreatedInternal();
            OnCreated();
        }

        private bool InstantiateAndCollectFields(string id, Transform parent, string prefabPath, UIViewBehaviour inputBehaviour)
        {
            ViewID = id;
            ParentTrans = parent;

            UIViewBehaviour behaviour = inputBehaviour;
            if (prefabPath != null)
            {
                GameObject prefab = LoadPrefab(prefabPath);
                if (prefab == null)
                {
                    MLog.Default.E($"{nameof(UIView)}: prefab not found at '{prefabPath}'.");
                    return false;
                }

                GameObject go = UnityEngine.Object.Instantiate(prefab, ParentTrans, false);
                behaviour = go.GetComponent<UIViewBehaviour>();
                if (behaviour == null)
                {
                    MLog.Default.E($"{nameof(UIView)}: '{id}' has no {nameof(UIViewBehaviour)}.");
                    return false;
                }
            }

            viewBehaviour = behaviour;
            GameObject = viewBehaviour.gameObject;
            RectTransform = GameObject.GetComponent<RectTransform>();
            CanvasGroup = GameObject.GetOrAddComponent<CanvasGroup>();
            PrefabName = prefabPath != null ? Path.GetFileNameWithoutExtension(prefabPath) : GameObject.name;
            return true;
        }

        protected virtual GameObject LoadPrefab(string prefabPath)
        {
            return UIManager.PrefabLoader.Load(prefabPath);
        }

        public T CreateWidget<T>(string id, Transform parent, string prefabPath, bool autoEnter = false) where T : UIWidget, new()
        {
            T widget = new T();
            widget.Create(id, parent, prefabPath, this, autoEnter);
            _widgetDic ??= new Dictionary<string, UIWidget>();
            _widgetDic.Add(id, widget);
            return widget;
        }

        public T CreateWidget<T>(string id, string prefabPath, bool autoEnter = false) where T : UIWidget, new()
        {
            return CreateWidget<T>(id, RectTransform, prefabPath, autoEnter);
        }

        public T CreateWidget<T>(Transform parent, string prefabPath, bool autoEnter = false) where T : UIWidget, new()
        {
            return CreateWidget<T>(typeof(T).Name, parent, prefabPath, autoEnter);
        }

        public T CreateWidget<T>(string prefabPath, bool autoEnter = false) where T : UIWidget, new()
        {
            return CreateWidget<T>(typeof(T).Name, RectTransform, prefabPath, autoEnter);
        }

        public T CreateWidget<T>(string id, UIWidgetBehaviour behaviour, bool autoEnter = false) where T : UIWidget, new()
        {
            T widget = new T();
            widget.Create(id, behaviour.transform.parent, behaviour, this, autoEnter);
            _widgetDic ??= new Dictionary<string, UIWidget>();
            _widgetDic.Add(id, widget);
            return widget;
        }

        public T CreateWidget<T>(UIWidgetBehaviour behaviour, bool autoEnter = false) where T : UIWidget, new()
        {
            return CreateWidget<T>(typeof(T).Name, behaviour, autoEnter);
        }

        public bool DestroyWidget(string id)
        {
            if (_widgetDic == null || !_widgetDic.TryGetValue(id, out UIWidget widget)) return false;

            _widgetDic.Remove(id);
            widget.Destroy();
            return true;
        }

        public bool DestroyWidget<T>() where T : UIWidget
        {
            return DestroyWidget(typeof(T).Name);
        }

        public void DestroyAllWidgets()
        {
            if (_widgetDic == null) return;

            List<string> ids = new(_widgetDic.Keys);
            foreach (string id in ids)
            {
                DestroyWidget(id);
            }

            _widgetDic = null;
        }

        public T GetWidget<T>(string id) where T : UIWidget
        {
            return _widgetDic != null && _widgetDic.TryGetValue(id, out UIWidget widget) ? (T)widget : null;
        }

        public T GetWidget<T>() where T : UIWidget
        {
            return GetWidget<T>(typeof(T).Name);
        }

        public bool ExistWidget(string id)
        {
            return _widgetDic != null && _widgetDic.ContainsKey(id);
        }

        public void OpenWidget(string id, Action onFinish = null)
        {
            if (_widgetDic != null && _widgetDic.TryGetValue(id, out UIWidget widget)) widget.Open(onFinish);
        }

        public void OpenWidget<T>(Action onFinish = null)
        {
            OpenWidget(typeof(T).Name, onFinish);
        }

        public void CloseWidget(string id, Action onFinish = null)
        {
            if (_widgetDic != null && _widgetDic.TryGetValue(id, out UIWidget widget)) widget.Close(onFinish);
        }

        public void CloseWidget<T>(Action onFinish = null)
        {
            CloseWidget(typeof(T).Name, onFinish);
        }

        public void SetWidgetSibiling(string id, SiblingMode mode)
        {
            if (_widgetDic != null && _widgetDic.TryGetValue(id, out UIWidget widget)) widget.SetSibling(mode);
        }

        public void SetWidgetSibiling<T>(SiblingMode mode)
        {
            SetWidgetSibiling(typeof(T).Name, mode);
        }

        protected internal virtual void CreatingInternal()
        {
        }

        protected internal virtual void DestroyingInternal()
        {
            UnityEngine.Object.Destroy(GameObject);
            ViewID = null;
            GameObject = null;
            RectTransform = null;
            ParentTrans = null;
            CanvasGroup = null;
            viewBehaviour = null;
            _widgetDic = null;
            UIManager = null;
        }

        protected internal virtual void CreatedInternal()
        {
            IsOpen = true;
        }

        protected internal virtual void DestroyedInternal()
        {
        }

        protected virtual void OnCreating() { }
        protected virtual void OnCreated() { }
        protected virtual void OnDestroying() { }
        protected virtual void OnDestroyed() { }
        protected virtual void OnBindCompsAndEvents() { }
        protected virtual void OnUnbindCompsAndEvents() { }
        protected virtual IEnumerator OnOpenAnim() => null;
        protected virtual IEnumerator OnCloseAnim() => null;
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

        protected void PlaySelfControlAnim(IEnumerator routine, Action onFinish)
        {
            if (routine == null)
            {
                onFinish?.Invoke();
                return;
            }

            if (UIManager?.CoroutineManager == null)
            {
                MLog.Default?.W($"{nameof(UIView)}: self-control animation requires {nameof(UIManager.CoroutineManager)}.");
                onFinish?.Invoke();
                return;
            }

            UIManager.CoroutineManager.StartCoroutineNoRecord(routine, onFinish);
        }

        protected void BindEvent(Button button) => button.onClick.AddListener(() => OnClicked(button));
        protected void BindEvent(Toggle toggle) => toggle.onValueChanged.AddListener(value => OnValueChanged(toggle, value));
        protected void BindEvent(Dropdown dropdown) => dropdown.onValueChanged.AddListener(value => OnValueChanged(dropdown, value));
        protected void BindEvent(TMP_Dropdown dropdown) => dropdown.onValueChanged.AddListener(value => OnValueChanged(dropdown, value));
        protected void BindEvent(InputField inputField) => inputField.onValueChanged.AddListener(value => OnValueChanged(inputField, value));
        protected void BindEvent(TMP_InputField inputField) => inputField.onValueChanged.AddListener(value => OnValueChanged(inputField, value));
        protected void BindEvent(Slider slider) => slider.onValueChanged.AddListener(value => OnValueChanged(slider, value));
        protected void BindEvent(Scrollbar scrollbar) => scrollbar.onValueChanged.AddListener(value => OnValueChanged(scrollbar, value));
        protected void BindEvent(ScrollRect scrollRect) => scrollRect.onValueChanged.AddListener(value => OnValueChanged(scrollRect, value));

        protected void UnbindEvent(Button button) => button.onClick.RemoveAllListeners();
        protected void UnbindEvent(Toggle toggle) => toggle.onValueChanged.RemoveAllListeners();
        protected void UnbindEvent(Dropdown dropdown) => dropdown.onValueChanged.RemoveAllListeners();
        protected void UnbindEvent(TMP_Dropdown dropdown) => dropdown.onValueChanged.RemoveAllListeners();
        protected void UnbindEvent(InputField inputField) => inputField.onValueChanged.RemoveAllListeners();
        protected void UnbindEvent(TMP_InputField inputField) => inputField.onValueChanged.RemoveAllListeners();
        protected void UnbindEvent(Slider slider) => slider.onValueChanged.RemoveAllListeners();
        protected void UnbindEvent(Scrollbar scrollbar) => scrollbar.onValueChanged.RemoveAllListeners();
        protected void UnbindEvent(ScrollRect scrollRect) => scrollRect.onValueChanged.RemoveAllListeners();
    }
}
