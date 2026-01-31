using System;
using MFramework.Core;
using UnityEngine;
using IServiceProvider = MFramework.Core.IServiceProvider;

namespace MFrameworkExamples.Framework
{
    public class MEntry : MEntryBase
    {
        public GameObject go;
        protected override void OnBootstrapping(TrackerStartedEvent e)
        {
            MLog.Default.D($"开始启动 时间：{e.StartTime}");
        }

        protected override void OnBootstrapped(TrackerStoppedEvent e)
        {
            var p = go.transform.position;
            MLog.Default.D($"启动完成 时间：{e.EndTime}");
            MLog.Default.E(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
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

        protected override IBootstrap GetUserBootstrap()
        {
            var provider = new IServiceProvider[]
            {
                new TestServiceProvider(),
            };
            return new UserBootstrap(_core, provider);
        }

        protected override IShutdown GetUserShutDown()
        {
            return null;
        }
    }
}