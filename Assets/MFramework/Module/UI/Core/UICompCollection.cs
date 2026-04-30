using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework.UI
{
    [Serializable]
    public class UICompCollection
    {
        [SerializeField] private GameObject target;
        [SerializeField] private List<Component> compList = new();

        public GameObject Target
        {
            get => target;
            set => target = value;
        }

        public List<Component> CompList => compList;

        public Component GetComp(int index)
        {
            return compList[index];
        }
    }
}
