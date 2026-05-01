using System;
using MFramework.Core.CoreEx;

namespace MFramework
{
    public sealed class HotUpdateRuntimeService : IRuntimeService, IRuntimeServiceWithContext
    {
        private readonly bool _startOnInitialize;
        private IDIContainer _container;
        private MHotUpdateManager _manager;

        public HotUpdateRuntimeService(bool startOnInitialize = false)
        {
            _startOnInitialize = startOnInitialize;
        }

        public void BindContext(IRuntimeServiceContext context)
        {
            _container = context.Container;
        }

        public void Initialize()
        {
            if (_container == null)
            {
                throw new InvalidOperationException($"{nameof(HotUpdateRuntimeService)} must be registered through MFrameworkCore before Initialize.");
            }

            _manager = _container.Resolve<MHotUpdateManager>();
            _manager.Initialize();

            MLuaInterpreter.Instance.Configure(_container.Resolve<ABRuntimeState>(), _container.Resolve<MResourceManager>());

            if (_startOnInitialize)
            {
                _manager.StartHotUpdate();
            }
        }

        public void Shutdown()
        {
            _manager?.Shutdown();
            _manager = null;
        }
    }
}
