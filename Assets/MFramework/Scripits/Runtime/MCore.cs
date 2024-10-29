using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace MFramework
{
    public class MCore : ComponentSingleton<MCore>
    {
        [SerializeField]
        private bool m_LogState;//�ڷ����汾�����Log�ļ�
        [SerializeField]
        private bool m_UICustomLoadState;//�ڱ༭���汾������UI�Զ������
        [SerializeField]
        private bool m_LocalState;//�Ƿ����ñ��ػ�

        private List<INeedInit> initList;
        private List<INeedQuit> quitList;

        public bool LogState => m_LogState;
        public bool UICustomLoadState => m_UICustomLoadState;

        protected override void Awake()
        {
            base.Awake();
            if (this == null) return;//�����ѱ�ɾ��(��Ϊ�Ѵ���)

            DontDestroyOnLoad(gameObject);

            //������̬���캯����ʹ������ǰ����
            var bem = BuiltInEventManager.Instance;
            if (m_LocalState)
            {
                var mlm = MLocalizationManager.Instance;
            }

            //TODO:��������ܺģ�������������
            initList = GetInterfaceInstanceList<INeedInit>();
            quitList = GetInterfaceInstanceList<INeedQuit>();

            //�����߳�����mainThread
            MainThreadUtility.SetMainThread();

            SceneManager.sceneLoaded += OnSceneLoaded;
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

        /// <summary>
        /// �л�����ʱ�ر�����Я��(��ֹ�л�ʱЯ�̻���ִ��)
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StopAllCoroutines();
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