using System.Collections.Generic;

namespace MFramework.Core.IOC
{
    public class MIOCContainer
    {
        public enum IOCMode
        {
            SL, // 服务定位器 Service Locator
            DI, // 依赖反转 Dependency Injection
        }
        
        public static MDIContainer Default = CreateDI(); // TODO：DI实现后改为DI

        private readonly Dictionary<string, MSLContainer> _slContainers = new Dictionary<string, MSLContainer>();
        
        public static MSLContainer CreateSL()
        {
            return new MSLContainer();
        }
        
        public static MDIContainer CreateDI()
        {
            return new MDIContainer();
        }
    }
}
