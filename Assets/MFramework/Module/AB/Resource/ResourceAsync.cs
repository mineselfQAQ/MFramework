using System;
using MFramework.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MFramework
{
    public class ResourceAsync : ResourceBaseAsync
    {
        public ResourceAsync(MResourceManager resourceManager, MBundleManager bundleManager)
            : base(resourceManager, bundleManager)
        {
        }

        public override bool keepWaiting => !done;

        private AssetBundleRequest assetBundleRequest;

        public override Object asset
        {
            get
            {
                if (done) return base.asset;

                // 正在异步加载的资源要变成同步
                FreshAsyncAsset();

                return base.asset;
            }

            protected set
            {
                base.asset = value;
            }
        }

        public override bool Update()
        {
            if (done) return true;

            // 自身完成前依赖必须先完成
            if (dependencies != null)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    if (!dependencies[i].done)
                    {
                        return false;
                    }
                }
            }

            // 先要等待Bundle完成
            if (!bundle.done) return false;

            // 对于场景情况，直接结算即可
            if (bundle.assetBundle.isStreamedSceneAssetBundle)
            {
                LoadAsset();
                return true;
            }

            if (assetBundleRequest == null)
            {
                LoadAssetAsync(); // 拿到assetBundleRequest（异步获取资源）
            }
            // 等待加载完成
            if (assetBundleRequest != null && !assetBundleRequest.isDone) return false;

            // 加载完成后最终加载资源
            LoadAsset(); // 拿到asset

            return true;
        }

        internal override void Load()
        {
            if (string.IsNullOrEmpty(url))
            {
                MLog.Default?.E("AB资源异步加载失败：资源路径为空");
            }
            if (bundle != null)
            {
                MLog.Default?.E($"AB资源异步加载失败：资源已绑定Bundle，asset={url}, bundle={bundle.url}");
            }

            string bundleUrl = null;
            if (!ResourceManager.ResourceBunldeDic.TryGetValue(url, out bundleUrl))
            {
                MLog.Default?.E($"AB资源异步加载失败：未找到资源对应的Bundle，asset={url}");
            }
            bundle = BundleManager.LoadAsync(bundleUrl); // 异步加载Bundle
        }

        internal override void LoadAsset()
        {
            if (bundle == null)
            {
                MLog.Default?.E($"AB资源加载失败：Bundle为空，asset={url}");
            }

            if (!bundle.isStreamedSceneAssetBundle)
            {
                if (assetBundleRequest != null) // 异步加载
                {
                    asset = assetBundleRequest.asset;
                }
                else // 同步加载
                {
                    asset = bundle.LoadAsset(url, typeof(UnityEngine.Object));
                }
            }

            done = true;

            if (finishedCallback != null)
            {
                Action<ResourceBase> tempCallback = finishedCallback;
                finishedCallback = null;
                tempCallback.Invoke(this);
            }
        }

        internal override void LoadAssetAsync()
        {
            if (bundle == null)
            {
                MLog.Default?.E($"AB资源异步请求创建失败：Bundle为空，asset={url}");
            }

            assetBundleRequest = bundle.LoadAssetAsync(url, typeof(UnityEngine.Object));
        }

        internal override void UnLoad()
        {
            if (bundle == null)
            {
                MLog.Default?.E($"AB资源卸载失败：Bundle为空，asset={url}");

            }
            if (base.asset != null && !(base.asset is GameObject))
            {
                Resources.UnloadAsset(base.asset);
                asset = null;
            }

            assetBundleRequest = null;
            BundleManager.UnLoad(bundle);
            bundle = null;
            awaiter = null;
            finishedCallback = null;
        }

        public override T GetAsset<T>()
        {
            Object tempAsset = asset;
            Type type = typeof(T);
            if (type == typeof(Sprite))
            {
                if (asset is Sprite)
                {
                    return tempAsset as T;
                }
                else
                {
                    if (tempAsset && !(tempAsset is GameObject))
                    {
                        Resources.UnloadAsset(tempAsset);
                    }

                    asset = bundle.LoadAsset(url, type);
                    return asset as T;
                }
            }
            else
            {
                return tempAsset as T;
            }
        }
    }
}
