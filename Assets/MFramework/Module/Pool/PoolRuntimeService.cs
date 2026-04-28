using System;
using MFramework.Core.CoreEx;

namespace MFramework.Pool
{
    public class PoolRuntimeService : IRuntimeService, IRuntimeServiceWithContext
    {
        private IDIContainer _container;
        private MPoolManager _manager;

        public void BindContext(IRuntimeServiceContext context)
        {
            _container = context.Container;
        }

        public void Initialize()
        {
            if (_container == null)
            {
                throw new InvalidOperationException($"{nameof(PoolRuntimeService)} must be registered through MFrameworkCore before Initialize.");
            }

            _manager = _container.Resolve<MPoolManager>();
        }

        public void Shutdown()
        {
            _manager?.Shutdown();
            _manager = null;
        }
    }
}
