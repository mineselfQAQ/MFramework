using System;
using UnityEngine;

namespace MFramework
{
    public abstract class BundleBase
    {
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
                MLog.Print($"{GetType()}.{nameof(ReduceReference)}：{url}的reference小于0，请检查", MLogType.Error);
            }
        }
    }
}