using System;
using MFramework.Core;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class MBundleManager
    {
        private Func<string, string> _getFileCallback;
        private AssetBundleManifest _assetBundleManifest;
        private readonly ABRuntimeState _runtimeState;
        internal ulong Offset { get; private set; }

        // 当前存在的bundle
        private Dictionary<string, BundleBase> _bundleDic = new Dictionary<string, BundleBase>();
        // 异步列表（正在加载的bundle）
        private List<BundleBaseAsync> _asyncList = new List<BundleBaseAsync>();
        // 卸载列表（正在卸载的bundle）
        private LinkedList<BundleBase> _unloadList = new LinkedList<BundleBase>();

        public MBundleManager(ABRuntimeState runtimeState)
        {
            _runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
        }

        internal void Initialize(string platform, Func<string, string> getFileCallback, ulong offset)
        {
            this._getFileCallback = getFileCallback;
            this.Offset = offset;

            // 平台名文件（如WINDOWS（无后缀））
            string assetBundleManifestFile = getFileCallback.Invoke(platform);

            // 获取AssetBundleManifest
            AssetBundle manifestAssetBundle = AssetBundle.LoadFromFile(assetBundleManifestFile);
            if (manifestAssetBundle == null)
            {
                MLog.Default?.E($"AB Manifest加载失败：file={assetBundleManifestFile}");
            }
            UnityEngine.Object[] objs = manifestAssetBundle.LoadAllAssets();
            if (objs.Length == 0)
            {
                MLog.Default?.E($"AB Manifest加载失败：Manifest中无数据，file={assetBundleManifestFile}");
            }
            _assetBundleManifest = objs[0] as AssetBundleManifest;
        }

        internal void Update()
        {
            for (int i = 0; i < _asyncList.Count; i++)
            {
                if (_asyncList[i].Update()) // 等待异步资源加载完成
                {
                    _asyncList.RemoveAt(i);
                    i--;
                }
            }
        }

        internal void LateUpdate()
        {
            // 存在需要释放的资源
            while (_unloadList.Count > 0)
            {
                BundleBase bundle = _unloadList.First.Value;
                _unloadList.RemoveFirst();
                if (bundle == null) continue;

                _bundleDic.Remove(bundle.url);

                // 还没创建完就卸载
                if (!bundle.done && bundle is BundleAsync)
                {
                    BundleAsync bundleAsync = bundle as BundleAsync;
                    if (_asyncList.Contains(bundleAsync))
                        _asyncList.Remove(bundleAsync);
                }
                // 一般卸载
                bundle.UnLoad();

                // 处理依赖
                if (bundle.dependencies != null)
                {
                    for (int i = 0; i < bundle.dependencies.Length; i++)
                    {
                        BundleBase temp = bundle.dependencies[i];
                        UnLoad(temp); // 卸载（reference--，如果没有引用就加入unloadList准备卸载）
                    }
                }
            }
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        internal BundleBase Load(string url)
        {
            return LoadInternal(url, false);
        }
        /// <summary>
        /// 异步加载
        /// </summary>
        internal BundleBase LoadAsync(string url)
        {
            return LoadInternal(url, true);
        }

        private BundleBase LoadInternal(string url, bool async)
        {
            // 尝试获取Bundle
            BundleBase bundle = null;
            if (_bundleDic.TryGetValue(url, out bundle))
            {
                // 已经准备卸载，但是现在又需要使用，直接取回
                if (bundle.reference == 0)
                {
                    _unloadList.Remove(bundle);
                }
                bundle.AddReference();

                return bundle;
            }

            // 创建Bundle
            if (async)
            {
                bundle = new BundleAsync(this, _runtimeState);
                bundle.url = url;
                _asyncList.Add(bundle as BundleBaseAsync);
            }
            else
            {
                bundle = new Bundle(this, _runtimeState);
                bundle.url = url;
            }
            _bundleDic.Add(url, bundle);

            // 加载依赖
            string[] dependencies = _assetBundleManifest.GetDirectDependencies(url);
            if (dependencies.Length > 0) // 如果该url有依赖就进行添加
            {
                bundle.dependencies = new BundleBase[dependencies.Length];
                for (int i = 0; i < dependencies.Length; i++)
                {
                    string dependencyUrl = dependencies[i];
                    BundleBase dependencyBundle = LoadInternal(dependencyUrl, async);
                    bundle.dependencies[i] = dependencyBundle;
                }
            }

            bundle.AddReference();
            bundle.Load(); // **实际bundle的Load（）**

            return bundle;
        }

        internal void UnLoad(BundleBase bundle)
        {
            if (bundle == null)
            {
                MLog.Default?.E("AB卸载失败：Bundle为空");
            }

            // 该Bundle的reference--，如果为0（没有引用），即可准备卸载
            bundle.ReduceReference();
            if (bundle.reference == 0)
            {
                WillUnload(bundle);
            }
        }

        private void WillUnload(BundleBase bundle)
        {
            _unloadList.AddLast(bundle);
        }

        internal string GetFileUrl(string url)
        {
            if (_getFileCallback == null)
            {
                MLog.Default?.E($"AB路径获取失败：路径回调为空，url={url}");
            }

            // 交到外部处理
            return _getFileCallback.Invoke(url);
        }

        internal void Shutdown()
        {
            _assetBundleManifest = null;
            _bundleDic.Clear();
            _asyncList.Clear();
            _unloadList.Clear();
            _getFileCallback = null;

        }
    }
}
