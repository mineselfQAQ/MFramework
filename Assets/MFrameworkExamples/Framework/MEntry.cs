using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Core.IOC;
using MFramework.Core.Tracker;

using UnityEngine;

namespace MFrameworkExamples.Framework
{
    public class MEntry : MEntryBase
    {
        protected override void OnBootstrapping(TrackerStartedEvent e)
        {
            MLog.Default.D($"开始启动 时间：{e.StartTime}");
        }

        protected override void OnBootstrapped(TrackerStoppedEvent e)
        {
            MLog.Default.D($"启动完成 时间：{e.EndTime}");
        }

        protected override void OnInitializing(TrackerStartedEvent e)
        {
            MLog.Default.D($"开始初始化 时间：{e.StartTime}");
        }

        protected override void OnInitialized(TrackerStoppedEvent e)
        {
            MLog.Default.D($"初始化完成 时间：{e.EndTime}");

            var container = MIOCContainer.Default;
            container.Resolve<A>().Print();
        }

        protected override void OnShuttingDown(TrackerStartedEvent e)
        {
            MLog.Default.D($"开始退出 时间：{e.StartTime}");
        }

        protected override void OnShutDown(TrackerStoppedEvent e)
        {
            MLog.Default.D($"退出完成 时间：{e.EndTime}");
        }

        protected override void OnUnityUpdate()
        {
            if(Input.GetKeyDown(KeyCode.Space))
                MLog.Default.D("MEntry-UPDATE");
        }

        protected override IModule[] ConfigureModules()
        {
            return new IModule[]
            {
                new TestFrameworkModule(),
            };
        }
    }
}
