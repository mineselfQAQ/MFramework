using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

using MFramework.Core;
namespace MFramework
{
    public class MResourceManager
    {
        private readonly MBundleManager _bundleManager;
        private readonly ABRuntimeState _runtimeState;

        /// <summary>
        /// 保存资源对应的bundle（key---资源 value---所在bundle）
        /// </summary>
        internal Dictionary<string, string> ResourceBunldeDic = new Dictionary<string, string>();
        /// <summary>
        /// 保存资源的依赖关系key---具有依赖的资源value---所依赖的资源
        /// </summary>
        internal Dictionary<string, List<string>> ResourceDependencyDic = new Dictionary<string, List<string>>();

        // 当前存在的resource
        internal Dictionary<string, ResourceBase> resourceDic = new Dictionary<string, ResourceBase>();
        // 异步列表（正在加载的resource）
        private List<ResourceBaseAsync> asyncList = new List<ResourceBaseAsync>();
        // 卸载列表（正在卸载的resource）
        private LinkedList<ResourceBase> unloadList = new LinkedList<ResourceBase>();

        public MResourceManager(MBundleManager bundleManager, ABRuntimeState runtimeState)
        {
            _bundleManager = bundleManager ?? throw new ArgumentNullException(nameof(bundleManager));
            _runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
        }

        private const string MANIFEST_NAME = "manifest.ab";
        private static readonly string RESOURCEASSET_NAME = MPathCache.AB_RESOURCE_ASSET_NAME;
        private static readonly string BUNDLEASSET_NAME = MPathCache.AB_BUNDLE_ASSET_NAME;
        private static readonly string DEPENDENCYASSET_NAME = MPathCache.AB_DEPENDENCY_ASSET_NAME;

        internal void Initialize(string platform, Func<string, string> getFileCallback, ulong offset)
        {
            // 获取BundleManager.assetBundleManifest信息
            _bundleManager.Initialize(platform, getFileCallback, offset);

            // 获取manifest.ab并加载成AssetBundle
            string manifestBunldeFile = getFileCallback.Invoke(MANIFEST_NAME);
            AssetBundle manifestAssetBundle = MABUtility.LoadAB(manifestBunldeFile, _runtimeState, offset);
            // 通过manifest.ab获取内部信息（打包时记录的文字信息）
            TextAsset resourceTextAsset = manifestAssetBundle.LoadAsset(RESOURCEASSET_NAME) as TextAsset;
            TextAsset bundleTextAsset = manifestAssetBundle.LoadAsset(BUNDLEASSET_NAME) as TextAsset;
            TextAsset dependencyTextAsset = manifestAssetBundle.LoadAsset(DEPENDENCYASSET_NAME) as TextAsset;
            byte[] resourceBytes = resourceTextAsset.bytes;
            byte[] bundleBytes = bundleTextAsset.bytes;
            byte[] dependencyBytes = dependencyTextAsset.bytes;
            // 提取完毕后卸载
            manifestAssetBundle.Unload(true);
            manifestAssetBundle = null;

            // -----提取信息-----
            Dictionary<ushort, string> assetUrlDic = new Dictionary<ushort, string>();
            #region 读取资源信息
            {
                MemoryStream resourceMemoryStream = new MemoryStream(resourceBytes);
                BinaryReader resourceBinaryReader = new BinaryReader(resourceMemoryStream);
                // 获取资源个数
                ushort resourceCount = resourceBinaryReader.ReadUInt16();
                for (ushort i = 0; i < resourceCount; i++)
                {
                    string assetUrl = resourceBinaryReader.ReadString();
                    assetUrlDic.Add(i, assetUrl);
                }
            }
            #endregion
            #region 读取bundle信息
            {
                ResourceBunldeDic.Clear();
                MemoryStream bundleMemoryStream = new MemoryStream(bundleBytes);
                BinaryReader bundleBinaryReader = new BinaryReader(bundleMemoryStream);
                // 获取bundle个数
                ushort bundleCount = bundleBinaryReader.ReadUInt16();
                for (int i = 0; i < bundleCount; i++)
                {
                    string bundleUrl = bundleBinaryReader.ReadString();
                    string bundleFileUrl = bundleUrl;
                    // 获取bundle内的资源个数
                    ushort resourceCount = bundleBinaryReader.ReadUInt16();
                    for (int j = 0; j < resourceCount; j++)
                    {
                        ushort assetId = bundleBinaryReader.ReadUInt16();
                        string assetUrl = assetUrlDic[assetId];
                        ResourceBunldeDic.Add(assetUrl, bundleFileUrl);
                    }
                }
            }
            #endregion
            #region 读取资源依赖信息
            {
                ResourceDependencyDic.Clear();
                MemoryStream dependencyMemoryStream = new MemoryStream(dependencyBytes);
                BinaryReader dependencyBinaryReader = new BinaryReader(dependencyMemoryStream);
                // 获取依赖链个数
                ushort dependencyCount = dependencyBinaryReader.ReadUInt16();
                for (int i = 0; i < dependencyCount; i++)
                {
                    // 获取资源个数
                    ushort resourceCount = dependencyBinaryReader.ReadUInt16();
                    ushort assetId = dependencyBinaryReader.ReadUInt16();
                    string assetUrl = assetUrlDic[assetId];
                    List<string> dependencyList = new List<string>(resourceCount);
                    for (int j = 1; j < resourceCount; j++) // 不包括自己
                    {
                        ushort dependencyAssetId = dependencyBinaryReader.ReadUInt16();
                        string dependencyUrl = assetUrlDic[dependencyAssetId];
                        dependencyList.Add(dependencyUrl);
                    }

                    ResourceDependencyDic.Add(assetUrl, dependencyList);
                }
            }
            #endregion
        }

        internal void Update()
        {
            _bundleManager.Update(); // 同ResourceManager.Update（），持续加载Bundle

            for (int i = 0; i < asyncList.Count; i++)
            {
                ResourceBaseAsync resourceAsync = asyncList[i];
                if (resourceAsync.Update()) // 等待异步资源加载完成
                {
                    asyncList.RemoveAt(i);
                    i--;

                    if (resourceAsync.awaiter != null)
                    {
                        resourceAsync.awaiter.SetResult(resourceAsync as IResource);
                    }
                }
            }
        }

        internal void LateUpdate()
        {
            // 存在需要释放的资源
            while (unloadList.Count > 0) // 逐个操作
            {
                ResourceBase resource = unloadList.First.Value;
                unloadList.RemoveFirst();
                if (resource == null) continue;

                resourceDic.Remove(resource.url);
                resource.UnLoad(); // 卸载资源（主要为将bundle加入unloadList）

                // 处理依赖
                if (resource.dependencies != null)
                {
                    for (int i = 0; i < resource.dependencies.Length; i++)
                    {
                        ResourceBase temp = resource.dependencies[i];
                        Unload(temp); // 卸载（reference--，如果没有引用就加入unloadList准备卸载）
                    }
                }
            }

            _bundleManager.LateUpdate(); // Bundle的卸载
        }

        /// <summary>
        /// 加载资源，如需异步，需要使用协程
        /// </summary>
        public IResource Load(string url, bool async)
        {
            return LoadInternal(url, async);
        }
        /// <summary>
        /// 加载资源（使用名称，如lua.ab），如需异步，需要使用协程
        /// </summary>
        public IResource LoadByName(string name, bool async)
        {
            return LoadByNameInternal(name, async);
        }
        /// <summary>
        /// 加载资源（具有回调）
        /// </summary>
        public void LoadWithCallback(string url, bool async, Action<IResource> callback)
        {
            ResourceBase resource = LoadInternal(url, async);

            if (resource.done) // 同步情况
            {
                callback?.Invoke(resource);
            }
            else // 异步情况，等待结束后执行回调
            {
                resource.finishedCallback += callback;
            }
        }
        /// <summary>
        /// 加载资源（使用Awaiter）
        /// </summary>
        public ResourceAwaiter LoadWithAwaiter(string url)
        {
            ResourceBase resource = LoadInternal(url, true);

            // 资源加载完成，可返回
            if (resource.done)
            {
                if (resource.awaiter == null)
                {
                    resource.awaiter = new ResourceAwaiter();
                    resource.awaiter.SetResult(resource as IResource);
                }

                return resource.awaiter;
            }

            // 第一次加载，创建ResourceAwaiter
            if (resource.awaiter == null)
            {
                resource.awaiter = new ResourceAwaiter();
            }

            return resource.awaiter;
        }

        private ResourceBase LoadInternal(string url, bool async)
        {
            // 尝试获取Resource
            ResourceBase resource = null;
            if (resourceDic.TryGetValue(url, out resource))
            {
                // 已经准备卸载，但是现在又需要使用，直接取回
                if (resource.reference == 0)
                {
                    unloadList.Remove(resource);
                }
                resource.AddReference();

                return resource;
            }

            // 创建Resource
            if (async)
            {
                ResourceAsync resourceAsync = new ResourceAsync(this, _bundleManager); // 异步加载方式
                asyncList.Add(resourceAsync);
                resource = resourceAsync;
            }
            else
            {
                resource = new Resource(this, _bundleManager); // 同步加载方式
            }
            resource.url = url;
            resourceDic.Add(url, resource);

            // 加载依赖
            List<string> dependencies = null;
            ResourceDependencyDic.TryGetValue(url, out dependencies);
            if (dependencies != null && dependencies.Count > 0) // 如果该url有依赖就进行添加
            {
                resource.dependencies = new ResourceBase[dependencies.Count];
                for (int i = 0; i < dependencies.Count; i++)
                {
                    string dependencyUrl = dependencies[i];
                    ResourceBase dependencyResource = LoadInternal(dependencyUrl, async);
                    resource.dependencies[i] = dependencyResource;
                }
            }

            resource.AddReference();
            resource.Load(); // **实际resource的Load（）**

            return resource;
        }
        private ResourceBase LoadByNameInternal(string name, bool async)
        {
            foreach (string resourceName in ResourceBunldeDic.Keys)
            {
                if (resourceName.Contains(name))
                {
                    return LoadInternal(resourceName, async);
                }
            }

            return null;
        }

        public void Unload(string assetUrl)
        {
            if (string.IsNullOrEmpty(assetUrl))
            {
                MLog.Default?.E("AB资源卸载失败：资源路径为空");
            }

            ResourceBase resource;
            if (!resourceDic.TryGetValue(assetUrl, out resource))
            {
                MLog.Default?.E($"AB资源卸载失败：资源不存在，asset={assetUrl}");
            }
            Unload(resource);
        }
        public void Unload(IResource resource)
        {
            if (resource == null)
            {
                MLog.Default?.E("AB资源卸载失败：资源对象为空");
            }

            // 该Resource的reference--，如果为0（没有引用），即可准备卸载
            ResourceBase resourceBase = resource as ResourceBase;
            resourceBase.ReduceReference();
            if (resourceBase.reference == 0)
            {
                WillUnload(resourceBase);
            }
        }

        private void WillUnload(ResourceBase resource)
        {
            unloadList.AddLast(resource);
        }

        internal void Shutdown()
        {
            resourceDic.Clear();
            asyncList.Clear();
            unloadList.Clear();
            ResourceBunldeDic.Clear();
            ResourceDependencyDic.Clear();
            _bundleManager.Shutdown();

        }
    }
}
