using System.Collections;
using System.IO;
using UnityEngine.Networking;

namespace MFramework
{
    /// <summary>
    /// AB����������������ĳ��.ab������
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

            string url = $"{MHotUpdateManager.url}/{serverInfo.ABName}";
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                MLog.Print($"{typeof(ABDownloader)}��<{url}>��ȡʧ��---{request.error}", MLogType.Warning);
                yield break;
            }
            else
            {
                string abPath = $"{MHotUpdateManager.Instance.ABLocalRootPath}/{serverInfo.ABName}";
                MPathUtility.CreateFolderIfNotExist(abPath, true);

                //�滻�ļ�
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
