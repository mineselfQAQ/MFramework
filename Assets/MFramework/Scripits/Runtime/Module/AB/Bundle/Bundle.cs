using System;
using System.IO;
using UnityEngine;

namespace MFramework
{
    public class Bundle : BundleBase
    {
        internal override void Load()
        {
            if (assetBundle)
            {
                MLog.Print($"{nameof(Bundle)}.{nameof(Load)}：AB已存在，请检查", MLogType.Error);
            }

            string file = MBundleManager.Instance.GetFileUrl(url);
#if UNITY_EDITOR || UNITY_STANDALONE
            if (!File.Exists(file))
            {
                MLog.Print($"{nameof(Bundle)}.{nameof(Load)}：{file}不存在，请检查", MLogType.Error);
            }
#endif

            //加载文件(由Unity提供)
            assetBundle = AssetBundle.LoadFromFile(file, 0, MBundleManager.Instance.offset);
            isStreamedSceneAssetBundle = assetBundle.isStreamedSceneAssetBundle;

            done = true;//说明该bundle已完成加载
        }

        /// <summary>
        /// 加载资源(同步)
        /// </summary>
        internal override UnityEngine.Object LoadAsset(string name, Type type)
        {
            if (string.IsNullOrEmpty(name))
            {
                MLog.Print($"{nameof(Bundle)}.{nameof(LoadAssetAsync)}：没有{name}资源，请检查", MLogType.Error);
            }
            if (assetBundle == null)
            {
                MLog.Print($"{nameof(Bundle)}.{nameof(LoadAssetAsync)}：Bunle为空，请检查", MLogType.Error);
            }

            return assetBundle.LoadAsset(name, type);
        }

        /// <summary>
        /// 加载资源(异步)
        /// </summary>
        internal override AssetBundleRequest LoadAssetAsync(string name, Type type)
        {
            if (string.IsNullOrEmpty(name))
            {
                MLog.Print($"{nameof(Bundle)}.{nameof(LoadAssetAsync)}：没有{name}资源，请检查", MLogType.Error);
            }
            if (assetBundle == null)
            {
                MLog.Print($"{nameof(Bundle)}.{nameof(LoadAssetAsync)}：Bunle为空，请检查", MLogType.Error);
            }

            return assetBundle.LoadAssetAsync(name, type);
        }

        internal override void UnLoad()
        {
            if (assetBundle)
            {
                assetBundle.Unload(true);//卸载核心
            }

            assetBundle = null;
            done = false;
            reference = 0;
            isStreamedSceneAssetBundle = false;
        }
    }
}