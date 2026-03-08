using MFramework.Core.CoreEx;

namespace MFramework.Core
{
    /// <summary>
    /// Unity层扩展
    /// </summary>
    public class MCore : MFrameworkCore
    {
        private static readonly ILog _log = new InternalLog(nameof(MCore)); 
        
        public MCore()
        {
            EventBus.LogError = (message) =>
            {
                _log.E(message);
            };
        }
    }
}