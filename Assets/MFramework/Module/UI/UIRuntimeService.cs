using MFramework.Core.CoreEx;

namespace MFramework.UI
{
    public sealed class UIRuntimeService : IRuntimeService, IRuntimeUpdateService, IRuntimeServiceWithContext
    {
        private IDIContainer _container;
        private MUIManager _manager;

        public void BindContext(IRuntimeServiceContext context)
        {
            _container = context.Container;
        }

        public void Initialize()
        {
            _manager = _container.Resolve<MUIManager>();
            _manager.Initialize();
        }

        public void Update()
        {
            _manager?.Update();
        }

        public void Shutdown()
        {
            _manager?.Shutdown();
            _manager = null;
        }
    }
}
