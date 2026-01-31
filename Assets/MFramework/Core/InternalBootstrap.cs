using UnityEngine;

namespace MFramework.Core
{
    public class InternalBootstrap : IBootstrap
    {
        private MFrameworkCore _core;
        
        public InternalBootstrap(MFrameworkCore core)
        {
            _core = core;
        }
        
        public void Bootstrap()
        {
            DoStaticBootstrap();
        }

        private void DoStaticBootstrap()
        {
            // MLog.Bootstrap();
        }

        private void DoRegister()
        {
            
        }
    }
}