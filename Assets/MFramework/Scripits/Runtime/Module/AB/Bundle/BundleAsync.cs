using System;
using System.IO;
using UnityEngine;

namespace MFramework
{
    public class BundleAsync : BundleBaseAsync
    {
        private AssetBundleCreateRequest assetBundleCreateRequest;

        internal override bool Update()
        {
            if (done) return true;

            //自身完成前依赖必须先完成
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

            //等待创建完成
            if (!assetBundleCreateRequest.isDone) return false; 
            //完成
            done = true;
            assetBundle = assetBundleCreateRequest.assetBundle;//取出assetBundle
            isStreamedSceneAssetBundle = assetBundle.isStreamedSceneAssetBundle;//取出isStreamedSceneAssetBundle

            //虽然创建完成，但是没人要用，卸载
            if (reference == 0)
            {
                UnLoad();
            }

            return true;
        }

        internal override void Load()
        {
            if (assetBundleCreateRequest != null)
            {
                MLog.Print($"{nameof(BundleAsync)}.{nameof(Load)}：AB创建请求已存在，请检查", MLogType.Error);
            }

            string file = MBundleManager.Instance.GetFileUrl(url);//获取文件路径
#if UNITY_EDITOR || UNITY_STANDALONE
            if (!File.Exists(file))
            {
                MLog.Print($"{nameof(BundleAsync)}.{nameof(Load)}：{file}不存在，请检查", MLogType.Error);
            }
#endif

            //核心---加载文件(由Unity提供)
            assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(file, 0, MBundleManager.Instance.offset);
        }

        /// <summary>
        /// 加载资源(同步)
        /// </summary>
        internal override UnityEngine.Object LoadAsset(string name, Type type)
        {
            if (string.IsNullOrEmpty(name))
            {
                MLog.Print($"{nameof(BundleAsync)}.{nameof(LoadAsset)}：没有{name}资源，请检查", MLogType.Error);
            }
            if (assetBundleCreateRequest == null)
            {
                MLog.Print($"{nameof(BundleAsync)}.{nameof(LoadAsset)}：AB创建请求为空，请检查", MLogType.Error);
            }

            if (assetBundle == null)
            {
                assetBundle = assetBundleCreateRequest.assetBundle;
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
                MLog.Print($"{nameof(BundleAsync)}.{nameof(LoadAssetAsync)}：没有{name}资源，请检查", MLogType.Error);
            }
            if (assetBundleCreateRequest == null)
            {
                MLog.Print($"{nameof(BundleAsync)}.{nameof(LoadAssetAsync)}：AB创建请求为空，请检查", MLogType.Error);
            }

            if (assetBundle == null)
            {
                assetBundle = assetBundleCreateRequest.assetBundle;
            }

            return assetBundle.LoadAssetAsync(name, type);
        }

        internal override void UnLoad()
        {
            if (assetBundle)//正常卸载
            {
                assetBundle.Unload(true);
            }
            else//还没加载完就卸载
            {
                //正在异步加载的资源也要切到主线程进行释放
                if (assetBundleCreateRequest != null)
                {
                    assetBundle = assetBundleCreateRequest.assetBundle;
                }
                if (assetBundle)
                {
                    assetBundle.Unload(true);
                }
            }

            assetBundleCreateRequest = null;
            done = false;
            reference = 0;
            assetBundle = null;
            isStreamedSceneAssetBundle = false;
        }
    }
}