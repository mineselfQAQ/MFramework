using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MFramework
{
    public class MCore : MonoBehaviour
    {
        public bool logCallbackOn;//在发布版本中输出Log文件
        IEnumerable<Type> INeedInitType;
        IEnumerable<Type> INeedQuitType;

        private void Awake()
        {
            INeedInitType = this.GetType().Assembly.GetTypes()
                                   .Where(t => t.GetInterfaces().Contains(typeof(INeedInit)));
            INeedQuitType = this.GetType().Assembly.GetTypes()
                                   .Where(t => t.GetInterfaces().Contains(typeof(INeedQuit)));
        }

        private void Start()
        {
            if (logCallbackOn)
            {
                foreach (var type in INeedInitType)
                {
                    var instance = (INeedInit)Activator.CreateInstance(type);
                    instance.Init();
                    Debug.Log($"{instance.GetType()}：INIT");
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (logCallbackOn)
            {
                foreach (var type in INeedQuitType)
                {
                    var instance = (INeedQuit)Activator.CreateInstance(type);
                    instance.Quit();
                    Debug.Log($"{instance.GetType()}：QUIT");
                }
            }
        }
    }
}