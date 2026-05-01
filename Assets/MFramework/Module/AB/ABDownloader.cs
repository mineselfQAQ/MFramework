using System.Collections;
using MFramework.Core;
using System.IO;
using UnityEngine.Networking;

using MFramework.Util;
namespace MFramework
{
    /// <summary>
    /// AB包下载器，可用于某个.ab的下载
    /// </summary>
    public class ABDownloader
    {
        private readonly MHotUpdateManager _hotUpdateManager;
        private ABInfo info;

        private bool isDownloading = false;
        public bool IsDownloading => isDownloading;

        public float GetSize() => info == null ? 0 : info.Size;

        public ABDownloader(MHotUpdateManager hotUpdateManager)
        {
            _hotUpdateManager = hotUpdateManager;
        }

        public IEnumerator DownloadABPack(ABInfo serverInfo)
        {
            isDownloading = true;
            info = serverInfo;

            string url = $"{MHotUpdateManager.url}/{serverInfo.ABName}";
            url = MPathUtility.HTTPConvert(url);
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                MLog.Default?.W($"AB热更新下载失败：url={url}, error={request.error}");
                yield break;
            }
            else
            {
                // Tip：由于ABLocalRootPath添加了xxx_AssetBundle/平台名，而Info中同样有，需要去除一个
                string abPath = $"{_hotUpdateManager.ABLocalRootPath.CD(2)}/{serverInfo.ABName}";
                MPathUtility.CreateFolderIfNotExist(abPath, true);

                // 替换文件
                using (FileStream fileStream = new FileStream(abPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] data = request.downloadHandler.data;
                    fileStream.Write(data, 0, data.Length);
                }

                _hotUpdateManager.UpdateLocalABInfo(serverInfo);
                isDownloading = false;
                _hotUpdateManager.DownloadNext(this);
            }
        }
    }
}
