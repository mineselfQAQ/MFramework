using System;
using MFramework.Core;
using System.IO;
using UnityEngine;

namespace MFramework
{
    public class Bundle : BundleBase
    {
        public Bundle(MBundleManager bundleManager, ABRuntimeState runtimeState)
            : base(bundleManager, runtimeState)
        {
        }

        internal override void Load()
        {
            if (assetBundle)
            {
                MLog.Default?.E($"AB同步加载失败：Bundle已加载，url={url}");
            }

            string file = BundleManager.GetFileUrl(url);
#if UNITY_EDITOR || UNITY_STANDALONE
            if (!File.Exists(file))
            {
                MLog.Default?.E($"AB同步加载失败：文件不存在，url={url}, file={file}");
            }
#endif

            // AB解密文件
            assetBundle = MABUtility.LoadAB(file, RuntimeState, BundleManager.Offset);
            isStreamedSceneAssetBundle = assetBundle.isStreamedSceneAssetBundle;

            done = true; // 说明该bundle已完成加载
        }

        /// <summary>
        /// 加载资源（同步）
        /// </summary>
        internal override UnityEngine.Object LoadAsset(string name, Type type)
        {
            if (string.IsNullOrEmpty(name))
            {
                MLog.Default?.E($"AB资源同步加载失败：资源名为空，bundle={url}");
            }
            if (assetBundle == null)
            {
                MLog.Default?.E($"AB资源同步加载失败：Bundle未加载，bundle={url}, asset={name}");
            }

            return assetBundle.LoadAsset(name, type);
        }

        /// <summary>
        /// 加载资源（异步）
        /// </summary>
        internal override AssetBundleRequest LoadAssetAsync(string name, Type type)
        {
            if (string.IsNullOrEmpty(name))
            {
                MLog.Default?.E($"AB资源异步加载失败：资源名为空，bundle={url}");
            }
            if (assetBundle == null)
            {
                MLog.Default?.E($"AB资源异步加载失败：Bundle未加载，bundle={url}, asset={name}");
            }

            return assetBundle.LoadAssetAsync(name, type);
        }

        internal override void UnLoad()
        {
            if (assetBundle)
            {
                assetBundle.Unload(true); // 卸载核心
            }

            assetBundle = null;
            done = false;
            reference = 0;
            isStreamedSceneAssetBundle = false;
        }
    }
}
