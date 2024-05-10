using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// UIView的MonoBehaviour脚本，用于Prefab的基本设置以及收集组件
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class UIViewBehaviour : MonoBehaviour
    {
        [SerializeField]
        private List<UICompCollection> compCollection;

        protected List<UICompCollection> CompCollection { get { return compCollection; } }

#if UNITY_EDITOR
        private void Awake()
        {
            hideFlags = HideFlags.NotEditable;
        }
#endif
    }
}