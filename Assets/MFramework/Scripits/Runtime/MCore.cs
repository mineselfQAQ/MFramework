using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MFramework
{
    public class MCore : ComponentSingleton<MCore>
    {
        [SerializeField]
        private bool m_ExportLog;//在发布版本中输出Log文件

        private List<INeedInit> initList;
        private List<INeedQuit> quitList;

        public bool GetExportLog() => m_ExportLog;
        public bool SetExportLog(bool b) => m_ExportLog = b;

        protected override void Awake()
        {
            base.Awake();
            if (this == null) return;//物体已被删除(因为已存在)

            DontDestroyOnLoad(gameObject);

            //触发静态构造函数，使单例提前激活
            var bem = BuiltInEventManager.Instance;
            var mlm = MLocalizationManager.Instance;//TODO:我不用就不应该触发

            //TODO:这样反射很耗，考虑其他方案
            initList = GetInterfaceInstanceList<INeedInit>();
            quitList = GetInterfaceInstanceList<INeedQuit>();
        }

        private void Start()
        {
            foreach (INeedInit instance in initList)
            {
                instance.Init();
            }
        }

        private void OnApplicationQuit()
        {
            foreach (INeedQuit instance in quitList)
            {
                instance.Quit();
            }
        }

        private List<T> GetInterfaceInstanceList<T>()
        {
            List<T> resList = new List<T>();
            var typeList = GetType().Assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(T)));
            foreach (var type in typeList) 
            {
                T instance = (T)Activator.CreateInstance(type);
                resList.Add(instance);
            }
            return resList;
        }
    }
}