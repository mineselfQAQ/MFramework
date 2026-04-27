using System;
using System.Collections.Generic;
using MFramework.Core.Event;
using MFramework.Core.Tracker;

namespace MFramework.Core.CoreEx
{
    public enum CoreState
    {
        None,
        Bootstrapping,
        Bootstrapped,
        Initializing,
        Running,
        ShuttingDown,
        ShutDown,
        Failed
    }

    /// <summary>
    /// 框架核心
    /// </summary>
    public class MFrameworkCore : IRuntimeServiceContext
    {
        private readonly List<IBootstrap> _bootstraps = new();
        private readonly List<IShutdown> _shutdowns = new();
        private readonly List<IRuntimeService> _runtimeServices = new();

        private readonly List<IRuntimeUpdateService> _runtimeUpdateServices = new();
        private readonly List<IRuntimeFixedUpdateService> _runtimeFixedUpdateServices = new();
        private readonly List<IRuntimeLateUpdateService> _runtimeLateUpdateServices = new();
        private readonly List<IRuntimeApplicationFocusService> _runtimeApplicationFocusServices = new();
        private readonly List<IRuntimeApplicationPauseService> _runtimeApplicationPauseServices = new();

        private readonly TrackerEventPublisher _trackerEventPublisher;
        private readonly TrackerCollector _trackerCollector = new TrackerCollector();
        private MTracker _runningTracker;

        private Action<TrackerStartedEvent> _onBootstrapping;
        private Action<TrackerStoppedEvent> _onBootstrapped;
        private Action<TrackerStartedEvent> _onInitializing;
        private Action<TrackerStoppedEvent> _onInitialized;
        private Action<TrackerStartedEvent> _onShuttingDown;
        private Action<TrackerStoppedEvent> _onShutDown;

        private CoreState _state = CoreState.None;

        protected MEventBus EventBus { get; } = new MEventBus();

        public CoreState State => _state;
        public bool IsApplicationPaused { get; private set; }
        public bool HasApplicationFocus { get; private set; } = true;

        public MFrameworkCore()
        {
            // Tip：Running不设置事件，开始即OnInitialized，结束即OnShuttingDown
            EventBus.RegisterSafe<TrackerStartedEvent>(OnBootstrapping);
            EventBus.RegisterSafe<TrackerStoppedEvent>(OnBootstrapped);
            EventBus.RegisterSafe<TrackerStartedEvent>(OnInitializing);
            EventBus.RegisterSafe<TrackerStoppedEvent>(OnInitialized);
            EventBus.RegisterSafe<TrackerStartedEvent>(OnShuttingDown);
            EventBus.RegisterSafe<TrackerStoppedEvent>(OnShutDown);

            _trackerEventPublisher = new TrackerEventPublisher(EventBus);
        }

        public void OnBootstrapping(Action<TrackerStartedEvent> action) => _onBootstrapping = action;
        public void OnBootstrapped(Action<TrackerStoppedEvent> action) => _onBootstrapped = action;
        public void OnInitializing(Action<TrackerStartedEvent> action) => _onInitializing = action;
        public void OnInitialized(Action<TrackerStoppedEvent> action) => _onInitialized = action;
        public void OnShuttingDown(Action<TrackerStartedEvent> action) => _onShuttingDown = action;
        public void OnShutDown(Action<TrackerStoppedEvent> action) => _onShutDown = action;

        protected virtual void OnBootstrapping(TrackerStartedEvent e)
        {
            if (e.Name == "BOOTSTRAP")
            {
                _onBootstrapping?.Invoke(e);
            }
        }
        protected virtual void OnBootstrapped(TrackerStoppedEvent e)
        {
            if (e.Name == "BOOTSTRAP")
            {
                _onBootstrapped?.Invoke(e);
            }
        }
        protected virtual void OnInitializing(TrackerStartedEvent e)
        {
            if (e.Name == "INITIALIZE")
            {
                _onInitializing?.Invoke(e);
            }
        }
        protected virtual void OnInitialized(TrackerStoppedEvent e)
        {
            if (e.Name == "INITIALIZE")
            {
                _onInitialized?.Invoke(e);
            }
        }
        protected virtual void OnShuttingDown(TrackerStartedEvent e)
        {
            if (e.Name == "SHUTDOWN")
            {
                _onShuttingDown?.Invoke(e);
            }
        }
        protected virtual void OnShutDown(TrackerStoppedEvent e)
        {
            if (e.Name == "SHUTDOWN")
            {
                _onShutDown?.Invoke(e);
            }
        }

        #region Core

        public virtual void Bootstrap()
        {
            EnsureState(CoreState.None);
            _state = CoreState.Bootstrapping;

            try
            {
                using (MTracker.StartNew(MTrackerFactory.CreateTracker(1, "BOOTSTRAP", _trackerEventPublisher, _trackerCollector)))
                {
                    foreach (var bootstrap in _bootstraps)
                    {
                        bootstrap.Bootstrap();
                    }
                }

                _state = CoreState.Bootstrapped;
            }
            catch
            {
                _state = CoreState.Failed;
                throw;
            }
        }

        public virtual void Initialize()
        {
            EnsureState(CoreState.Bootstrapped);
            _state = CoreState.Initializing;

            try
            {
                using (MTracker.StartNew(MTrackerFactory.CreateTracker(2, "INITIALIZE",  _trackerEventPublisher, _trackerCollector)))
                {
                    foreach (var service in _runtimeServices)
                    {
                        service.Initialize();
                    }
                }

                _runningTracker = MTrackerFactory.CreateTracker(3, "RUNNING", _trackerEventPublisher, _trackerCollector);
                _runningTracker.Start();

                _state = CoreState.Running;
            }
            catch
            {
                _state = CoreState.Failed;
                throw;
            }
        }

        public virtual void Shutdown()
        {
            if (_state == CoreState.None || _state == CoreState.ShutDown) return;
            if (_state == CoreState.Failed)
            {
                StopRunningTracker();
                return;
            }

            EnsureState(CoreState.Running);
            _state = CoreState.ShuttingDown;

            try
            {
                StopRunningTracker();

                using (MTracker.StartNew(MTrackerFactory.CreateTracker(4, "SHUTDOWN", _trackerEventPublisher, _trackerCollector)))
                {
                    foreach (var service in _runtimeServices)
                    {
                        service.Shutdown();
                    }
                    _runtimeServices.Clear();
                    _runtimeFixedUpdateServices.Clear();
                    _runtimeUpdateServices.Clear();
                    _runtimeLateUpdateServices.Clear();
                    _runtimeApplicationFocusServices.Clear();
                    _runtimeApplicationPauseServices.Clear();

                    foreach (var shutdown in _shutdowns)
                    {
                        shutdown.Shutdown();
                    }

                    _state = CoreState.ShutDown;
                }
            }
            catch
            {
                _state = CoreState.Failed;
                throw;
            }
        }

        public virtual void FixedUpdate()
        {
            if (_state != CoreState.Running) return;

            foreach (var service in _runtimeFixedUpdateServices)
            {
                service.FixedUpdate();
            }
        }

        public virtual void Update()
        {
            if (_state != CoreState.Running) return;

            foreach (var service in _runtimeUpdateServices)
            {
                service.Update();
            }
        }

        public virtual void LateUpdate()
        {
            if (_state != CoreState.Running) return;

            foreach (var service in _runtimeLateUpdateServices)
            {
                service.LateUpdate();
            }
        }

        public virtual void OnApplicationFocus(bool hasFocus)
        {
            if (_state != CoreState.Running) return;

            HasApplicationFocus = hasFocus;

            foreach (var service in _runtimeApplicationFocusServices)
            {
                service.OnApplicationFocus(hasFocus);
            }
        }

        public virtual void OnApplicationPause(bool pauseStatus)
        {
            if (_state != CoreState.Running) return;

            IsApplicationPaused = pauseStatus;

            foreach (var service in _runtimeApplicationPauseServices)
            {
                service.OnApplicationPause(pauseStatus);
            }
        }

        private void StopRunningTracker()
        {
            if (_runningTracker != null)
            {
                _runningTracker.Stop();
                _runningTracker = null;
            }
        }

        private void EnsureState(CoreState required)
        {
            if (_state != required)
            {
                throw new CSharpFrameworkException($"非法状态转换: 当前-{_state} 需要-{required}");
            }
        }

        public virtual void Register(IRuntimeService service)
        {
            _runtimeServices.Add(service);

            if (service is IRuntimeFixedUpdateService fixedUpdateService)
            {
                _runtimeFixedUpdateServices.Add(fixedUpdateService);
            }
            if (service is IRuntimeUpdateService updateService)
            {
                _runtimeUpdateServices.Add(updateService);
            }
            if (service is IRuntimeLateUpdateService lateUpdateService)
            {
                _runtimeLateUpdateServices.Add(lateUpdateService);
            }
            if (service is IRuntimeApplicationPauseService applicationPauseService)
            {
                _runtimeApplicationPauseServices.Add(applicationPauseService);
            }
            if (service is IRuntimeApplicationFocusService applicationFocusService)
            {
                _runtimeApplicationFocusServices.Add(applicationFocusService);
            }

            if (service is IRuntimeServiceWithContext serviceWithContext)
            {
                serviceWithContext.BindContext(this);
            }
        }

        public virtual void UnRegister(IRuntimeService service)
        {
            _runtimeServices.Remove(service);

            if (service is IRuntimeFixedUpdateService fixedUpdateService)
            {
                _runtimeFixedUpdateServices.Remove(fixedUpdateService);
            }
            if (service is IRuntimeUpdateService updateService)
            {
                _runtimeUpdateServices.Remove(updateService);
            }
            if (service is IRuntimeLateUpdateService lateUpdateService)
            {
                _runtimeLateUpdateServices.Remove(lateUpdateService);
            }
            if (service is IRuntimeApplicationPauseService applicationPauseService)
            {
                _runtimeApplicationPauseServices.Remove(applicationPauseService);
            }
            if (service is IRuntimeApplicationFocusService applicationFocusService)
            {
                _runtimeApplicationFocusServices.Remove(applicationFocusService);
            }
        }

        #endregion

        public void AddBootstrap(IBootstrap bootstrap)
        {
            if (bootstrap == null) return;

            _bootstraps.Add(bootstrap);
        }

        public void AddShutdown(IShutdown shutdown)
        {
            if (shutdown == null) return;

            _shutdowns.Add(shutdown);
        }

        public string GetLog()
        {
            return _trackerCollector.GetLog();
        }
    }
}
