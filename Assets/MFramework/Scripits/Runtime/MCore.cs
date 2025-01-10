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
        [SerializeField]
        private bool m_ABState;//�Ƿ�����AB
        [SerializeField]
        private bool m_ABEncryptState;//�Ƿ�����AB����
        [SerializeField]
        private bool m_AutoHotUpdateState;//�Ƿ������������Զ��ȸ�

        [SerializeField]
        private bool m_PerformanceState;//�Ƿ��������ܼ��
        [SerializeField] private FPSMonitor.DisplayMode m_FPSDisplayMode = FPSMonitor.DisplayMode.FPS;
        [SerializeField] private float m_FPSSampleDuration = 1.0f;
        [SerializeField] private PerformanceMonitor.PKeycode m_keycode = PerformanceMonitor.PKeycode.Backspace;
        //TODO:���ܼ����ϸ��(FPS/CPU/GPU/���ݼ�����)

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
            if (this == null) return;//�����ѱ�ɾ��(��Ϊ�Ѵ���)
            DontDestroyOnLoad(gameObject);

            InitializeAB();
            InitializeMonoSingleton();
            InitializeMonitor();
            InitializeInterface();
            InitializeVSync(600);//TODO:��ͬƽ̨��ͬ���ã���������

            MainThreadUtility.SetMainThread();//�����߳�����mainThread(Post()���ܵ�����)
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
        /// ��ʼ��AB(����������+��ʼ��)
        /// </summary>
        private void InitializeAB()
        {
            //Tip��
            //��ʼ����ʽΪ�ȳ�ʼ����ʼ��(��WINDOW�ļ���manifest.ab)��������Ϊ��������
            //�ȸ��º���滻�ļ���������滻������Ҫ������Щ��Ϣ�ļ����ٴγ�ʼ��(��֤��Ϣ��ȫ)

            if (ABState)
            {
                MHotUpdateManager.Instance.OnUpdateEnd += () =>
                {
                    isHotUpdateFinish.Value = true;
                };

                //��ʼ����ʼ��
                MResourceManager.Instance.Initialize(MABUtility.GetPlatform(), GetFileUrl, 0);
                //�ȸ���
                MHotUpdateManager.Instance.Initialize();
                if (AutoHotUpdateState)
                {
                    MHotUpdateManager.Instance.StartHotUpdate();
                    MCoroutineManager.Instance.WaitNoRecord(() =>
                    {
                        MResourceManager.Instance.Initialize(MABUtility.GetPlatform(), GetFileUrl, 0);
                    }, isHotUpdateFinish);
                }
                else//�ٳ�ʼ��
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
        /// ��MonoSingleton������̬���캯��(ʹ������ǰ����)
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
        /// ��ʼ�����ܼ����
        /// </summary>
        private void InitializeMonitor()
        {
            if (PerformanceState)
            {
                monitor = new PerformanceMonitor(m_FPSDisplayMode, m_FPSSampleDuration);
            }
        }
        /// <summary>
        /// ��ȡ��Ҫ��ʼ�����˳����б�
        /// </summary>
        private void InitializeInterface()
        {
            //TODO:��������ܺģ�������������
            initList = GetInterfaceInstanceList<INeedInit>();
            quitList = GetInterfaceInstanceList<INeedQuit>();
        }
        private void InitializeVSync(int maxFrameRate)
        {
            int vSyncLevel = QualitySettings.vSyncCount;
            QualitySettings.vSyncCount = 0;//�رմ�ֱͬ��
            if (vSyncLevel != 0) MLog.Print($"{typeof(MCore)}����ֱͬ��������--->�ر�");

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