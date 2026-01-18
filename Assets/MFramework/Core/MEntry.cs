using System;
using UnityEngine;

namespace MFramework.Core
{
    /// <summary>
    /// 框架的唯一入口，一个项目仅应该存在一个MEntry
    /// MEntry本质上是MFramework的一层封装，操作由MFramework提供
    /// </summary>
    public abstract class MEntryBase : MonoBehaviour
    {
        protected MCore _core;
        
        protected void Awake()
        {
            _core = (MCore)CreateCore();
            // 事件注册
            _core.OnBootstrapping(OnBootstrapping);
            _core.OnBootstrapped(OnBootstrapped);
            _core.OnInitializing(OnInitializing);
            _core.OnInitialized(OnInitialized);
            _core.OnShuttingDown(OnShuttingDown);
            _core.OnShutDown(OnShutDown);
            // 流程项注册
            _core.AddBootstrap(new InternalBootstrap(_core));
            _core.AddBootstrap(GetUserBootstrap());
            _core.AddShutdown(new InternalShutDown());
            _core.AddShutdown(GetUserShutDown());
            
            // 启动
            _core.Bootstrap();
        }

        protected void Start()
        {
            _core.Initialize();
        }

        protected void Update()
        {
            
        }

        protected virtual void OnApplicationQuit()
        {
            _core.Shutdown();
        }

        protected virtual MFrameworkCore CreateCore()
        {
            return new MCore();
        }
        
        protected abstract IBootstrap GetUserBootstrap();
        protected abstract IShutdown GetUserShutDown();
        
        protected virtual void OnBootstrapping(TrackerStartedEvent e) { }
        protected virtual void OnBootstrapped(TrackerStoppedEvent e) { }
        protected virtual void OnInitializing(TrackerStartedEvent e) { }
        protected virtual void OnInitialized(TrackerStoppedEvent e) { }
        protected virtual void OnShuttingDown(TrackerStartedEvent e) { }
        protected virtual void OnShutDown(TrackerStoppedEvent e) { }
    }
}