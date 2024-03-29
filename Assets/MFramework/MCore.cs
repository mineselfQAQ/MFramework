using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

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
                    PropertyInfo info = type.GetProperty("Instance");
                    var instance = (INeedInit)info.GetValue(this, null);
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
                    PropertyInfo info = type.GetProperty("Instance");
                    var instance = (INeedQuit)info.GetValue(this, null);
                    instance.Quit();
                    Debug.Log($"{instance.GetType()}：QUIT");
                }
            }
        }
    }
}