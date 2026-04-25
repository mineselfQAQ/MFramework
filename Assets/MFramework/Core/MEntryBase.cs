using System;
using System.Collections.Generic;

using MFramework.Core.CoreEx;
using MFramework.Core.Tracker;
using UnityEngine;

namespace MFramework.Core
{
    /// <summary>
    /// 框架的唯一入口，一个项目仅应该存在一个MEntry
    /// MEntry本质上是MFramework的一层封装，操作由MFramework提供
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class MEntryBase : MonoBehaviour
    {
        protected MCore Core;

        protected void Awake()
        {
            Core = (MCore)CreateCore();
            // 事件注册
            Core.OnBootstrapping(OnBootstrapping);
            Core.OnBootstrapped(OnBootstrapped);
            Core.OnInitializing(OnInitializing);
            Core.OnInitialized(OnInitialized);
            Core.OnShuttingDown(OnShuttingDown);
            Core.OnShutDown(OnShutDown);
            // 流程项注册
            Core.AddBootstrap(new InternalBootstrap());
            Core.AddBootstrap(GetServiceBootstrap());
            Core.AddBootstrap(GetUserBootstrap());
            Core.AddShutdown(new InternalShutDown());
            Core.AddShutdown(GetUserShutDown());

            // 启动
            MLog.SetDefaultLogFilter(SetLogFilter());
            MLog.Bootstrap(); // 主动提前，使OnBootstrapping事件中可进行MLog操作
            Core.Bootstrap();
        }

        protected void Start()
        {
            Core.Initialize();
        }

        protected void FixedUpdate()
        {
            if (!CanDispatchUnityLifecycle()) return;

            Core.FixedUpdate();
            OnUnityFixedUpdate();
        }

        protected void Update()
        {
            if (!CanDispatchUnityLifecycle()) return;

            Core.Update();
            OnUnityUpdate();
        }

        protected void LateUpdate()
        {
            if (!CanDispatchUnityLifecycle()) return;

            Core.LateUpdate();
            OnUnityLateUpdate();
        }

        protected void OnApplicationFocus(bool hasFocus)
        {
            if (!CanDispatchUnityLifecycle()) return;

            Core.OnApplicationFocus(hasFocus);
            OnUnityApplicationFocus(hasFocus);
        }

        protected void OnApplicationPause(bool pauseStatus)
        {
            if (!CanDispatchUnityLifecycle()) return;

            Core.OnApplicationPause(pauseStatus);
            OnUnityApplicationPause(pauseStatus);
        }

        protected virtual void OnApplicationQuit()
        {
            // 终止
            Core.Shutdown();
            MLog.Shutdown(Core); // 主动延后，使MTrackerLog正确输出
        }

        protected virtual MFrameworkCore CreateCore()
        {
            return new MCore();
        }

        protected IBootstrap GetServiceBootstrap()
        {
            var services = ConfigureServices();
            return new ServiceBootstrap(Core, services);
        }

        /// <summary>
        /// 添加Module/ServiceProvider
        /// </summary>
        /// <returns></returns>
        protected abstract IManagedService[] ConfigureServices();

        protected virtual IBootstrap GetUserBootstrap()
        {
            return null;
        }

        protected virtual IShutdown GetUserShutDown()
        {
            return null;
        }

        protected virtual void OnBootstrapping(TrackerStartedEvent e) { }
        protected virtual void OnBootstrapped(TrackerStoppedEvent e) { }
        protected virtual void OnInitializing(TrackerStartedEvent e) { }
        protected virtual void OnInitialized(TrackerStoppedEvent e) { }
        protected virtual void OnShuttingDown(TrackerStartedEvent e) { }
        protected virtual void OnShutDown(TrackerStoppedEvent e) { }

        protected virtual void OnUnityUpdate() { }
        protected virtual void OnUnityFixedUpdate() { }
        protected virtual void OnUnityLateUpdate() { }
        protected virtual void OnUnityApplicationFocus(bool hasFocus) { }
        protected virtual void OnUnityApplicationPause(bool pauseStatus) { }

        protected virtual MLog.LogFilter SetLogFilter()
        {
            return MLog.BUILD_FILTER;
        }

        private bool CanDispatchUnityLifecycle()
        {
            return Core != null && Core.State == CoreState.Running;
        }
    }
}
