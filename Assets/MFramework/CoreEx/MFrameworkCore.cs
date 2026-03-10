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
    public class MFrameworkCore
    {
        private readonly List<IBootstrap> _bootstraps = new List<IBootstrap>();
        private readonly List<IShutdown> _shutdowns = new List<IShutdown>();
        private readonly List<IServiceProvider> _loadedServiceProviders = new List<IServiceProvider>();

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
                    foreach (var serviceProvider in _loadedServiceProviders)
                    {
                        serviceProvider.Initialize();
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
                    foreach (var serviceProvider in _loadedServiceProviders)
                    {
                        serviceProvider.Shutdown();
                        serviceProvider.Unregister();
                    }
                    _loadedServiceProviders.Clear();

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

        public virtual void Register(IServiceProvider serviceProvider)
        {
            serviceProvider.Register();
            _loadedServiceProviders.Add(serviceProvider);
        }


        public virtual void UnRegister(IServiceProvider serviceProvider)
        {
            serviceProvider.Unregister();
            _loadedServiceProviders.Remove(serviceProvider);
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