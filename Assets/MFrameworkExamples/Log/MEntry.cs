using MFramework.Core;

namespace MFrameworkExamples.Log
{
    public class MEntry : MEntryBase
    {
        // 一个类仅有一个ILog(即使有2个也是同一对象)
        private ILog _log = MLog.Create<MEntry>(MLog.LogFilter.Debug); // 可修改过滤等级

        // 默认Editor为Debug，打包后为Off，调用后可强制更改
        protected override MLog.LogFilter SetLogFilter()
        {
            return MLog.LogFilter.Debug;
        }

        protected override void OnBootstrapped(TrackerStoppedEvent e)
        {
            MLog.Default.D("Default输出"); // Default的LogFilter会跟着设置走
            
            _log.D("Debug");
            _log.W("Warning");
            _log.E("Error");
            _log.EX(LogException.NullReference);
            _log.EX(LogException.NullReference, MLog.LogLevel.Error); // 修改报错等级
        }
    }
}