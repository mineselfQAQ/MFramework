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
        private bool m_LogState;//在发布版本中输出Log文件
        [SerializeField]
        private bool m_UICustomLoadState;//在编辑器版本中启用UI自定义加载
        [SerializeField]
        private bool m_LocalState;//是否启用本地化

        [SerializeField]
        private bool m_PerformanceState;//是否启用性能检测
        [SerializeField] private FPSMonitor.DisplayMode m_FPSDisplayMode = FPSMonitor.DisplayMode.FPS;
        [SerializeField] private float m_FPSSampleDuration = 1.0f;
        [SerializeField] private PerformanceMonitor.PKeycode m_keycode = PerformanceMonitor.PKeycode.Backspace;
        //TODO:性能检测需细分(FPS/CPU/GPU/数据检测输出)

        private bool showPerformance = true;
        private static GUIStyle style24;
        private static GUIStyle style30;

        private PerformanceMonitor monitor = null;

        private List<INeedInit> initList;
        private List<INeedQuit> quitList;

        public bool LogState => m_LogState;
        public bool UICustomLoadState => m_UICustomLoadState;

        protected override void Awake()
        {
            base.Awake();
            if (this == null) return;//物体已被删除(因为已存在)

            DontDestroyOnLoad(gameObject);

            //触发静态构造函数，使单例提前激活
            var bem = BuiltInEventManager.Instance;
            if (m_LocalState)
            {
                var mlm = MLocalizationManager.Instance;
            }

            if (m_PerformanceState)
            {
                style24 = MGUIStyleUtility.GetStyle(24);
                style30 = MGUIStyleUtility.GetStyle(30);
                monitor = new PerformanceMonitor(m_FPSDisplayMode, m_FPSSampleDuration);
            }

            //TODO:这样反射很耗，考虑其他方案
            initList = GetInterfaceInstanceList<INeedInit>();
            quitList = GetInterfaceInstanceList<INeedQuit>();

            //在主线程设置mainThread(Post()才能调回来)
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

        private void Update()
        {
            if (m_PerformanceState && monitor != null)
            {
                if (Input.GetKeyDown(PerformanceMonitor.ToKeycode(m_keycode)))
                {
                    showPerformance = !showPerformance;
                }
                monitor.Update();
            }
        }

        private void OnApplicationQuit()
        {
            foreach (INeedQuit instance in quitList)
            {
                instance.Quit();
            }
        }

        private void OnGUI()
        {
            if (m_PerformanceState && monitor != null && showPerformance)
            {
                if (monitor.CheckFPS)
                {
                    monitor.FPSResult(out float best, out float average, out float worst);
                    GUI.Label(new Rect(10, 10, 200, 30), "FPS", style30);
                    GUI.Label(new Rect(10, 50, 200, 30), string.Format("Average:{0:0}", average), style24);
                    GUI.Label(new Rect(10, 80, 200, 30), string.Format("Best:{0:0}", best), style24);
                    GUI.Label(new Rect(10, 110, 200, 30), string.Format("Worst:{0:0}", worst), style24);
                }
            }
        }

        /// <summary>
        /// 切换场景时关闭所有携程(防止切换时携程还在执行)
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