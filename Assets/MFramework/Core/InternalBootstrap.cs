using MFramework.Core.CoreEx;

namespace MFramework.Core
{
    public class InternalBootstrap : IBootstrap
    {

        public InternalBootstrap()
        {

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
