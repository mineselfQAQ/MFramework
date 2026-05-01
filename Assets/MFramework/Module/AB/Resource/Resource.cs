using System;
using MFramework.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MFramework
{
    public class Resource : ResourceBase
    {
        public Resource(MResourceManager resourceManager, MBundleManager bundleManager)
            : base(resourceManager, bundleManager)
        {
        }

        public override bool keepWaiting => !done;

        internal override void Load()
        {
            if (string.IsNullOrEmpty(url))
            {
                MLog.Default?.E("AB error.");
            }
            if (bundle != null)
            {
                MLog.Default?.E("AB error.");
            }

            string bundleUrl = null;
            if (!ResourceManager.ResourceBunldeDic.TryGetValue(url, out bundleUrl))
            {
                MLog.Default?.E("AB error.");
            }

            bundle = BundleManager.Load(bundleUrl);//同步获取Bundle
            LoadAsset();
        }

        internal override void LoadAsset()
        {
            if (bundle == null)
            {
                MLog.Default?.E("AB error.");
            }

            //正在异步加载的资源要变成同步
            FreshAsyncAsset();

            if (bundle.isStreamedSceneAssetBundle) return;//对于场景不需要LoadAsset()
            asset = bundle.LoadAsset(url, typeof(Object));

            done = true;

            if (finishedCallback != null)
            {
                Action<ResourceBase> tempCallback = finishedCallback;
                finishedCallback = null;
                tempCallback.Invoke(this);
            }
        }
        
        internal override void UnLoad()
        {
            if (bundle == null)
            {
                MLog.Default?.E("AB error.");
            }
            if (asset != null && !(asset is GameObject))
            {
                Resources.UnloadAsset(asset);
                asset = null;
            }

            BundleManager.UnLoad(bundle);

            bundle = null;
            awaiter = null;
            finishedCallback = null;
        }

        public override T GetAsset<T>()
        {
            Object tempAsset = asset;
            Type type = typeof(T);

            if (type == typeof(Sprite))//获取Sprite资源的处理
            {
                if (asset is Sprite)//如果资源是Sprite那么直接取出即可
                {
                    return tempAsset as T;
                }
                else//如果资源不是Sprite就需要重新加载
                {
                    if (tempAsset && !(tempAsset is GameObject))
                    {
                        Resources.UnloadAsset(tempAsset);
                    }

                    asset = bundle.LoadAsset(url, type);
                    return asset as T;
                }
            }
            else//非Sprite直接取出
            {
                return tempAsset as T;
            }
        }
    }
}