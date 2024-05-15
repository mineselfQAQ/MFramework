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
        private List<UICompCollection> compCollections;

        public List<UICompCollection> CompCollections { get { return compCollections; } }

#if UNITY_EDITOR
        private void Awake()
        {
            hideFlags = HideFlags.NotEditable;
        }
#endif

        /// <summary>
        /// 获取某个Collection的某个Component
        /// </summary>
        public Component GetComp(int collectionIndex, int compIndex)
        {
            return compCollections[collectionIndex].GetComp(compIndex);
        }
    }
}