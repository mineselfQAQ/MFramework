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
        
        // TODO：特例，不清楚会不会出现无论用户怎么设置都一定需要输出的情况
        public InternalLog(string name, MLog.LogFilter logFilter) : base(name, logFilter) { }

        protected override string SrcName => "I";
    }
}