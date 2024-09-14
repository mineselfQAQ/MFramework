using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class MBundleManager : Singleton<MBundleManager>
    {
        private Func<string, string> getFileCallback;
        private AssetBundleManifest assetBundleManifest;
        internal ulong offset { get; private set; }

        //当前存在的bundle
        private Dictionary<string, BundleBase> bundleDic = new Dictionary<string, BundleBase>();
        //异步列表(正在加载的bundle)
        private List<BundleBaseAsync> asyncList = new List<BundleBaseAsync>();
        //卸载列表(正在卸载的bundle)
        private LinkedList<BundleBase> unloadList = new LinkedList<BundleBase>();

        private MBundleManager() { }

        internal void Initialize(string platform, Func<string, string> getFileCallback, ulong offset)
        {
            this.getFileCallback = getFileCallback;
            this.offset = offset;

            //平台名文件(如WINDOWS(无后缀))
            string assetBundleManifestFile = getFileCallback.Invoke(platform);

            //获取AssetBundleManifest
            AssetBundle manifestAssetBundle = AssetBundle.LoadFromFile(assetBundleManifestFile);
            if (manifestAssetBundle == null)
            {
                MLog.Print($"{nameof(MBundleManager)}.{nameof(Initialize)}：AssetBundleManifest加载失败，请检查getFileCallback()获取路径是否正确", MLogType.Error);
            }
            UnityEngine.Object[] objs = manifestAssetBundle.LoadAllAssets();
            if (objs.Length == 0)
            {
                MLog.Print($"{nameof(MBundleManager)}.{nameof(Initialize)}：AssetBundleManifest中无数据，请检查", MLogType.Error);
            }
            assetBundleManifest = objs[0] as AssetBundleManifest;
        }

        public void Update()
        {
            for (int i = 0; i < asyncList.Count; i++)
            {
                if (asyncList[i].Update())//等待异步资源加载完成
                {
                    asyncList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void LateUpdate()
        {
            //存在需要释放的资源
            while (unloadList.Count > 0)//逐个操作
            {
                BundleBase bundle = unloadList.First.Value;
                unloadList.RemoveFirst();
                if (bundle == null) continue;

                bundleDic.Remove(bundle.url);

                //还没创建完就卸载
                if (!bundle.done && bundle is BundleAsync)
                {
                    BundleAsync bundleAsync = bundle as BundleAsync;
                    if (asyncList.Contains(bundleAsync))
                        asyncList.Remove(bundleAsync);
                }
                //一般卸载
                bundle.UnLoad();

                //处理依赖
                if (bundle.dependencies != null)
                {
                    for (int i = 0; i < bundle.dependencies.Length; i++)
                    {
                        BundleBase temp = bundle.dependencies[i];
                        UnLoad(temp);//卸载(reference--，如果没有引用就加入unloadList准备卸载)
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
            //尝试获取Bundle
            BundleBase bundle = null;
            if (bundleDic.TryGetValue(url, out bundle))
            {
                //已经准备卸载，但是现在又需要使用，直接取回
                if (bundle.reference == 0)
                {
                    unloadList.Remove(bundle);
                }
                bundle.AddReference();

                return bundle;
            }

            //创建Bundle
            if (async)
            {
                bundle = new BundleAsync();
                bundle.url = url;
                asyncList.Add(bundle as BundleBaseAsync);
            }
            else
            {
                bundle = new Bundle();
                bundle.url = url;
            }
            bundleDic.Add(url, bundle);

            //加载依赖
            string[] dependencies = assetBundleManifest.GetDirectDependencies(url);
            if (dependencies.Length > 0)//如果该url有依赖就进行添加
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
            bundle.Load();//**实际bundle的Load()**

            return bundle;
        }

        internal void UnLoad(BundleBase bundle)
        {
            if (bundle == null)
            {
                MLog.Print($"{nameof(MBundleManager)}.{nameof(UnLoad)}：Bundle不存在，请检查", MLogType.Error);
            }

            //该Bundle的reference--，如果为0(没有引用)，即可准备卸载
            bundle.ReduceReference();
            if (bundle.reference == 0)
            {
                WillUnload(bundle);
            }
        }

        private void WillUnload(BundleBase bundle)
        {
            unloadList.AddLast(bundle);
        }

        internal string GetFileUrl(string url)
        {
            if (getFileCallback == null)
            {
                MLog.Print($"{nameof(MBundleManager)}.{nameof(GetFileUrl)}：获取路径回调为空，请检查", MLogType.Error);
            }

            //交到外部处理
            return getFileCallback.Invoke(url);
        }
    }
}
