using System;

namespace MFramework.Core
{
    /// <summary>
    /// 框架层面Log
    /// </summary>
    public class InternalLog : LogBase
    {
        public InternalLog(string name) : base(name) { }
        
        public InternalLog(string name, MLog.MLogLevel logLevel) : base(name, logLevel) { }

        protected override string SrcName => "Internal";
    }
}