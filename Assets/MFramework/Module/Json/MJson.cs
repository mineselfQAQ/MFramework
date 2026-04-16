using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using MFramework.Core;

using UnityEngine;
using UnityEngine.Networking;

namespace MFramework.Json
{
    /// <summary>
    /// Json 读取与解析模块（基于 Unity JsonUtility）
    /// </summary>
    public static class MJson
    {
        private static readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
        private static ILog _log = new UserLog(nameof(MJson));

        /// <summary>
        /// 可替换默认日志实例
        /// </summary>
        public static void SetLogger(ILog log)
        {
            if (log != null) _log = log;
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public static void ClearCache() => _cache.Clear();

        /// <summary>
        /// 移除指定 Key 的缓存
        /// </summary>
        public static bool RemoveCache(string key) => _cache.Remove(key);

        /// <summary>
        /// 从 Resources 读取并解析
        /// </summary>
        public static T LoadFromResources<T>(string path, bool cache = true)
        {
            string cacheKey = $"res::{path}::{typeof(T).FullName}";
            if (cache && TryGetCache(cacheKey, out T cached)) return cached;

            TextAsset asset = Resources.Load<TextAsset>(path);
            if (asset == null)
            {
                throw new UnityFrameworkException($"Resources 未找到: {path}");
            }

            T data = ParseJson<T>(asset.text);
            if (cache) _cache[cacheKey] = data;
            return data;
        }

        /// <summary>
        /// 从 Resources 读取数组（JsonUtility 不支持顶层数组）
        /// </summary>
        public static T[] LoadArrayFromResources<T>(string path, bool cache = true)
        {
            string cacheKey = $"res-array::{path}::{typeof(T).FullName}";
            if (cache && TryGetCache(cacheKey, out T[] cached)) return cached;

            TextAsset asset = Resources.Load<TextAsset>(path);
            if (asset == null)
            {
                throw new UnityFrameworkException($"Resources 未找到: {path}");
            }

            T[] data = ParseJsonArray<T>(asset.text);
            if (cache) _cache[cacheKey] = data;
            return data;
        }

        /// <summary>
        /// 从本地文件路径读取并解析
        /// </summary>
        public static T LoadFromFile<T>(string absolutePath, bool cache = true)
        {
            string cacheKey = $"file::{absolutePath}::{typeof(T).FullName}";
            if (cache && TryGetCache(cacheKey, out T cached)) return cached;

            string json = ReadAllTextUtf8(absolutePath);
            T data = ParseJson<T>(json);
            if (cache) _cache[cacheKey] = data;
            return data;
        }

        /// <summary>
        /// 异步读取本地文件并解析
        /// </summary>
        public static async Task<T> LoadFromFileAsync<T>(string absolutePath, bool cache = true)
        {
            string cacheKey = $"file::{absolutePath}::{typeof(T).FullName}";
            if (cache && TryGetCache(cacheKey, out T cached)) return cached;

            string json = await ReadAllTextUtf8Async(absolutePath).ConfigureAwait(false);
            T data = ParseJson<T>(json);
            if (cache) _cache[cacheKey] = data;
            return data;
        }

        /// <summary>
        /// 从 StreamingAssets 读取（PC/Editor 可同步，Android/WebGL 请用异步）
        /// </summary>
        public static T LoadFromStreamingAssets<T>(string relativePath, bool cache = true)
        {
            if (IsStreamingAssetsAsyncOnly())
            {
                throw new UnityFrameworkException("当前平台 StreamingAssets 仅支持异步读取，请使用 LoadFromStreamingAssetsAsync。");
            }

            string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
            return LoadFromFile<T>(fullPath, cache);
        }

        /// <summary>
        /// 异步从 StreamingAssets 读取
        /// </summary>
        public static async Task<T> LoadFromStreamingAssetsAsync<T>(string relativePath, bool cache = true)
        {
            string cacheKey = $"sa::{relativePath}::{typeof(T).FullName}";
            if (cache && TryGetCache(cacheKey, out T cached)) return cached;

            string uri = BuildStreamingAssetsUri(relativePath);
            string json = await GetTextByRequest(uri).ConfigureAwait(false);
            T data = ParseJson<T>(json);
            if (cache) _cache[cacheKey] = data;
            return data;
        }

        /// <summary>
        /// 直接解析 Json 文本
        /// </summary>
        public static T LoadFromText<T>(string json, bool cache = false, string cacheKey = null)
        {
            if (cache && !string.IsNullOrEmpty(cacheKey) && TryGetCache(cacheKey, out T cached)) return cached;

            T data = ParseJson<T>(json);
            if (cache && !string.IsNullOrEmpty(cacheKey)) _cache[cacheKey] = data;
            return data;
        }

        /// <summary>
        /// 直接解析 Json 数组文本
        /// </summary>
        public static T[] LoadArrayFromText<T>(string json, bool cache = false, string cacheKey = null)
        {
            if (cache && !string.IsNullOrEmpty(cacheKey) && TryGetCache(cacheKey, out T[] cached)) return cached;

            T[] data = ParseJsonArray<T>(json);
            if (cache && !string.IsNullOrEmpty(cacheKey)) _cache[cacheKey] = data;
            return data;
        }

        /// <summary>
        /// 安全读取 Resources
        /// </summary>
        public static bool TryLoadFromResources<T>(string path, out T data, bool cache = true)
        {
            try
            {
                data = LoadFromResources<T>(path, cache);
                return true;
            }
            catch (Exception ex)
            {
                _log.E(ex);
                data = default;
                return false;
            }
        }

        /// <summary>
        /// 安全读取本地文件
        /// </summary>
        public static bool TryLoadFromFile<T>(string absolutePath, out T data, bool cache = true)
        {
            try
            {
                data = LoadFromFile<T>(absolutePath, cache);
                return true;
            }
            catch (Exception ex)
            {
                _log.E(ex);
                data = default;
                return false;
            }
        }

        private static bool TryGetCache<T>(string key, out T data)
        {
            if (_cache.TryGetValue(key, out object obj) && obj is T typed)
            {
                data = typed;
                return true;
            }

            data = default;
            return false;
        }

        private static T ParseJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new UnityFrameworkException("Json 内容为空。");
            }

            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                throw new UnityFrameworkException($"Json 解析失败: {ex.Message}");
            }
        }

        private static T[] ParseJsonArray<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new UnityFrameworkException("Json 数组内容为空。");
            }

            try
            {
                string wrapped = "{\"items\":" + json + "}";
                JsonArrayWrapper<T> wrapper = JsonUtility.FromJson<JsonArrayWrapper<T>>(wrapped);
                return wrapper?.Items ?? Array.Empty<T>();
            }
            catch (Exception ex)
            {
                throw new UnityFrameworkException($"Json 数组解析失败: {ex.Message}");
            }
        }

        private static string ReadAllTextUtf8(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                throw new UnityFrameworkException($"文件不存在: {absolutePath}");
            }

            return File.ReadAllText(absolutePath, Encoding.UTF8);
        }

        private static async Task<string> ReadAllTextUtf8Async(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                throw new UnityFrameworkException($"文件不存在: {absolutePath}");
            }

            using (var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        private static bool IsStreamingAssetsAsyncOnly()
        {
#if UNITY_ANDROID || UNITY_WEBGL
            return true;
#else
            return false;
#endif
        }

        private static string BuildStreamingAssetsUri(string relativePath)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
            fullPath = fullPath.Replace("\\", "/");

            if (fullPath.Contains("://")) return fullPath;
            return "file://" + fullPath;
        }

        private static async Task<string> GetTextByRequest(string uri)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(uri))
            {
                var op = req.SendWebRequest();
                while (!op.isDone)
                {
                    await Task.Yield();
                }

#if UNITY_2020_2_OR_NEWER
                if (req.result != UnityWebRequest.Result.Success)
#else
                if (req.isNetworkError || req.isHttpError)
#endif
                {
                    throw new UnityFrameworkException($"读取失败: {req.error} ({uri})");
                }

                return req.downloadHandler.text;
            }
        }

        [Serializable]
        private class JsonArrayWrapper<T>
        {
            public T[] Items;
        }
    }
}
