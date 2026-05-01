using System;
using MFramework.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MFramework
{
    public abstract class ResourceBase : CustomYieldInstruction, IResource
    {
        protected readonly MResourceManager ResourceManager;
        protected readonly MBundleManager BundleManager;

        protected ResourceBase(MResourceManager resourceManager, MBundleManager bundleManager)
        {
            ResourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
            BundleManager = bundleManager ?? throw new ArgumentNullException(nameof(bundleManager));
        }

        internal abstract void Load();
        internal abstract void UnLoad();
        internal abstract void LoadAsset();

        public virtual Object asset { get; protected set; }
        public string url { get; set; }
        internal bool done { get; set; } // 加载完成状态
        internal BundleBase bundle { get; set; }
        internal ResourceBase[] dependencies { get; set; }
        internal int reference { get; set; }
        internal Action<ResourceBase> finishedCallback { get; set; }
        internal ResourceAwaiter awaiter { get; set; }

        internal void AddReference()
        {
            ++reference;
        }
        internal void ReduceReference()
        {
            --reference;

            if (reference < 0)
            {
                MLog.Default?.E($"AB资源引用计数异常：引用计数小于0，asset={url}, reference={reference}");
            }
        }

        public Object GetAsset()
        {
            return asset;
        }
        public abstract T GetAsset<T>() where T : Object;

        public GameObject Instantiate(bool autoUnload = false)
        {
            Object obj = asset;
            if (!obj && !(obj is GameObject)) return null;
            GameObject go = Object.Instantiate(obj) as GameObject;

            if (autoUnload && go) // 添加自动卸载功能
            {
                AutoUnloadAB temp = go.AddComponent<AutoUnloadAB>();
                temp.resource = this;
                temp.resourceManager = ResourceManager;
            }

            return go;
        }
        public GameObject Instantiate(Vector3 position, Quaternion rotation, bool autoUnload = false)
        {
            Object obj = asset;
            if (!obj && !(obj is GameObject)) return null;

            GameObject go = Object.Instantiate(obj, position, rotation) as GameObject;
            if (autoUnload && go) // 添加自动卸载功能
            {
                AutoUnloadAB temp = go.AddComponent<AutoUnloadAB>();
                temp.resource = this;
                temp.resourceManager = ResourceManager;
            }

            return go;
        }
        public GameObject Instantiate(Transform parent, bool instantiateInWorldSpace, bool autoUnload = false)
        {
            Object obj = asset;
            if (!obj && !(obj is GameObject)) return null;
            GameObject go = Object.Instantiate(obj, parent, instantiateInWorldSpace) as GameObject;

            if (autoUnload && go) // 添加自动卸载功能
            {
                AutoUnloadAB temp = go.AddComponent<AutoUnloadAB>();
                temp.resource = this;
                temp.resourceManager = ResourceManager;
            }

            return go;
        }

        internal void FreshAsyncAsset()
        {
            if (done) return;

            if (dependencies != null)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    ResourceBase resource = dependencies[i];
                    resource.FreshAsyncAsset();
                }
            }

            if (this is ResourceBaseAsync)
            {
                LoadAsset();
            }
        }
    }
}
