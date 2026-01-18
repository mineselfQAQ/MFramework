namespace MFramework.Core
{
    public class InternalShutDown : IShutdown
    {
        public void Shutdown()
        {
            MLog.Shutdown();
        }
    }
}