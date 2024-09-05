using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MFramework
{
    public class Resource : ResourceBase
    {
        public override bool keepWaiting => !done;

        internal override void Load()
        {
            if (string.IsNullOrEmpty(url))
            {
                MLog.Print($"{nameof(Resource)}.{nameof(Load)}：url为空，请检查", MLogType.Error);
            }
            if (bundle != null)
            {
                MLog.Print($"{nameof(Resource)}.{nameof(Load)}：Bundle已加载，请检查", MLogType.Error);
            }

            string bundleUrl = null;
            if (!MResourceManager.Instance.ResourceBunldeDic.TryGetValue(url, out bundleUrl))
            {
                MLog.Print($"{nameof(Resource)}.{nameof(Load)}：{bundleUrl}为空，请检查", MLogType.Error);
            }

            bundle = MBundleManager.Instance.Load(bundleUrl);//同步获取Bundle
            LoadAsset();
        }

        internal override void LoadAsset()
        {
            if (bundle == null)
            {
                MLog.Print($"{nameof(Resource)}.{nameof(LoadAsset)}：Bundle为空，请检查", MLogType.Error);
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
                MLog.Print($"{nameof(Resource)}.{nameof(UnLoad)}：Bundle为空，请检查", MLogType.Error);
            }
            if (asset != null && !(asset is GameObject))
            {
                Resources.UnloadAsset(asset);
                asset = null;
            }

            MBundleManager.Instance.UnLoad(bundle);

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