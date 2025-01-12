using System.Collections;
using System.IO;
using UnityEngine.Networking;

namespace MFramework
{
    /// <summary>
    /// AB包下载器，可用于某个.ab的下载
    /// </summary>
    public class ABDownloader
    {
        private ABInfo info;

        private bool isDownloading = false;
        public bool IsDownloading => isDownloading;

        public float GetSize() => info == null ? 0 : info.Size;

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
                MLog.Print($"{typeof(ABDownloader)}：<{url}>获取失败---{request.error}", MLogType.Warning);
                yield break;
            }
            else
            {
                //Tip：由于ABLocalRootPath添加了xxx_AssetBundle/平台名，而Info中同样有，需要去除一个
                string abPath = $"{MHotUpdateManager.Instance.ABLocalRootPath.CD(2)}/{serverInfo.ABName}";
                MPathUtility.CreateFolderIfNotExist(abPath, true);

                //替换文件
                using (FileStream fileStream = new FileStream(abPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fileStream.Write(request.downloadHandler.data);
                }

                MHotUpdateManager.Instance.UpdateLocalABInfo(serverInfo);
                isDownloading = false;
                MHotUpdateManager.Instance.DownloadNext(this);
            }
        }
    }
}
