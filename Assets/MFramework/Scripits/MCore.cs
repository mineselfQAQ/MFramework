using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace MFramework
{
    public class MCore : MonoBehaviour
    {
        [SerializeField]
        private bool m_ExportLog;//�ڷ����汾�����Log�ļ�

        private List<INeedInit> initList;
        private List<INeedQuit> quitList;

        public bool GetExportLog() => m_ExportLog;
        public bool SetExportLog(bool b) => m_ExportLog = b;

        private void Awake()
        {
            //������̬���캯����ʹ������ǰ����
            var bem = BuiltInEventManager.Instance;
            var mlm = MLocalizationManager.Instance;//TODO:�Ҳ��þͲ�Ӧ�ô���

            //TODO:��������ܺģ�������������
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