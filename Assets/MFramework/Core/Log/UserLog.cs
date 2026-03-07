using System;
using MFramework.Core.Internal;

namespace MFramework.Core
{
    /// <summary>
    /// 开发者Log
    /// </summary>
    public class UserLog : LogBase
    {
        public UserLog(string name) : base(name) { }
        
        public UserLog(string name, MLog.LogFilter logFilter) : base(name, logFilter) { }

        protected override string SrcName => Consts.UserName;
    }
}