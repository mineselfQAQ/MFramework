using MFramework.Core.CoreEx;
using MFramework.Core.IOC;

namespace MFramework.Core
{
    /// <summary>
    /// Unity层扩展
    /// </summary>
    public class MCore : MFrameworkCore
    {
        private static readonly ILog _log = new InternalLog(nameof(MCore)); 
        
        public MCore()
            : base(MIOCContainer.CreateDI())
        {
            EventBus.LogError = (message) =>
            {
                _log.E(message);
            };
        }
    }
}
