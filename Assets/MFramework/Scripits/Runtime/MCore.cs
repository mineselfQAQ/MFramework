using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

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

        [SerializeField]
        TextMeshProUGUI display;

        public enum DisplayMode { FPS, MS }

        [SerializeField]
        DisplayMode displayMode = DisplayMode.FPS;

        [SerializeField, Range(0.1f, 2f)]
        float sampleDurations = 0.2f;

        int frames;

        float duration, bestDuration = float.MaxValue, worstDuration;
        float best = 0, avr = 0, worst = 0;

        void Update()
        {
            float frameDuration = Time.unscaledDeltaTime;
            frames += 1;
            duration += frameDuration;

            if (frameDuration < bestDuration)
            {
                bestDuration = frameDuration;
            }
            if (frameDuration > worstDuration)
            {
                worstDuration = frameDuration;
            }

            if (duration >= sampleDurations)
            {
                if (displayMode == DisplayMode.FPS)
                {
                    best = 1f / bestDuration;
                    avr = frames / duration;
                    worst = 1f / worstDuration;
                }
                else
                {
                    best = 1000f * bestDuration;
                    avr = 1000f * duration / frames;
                    worst = 1000f * worstDuration;
                }
                frames = 0;
                duration = 0f;
                bestDuration = float.MaxValue;
                worstDuration = 0f;
            }
        }
        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 200, 30), "FPS");
            GUI.Label(new Rect(10, 50, 200, 30), string.Format("Average:{0:0}", avr));
            GUI.Label(new Rect(10, 100, 200, 30), string.Format("Best:{0:0}", best));
            GUI.Label(new Rect(10, 150, 200, 30), string.Format("Worst:{0:0}", worst));
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