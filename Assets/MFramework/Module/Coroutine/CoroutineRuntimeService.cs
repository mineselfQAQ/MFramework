using System;
using MFramework.Core.CoreEx;

namespace MFramework.Coroutines
{
    public class CoroutineRuntimeService : IRuntimeService, IRuntimeServiceWithContext
    {
        private IDIContainer _container;
        private MCoroutineManager _manager;

        public void BindContext(IRuntimeServiceContext context)
        {
            _container = context.Container;
        }

        public void Initialize()
        {
            if (_container == null)
            {
                throw new InvalidOperationException($"{nameof(CoroutineRuntimeService)} must be registered through MFrameworkCore before Initialize.");
            }

            _manager = _container.Resolve<MCoroutineManager>();
            _manager.EnsureRunner();
        }

        public void Shutdown()
        {
            _manager?.Shutdown();
            _manager = null;
        }
    }
}
