using System;
using MFramework.Core.CoreEx;

namespace MFramework.Text
{
    public sealed class MTextRuntimeService : IRuntimeService, IRuntimeServiceWithContext
    {
        private IDIContainer _container;

        public void BindContext(IRuntimeServiceContext context)
        {
            _container = context.Container;
        }

        public void Initialize()
        {
            if (_container == null)
            {
                throw new InvalidOperationException($"{nameof(MTextRuntimeService)} must be registered through MFrameworkCore before Initialize.");
            }
        }

        public void Shutdown()
        {
            _container = null;
        }
    }
}
