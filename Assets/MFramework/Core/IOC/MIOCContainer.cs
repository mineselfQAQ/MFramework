using System.Collections.Generic;
using UnityEngine;

namespace MFramework.Core
{
    public class MIOCContainer
    {
        public enum IOCMode
        {
            SL, // 服务定位器 Service Locator
            DI, // 依赖反转 Dependency Injection
        }
        
        public static MSLContainer Default = CreateSL(); // TODO：DI实现后改为DI

        private readonly Dictionary<string, MSLContainer> _slContainers = new Dictionary<string, MSLContainer>();
        
        public static MSLContainer CreateSL()
        {
            return new MSLContainer();
        }
        
        public static MDIContainer CreateDI()
        {
            return null; // TODO：待实现
        }
    }
}
