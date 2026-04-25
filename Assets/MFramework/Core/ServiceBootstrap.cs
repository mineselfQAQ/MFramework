using MFramework.Core.CoreEx;

namespace MFramework.Core
{
    public class ServiceBootstrap : IBootstrap
    {
        private readonly MFrameworkCore _core;
        private readonly IManagedService[] _services;

        public ServiceBootstrap(MFrameworkCore core, IManagedService[] services)
        {
            _core = core;
            _services = services;
        }

        public void Bootstrap()
        {
            foreach (IManagedService serviceProvider in _services)
            {
                _core.Register(serviceProvider);
            }
        }
    }
}
