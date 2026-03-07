using MFramework.Core.Event;
using MFramework.Core.Internal;
using UnityEngine;

namespace MFramework.Core
{
    /// <summary>
    /// 框架层面Log
    /// <para>
    /// InternalLog遵循DefaultLogFilter，具有相同的等级
    /// </para>
    /// </summary>
    public class InternalLog : LogBase
    {
        public InternalLog(string name) : base(name) { }
        
        public InternalLog(string name, MLog.LogFilter logFilter) : base(name, logFilter) { }

        protected override string SrcName => Consts.InternalName;
    }
}