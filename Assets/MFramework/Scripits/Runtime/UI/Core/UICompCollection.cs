using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Behaviour中收集到的GameObject的Component集合
    /// </summary>
    [Serializable]
    public class UICompCollection
    {
        [SerializeField]
        private GameObject target;//收集目标
        [SerializeField]
        private List<Component> compList;//收集到的target上的组件

        public GameObject Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
        public List<Component> CompList
        {
            get
            {
                return compList;
            }
        }

        public UICompCollection()
        {
            target = null;
            compList = new List<Component>();
        }

        public Component GetComp(int index) => compList[index];
    }
}