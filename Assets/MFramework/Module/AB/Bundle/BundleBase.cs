using System;
using MFramework.Core;
using UnityEngine;

namespace MFramework
{
    public abstract class BundleBase
    {
        protected readonly MBundleManager BundleManager;
        protected readonly ABRuntimeState RuntimeState;

        protected BundleBase(MBundleManager bundleManager, ABRuntimeState runtimeState)
        {
            BundleManager = bundleManager ?? throw new ArgumentNullException(nameof(bundleManager));
            RuntimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
        }

        internal abstract void Load();
        internal abstract void UnLoad();
        internal abstract UnityEngine.Object LoadAsset(string name, Type type);
        internal abstract AssetBundleRequest LoadAssetAsync(string name, Type type);

        internal AssetBundle assetBundle { get; set; }

        internal bool isStreamedSceneAssetBundle { get; set; }

        internal string url { get; set; }

        internal int reference { get; set; }

        internal bool done { get; set; }

        internal BundleBase[] dependencies { get; set; }

        internal void AddReference()
        {
            ++reference;
        }
        internal void ReduceReference()
        {
            --reference;

            if (reference < 0)
            {
                MLog.Default?.E($"AB引用计数异常：Bundle引用计数小于0，url={url}, reference={reference}");
            }
        }
    }
}
