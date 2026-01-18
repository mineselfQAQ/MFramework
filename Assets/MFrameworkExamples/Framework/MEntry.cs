using System;
using MFramework.Core;
using UnityEngine;
using IServiceProvider = MFramework.Core.IServiceProvider;

namespace MFrameworkExamples.Framework
{
    public class MEntry : MEntryBase
    {
        public GameObject go;
        protected override void OnBootstrapped(TrackerStoppedEvent e)
        {
            MLog.Default.D("启动完成");
            MLog.Default.D(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            MLog.Default.D(go.name);
        }

        protected override void OnInitialized(TrackerStoppedEvent e)
        {
            MLog.Default.D("初始化完成");
            
            var container = MIOCContainer.Default;
            container.Resolve<A>().Print();
            
            MLog.Default.E("出错了");
            MLog.Default.E("出错了");
            MLog.Default.D("OK");
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