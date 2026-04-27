using MFramework.Core.CoreEx;

namespace MFramework.Pool
{
    public class PoolRuntimeService : IRuntimeService
    {
        private readonly MPoolManager _manager;

        public PoolRuntimeService(MPoolManager manager)
        {
            _manager = manager;
        }

        public void Initialize()
        {
        }

        public void Shutdown()
        {
            _manager.Shutdown();
        }
    }
}
