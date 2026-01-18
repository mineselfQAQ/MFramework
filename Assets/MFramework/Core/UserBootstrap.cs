using MFramework.Core;

namespace MFramework.Core
{
    public class UserBootstrap : IBootstrap
    {
        private readonly MFrameworkCore _core;
        private readonly IServiceProvider[] _serviceProviders;
        
        public UserBootstrap(MFrameworkCore core, IServiceProvider[] serviceProviders)
        {
            _core = core;
            _serviceProviders = serviceProviders;
        }
        
        public void Bootstrap()
        {
            foreach (IServiceProvider serviceProvider in _serviceProviders)
            {
                _core.Register(serviceProvider);
            }
        }
    }
}