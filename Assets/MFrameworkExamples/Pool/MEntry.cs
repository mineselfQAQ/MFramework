using MFramework.Core;
using MFramework.Core.CoreEx;
using MFramework.Core.IOC;
using MFramework.Core.Tracker;
using MFramework.Pool;

using UnityEngine;

using IServiceProvider = MFramework.Core.CoreEx.IServiceProvider;

namespace MFrameworkExamples.Pool
{
    public class MEntry : MEntryBase
    {
        private GameObject _template;

        protected override void OnInitialized(TrackerStoppedEvent e)
        {
            var manager = MIOCContainer.Default.Resolve<MPoolManager>();

            _template = CreateTemplate();
            manager.WarmPool(_template, null, 2);

            ObjectPoolContainer<GameObject> first = manager.SpawnObject(_template, new Vector3(-2f, 0f, 0f), Quaternion.identity);
            ObjectPoolContainer<GameObject> second = manager.SpawnObject(_template, Vector3.zero, Quaternion.identity);

            manager.ReleaseObject(first);
            ObjectPoolContainer<GameObject> reused = manager.SpawnObject(_template, new Vector3(2f, 0f, 0f), Quaternion.identity);

            MLog.Default.D($"Pool 示例复用结果：{object.ReferenceEquals(first, reused)}");

            manager.ReleaseObject(second);
            manager.ReleaseObject(reused);
        }

        protected override void OnShutDown(TrackerStoppedEvent e)
        {
            if (_template != null)
            {
                Destroy(_template);
                _template = null;
            }
        }

        protected override IBootstrap GetUserBootstrap()
        {
            var providers = new IServiceProvider[]
            {
                new PoolServiceProvider(),
            };

            return new UserBootstrap(Core, providers);
        }

        protected override IShutdown GetUserShutDown()
        {
            return null;
        }

        private static GameObject CreateTemplate()
        {
            GameObject template = GameObject.CreatePrimitive(PrimitiveType.Cube);
            template.name = "PoolCubeTemplate";
            template.SetActive(false);
            return template;
        }
    }
}
