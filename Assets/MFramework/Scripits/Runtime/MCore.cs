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
        private bool m_ABState;//是否启用AB
        [SerializeField]
        private bool m_ABEncryptState;//是否启用AB加密
        [SerializeField]
        private bool m_AutoHotUpdateState;//是否启用启动后自动热更

        [SerializeField]
        private bool m_PerformanceState;//是否启用性能检测
        [SerializeField] private FPSMonitor.DisplayMode m_FPSDisplayMode = FPSMonitor.DisplayMode.FPS;
        [SerializeField] private float m_FPSSampleDuration = 1.0f;
        [SerializeField] private PerformanceMonitor.PKeycode m_keycode = PerformanceMonitor.PKeycode.Backspace;
        //TODO:性能检测需细分(FPS/CPU/GPU/数据检测输出)

        private bool showPerformance = true;

        private PerformanceMonitor monitor = null;

        private List<INeedInit> initList;
        private List<INeedQuit> quitList;

        public bool LogState => m_LogState;
        public bool UICustomLoadState => m_UICustomLoadState;
        public bool LocalState => m_LocalState;
        public bool ABEncryptState => m_ABEncryptState;
        public bool ABState => m_ABState;
        public bool AutoHotUpdateState => m_AutoHotUpdateState;
        public bool PerformanceState => m_PerformanceState;

        public BoolWrapper isHotUpdateFinish = new BoolWrapper(false);

        protected override void Awake()
        {
            base.Awake();
            if (this == null) return;//物体已被删除(因为已存在)
            DontDestroyOnLoad(gameObject);

            InitializeAB();
            InitializeMonoSingleton();
            InitializeMonitor();
            InitializeInterface();
            InitializeVSync(600);//TODO:不同平台不同设置，公开参数

            MainThreadUtility.SetMainThread();//在主线程设置mainThread(Post()才能调回来)
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            DoInit();
        }

        private void Update()
        {
            HandleAB();
            HandleMonitor();
        }

        private void LateUpdate()
        {
            HandleLateAB();
        }

        private void OnApplicationQuit()
        {
            DoQuit();
            ABQuit();
        }

        private void OnGUI()
        {
            DrawMonitor();
        }

        /// <summary>
        /// 初始化AB(服务器下载+初始化)
        /// </summary>
        private void InitializeAB()
        {
            //Tip：
            //初始化方式为先初始化初始包(如WINDOW文件与manifest.ab)，该内容为不完整的
            //热更新后会替换文件，如果有替换，则需要更新这些信息文件并再次初始化(保证信息完全)

            if (ABState)
            {
                MHotUpdateManager.Instance.OnUpdateEnd += () =>
                {
                    isHotUpdateFinish.Value = true;
                };

                //初始包初始化
                MResourceManager.Instance.Initialize(MABUtility.GetPlatform(), GetFileUrl, 0);
                //先更新
                MHotUpdateManager.Instance.Initialize();
                if (AutoHotUpdateState)
                {
                    MHotUpdateManager.Instance.StartHotUpdate();
                    MCoroutineManager.Instance.WaitNoRecord(() =>
                    {
                        MResourceManager.Instance.Initialize(MABUtility.GetPlatform(), GetFileUrl, 0);
                    }, isHotUpdateFinish);
                }
                else//再初始化
                {
                    MResourceManager.Instance.Initialize(MABUtility.GetPlatform(), GetFileUrl, 0);
                }
            }
        }
        protected string GetFileUrl(string fileName)
        {
            string abRootPath = MABUtility.GetABRootPath();

            return $"{abRootPath}/{fileName}";
        }

        /// <summary>
        /// 对MonoSingleton触发静态构造函数(使单例提前激活)
        /// </summary>
        private void InitializeMonoSingleton()
        {
            var bem = BuiltInEventManager.Instance;
            if (LocalState)
            {
                var mlm = MLocalizationManager.Instance;
            }
        }
        /// <summary>
        /// 初始化性能检测器
        /// </summary>
        private void InitializeMonitor()
        {
            if (PerformanceState)
            {
                monitor = new PerformanceMonitor(m_FPSDisplayMode, m_FPSSampleDuration);
            }
        }
        /// <summary>
        /// 获取需要初始化或退出的列表
        /// </summary>
        private void InitializeInterface()
        {
            //TODO:这样反射很耗，考虑其他方案
            initList = GetInterfaceInstanceList<INeedInit>();
            quitList = GetInterfaceInstanceList<INeedQuit>();
        }
        private void InitializeVSync(int maxFrameRate)
        {
            int vSyncLevel = QualitySettings.vSyncCount;
            QualitySettings.vSyncCount = 0;//关闭垂直同步
            if (vSyncLevel != 0) MLog.Print($"{typeof(MCore)}：垂直同步：开启--->关闭");

            Application.targetFrameRate = maxFrameRate;
        }

        private void HandleAB()
        {
            if (ABState)
            {
                MResourceManager.Instance.Update();
            }
        }
        private void HandleLateAB()
        {
            if (ABState)
            {
                MResourceManager.Instance.LateUpdate();
            }
        }
        private void HandleMonitor()
        {
            if (PerformanceState && monitor != null)
            {
                if (Input.GetKeyDown(PerformanceMonitor.ToKeycode(m_keycode)))
                {
                    showPerformance = !showPerformance;
                }
                monitor.Update();
            }
        }

        private void ABQuit()
        {
            if (ABState)
            {
                foreach (var resource in MResourceManager.Instance.resourceDic.Values)
                {
                    MResourceManager.Instance.Unload(resource);
                }
            }
        }

        private void DoInit()
        {
            foreach (INeedInit instance in initList)
            {
                instance.Init();
            }
        }
        private void DoQuit()
        {
            foreach (INeedQuit instance in quitList)
            {
                instance.Quit();
            }
        }

        private void DrawMonitor()
        {
            if (PerformanceState && monitor != null && showPerformance)
            {
                monitor.Draw();
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