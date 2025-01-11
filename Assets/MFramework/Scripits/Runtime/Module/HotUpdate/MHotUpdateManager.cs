using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace MFramework
{
    public class MHotUpdateManager : MonoSingleton<MHotUpdateManager>
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static string url = "http://127.0.0.1:5858";
#elif UNITY_ANDROID
        public static string url = null;
#elif UNITY_IPHONE
        public static string url = null;
#endif

        public Action OnUpdateEnd;

        public float downloadTotalSize;
        public float curDownloadSize;

        private string ABInfoFileName;
        internal string ABLocalRootPath;

        private Dictionary<string, ABInfo> localInfoDic = new Dictionary<string, ABInfo>();
        Queue<ABInfo> needUpdateInfoQueue = new Queue<ABInfo>();
        private List<ABDownloader> downloaderList = new List<ABDownloader>();
        private const int maxDownloaderCount = 5;

        internal void Initialize()
        {
            string platform = MABUtility.GetPlatform();
            if (MCore.Instance.ABEncryptState)
            {
                platform = $"{platform}_ENCRYPT";
            }
            string rootPath = $"{Application.productName}_AssetBundle/{platform}";

            //TODO����׿IOSӦ�ò�����
            ABLocalRootPath = Path.GetFullPath(rootPath).ReplaceSlash();
            ABInfoFileName = $"{rootPath}/{MSettings.ABInfoFileName}";
            MPathUtility.CreateFolderIfNotExist(ABLocalRootPath);
        }

        public void StartHotUpdate()
        {
            MLog.Print("��ʼ�ȸ�...");
            StartCoroutine(DownloadABInfo());
        }

        private IEnumerator DownloadABInfo()
        {
            string infoUrl = $"{url}/{ABInfoFileName}";
            Debug.Log(infoUrl);

            using (UnityWebRequest request = UnityWebRequest.Get(infoUrl))
            {
                yield return request.SendWebRequest();
                if(request.result != UnityWebRequest.Result.Success)
                {
                    MLog.Print($"{typeof(MHotUpdateManager)}����ȡAB��Ϣʧ��---{request.error}", MLogType.Warning);
                    yield break;
                }
                else
                {
                    string info = request.downloadHandler.text;
                    CheckNeedDownloadABPack(info);
                    DownloadABPack();
                }
            }
        }

        private void CheckNeedDownloadABPack(string info)
        {
            Dictionary<string, ABInfo> serverInfoDic = ConvertToABInfo(info);//�����ABInfo

            if (File.Exists(ABLocalRootPath))
            {
                string localInfo = File.ReadAllText(ABInfoFileName);//������Ϣ
                localInfoDic = ConvertToABInfo(localInfo); // �ͻ��˱��ػ������Դ�����б�

                //�����������ļ�
                foreach (ABInfo serverInfo in serverInfoDic.Values)
                {
                    if (localInfoDic.ContainsKey(serverInfo.ABName))//�Ѵ����ļ�
                    {
                        //�����������MD5��һ�£��������ı䣬��Ҫ����
                        if (localInfoDic[serverInfo.ABName].MD5 != serverInfo.MD5)
                        {
                            needUpdateInfoQueue.Enqueue(serverInfo);
                            downloadTotalSize += serverInfo.Size;
                        }
                    }
                    else//δ�����ļ�
                    {
                        //�����ڵ��ļ���Ҫ��ӽ���
                        needUpdateInfoQueue.Enqueue(serverInfo);
                        downloadTotalSize += serverInfo.Size;
                    }
                }
            }
            else//��������ڱ��أ�����Ҫ��������AB
            {
                foreach (ABInfo serverInfo in serverInfoDic.Values)
                {
                    needUpdateInfoQueue.Enqueue(serverInfo);
                    downloadTotalSize += serverInfo.Size;
                }
            }
        }

        private void DownloadABPack()
        {
            //list�����ݣ����������
            if (needUpdateInfoQueue.Count == 0)
            {
                OnUpdateEndInternal();
                return;
            }

            //����Downloader
            int downloaderCount = Mathf.Min(needUpdateInfoQueue.Count, maxDownloaderCount);
            for (int i = 0; i < downloaderCount; i++)
            {
                ABInfo info = needUpdateInfoQueue.Dequeue();
                ABDownloader downloader = new ABDownloader();
                downloaderList.Add(downloader);
                StartCoroutine(downloader.DownloadABPack(info));
            }
        }

        public Dictionary<string, ABInfo> ConvertToABInfo(string infos)
        {
            Dictionary<string, ABInfo> infoDic = new Dictionary<string, ABInfo>();

            string[] lines = infos.Split("\n");
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] datas = line.Split('|');
                ABInfo abInfo = new ABInfo();
                abInfo.ABName = datas[0];
                abInfo.MD5 = datas[1];
                abInfo.Size = int.Parse(datas[2]);

                infoDic.Add(abInfo.ABName, abInfo);//��������Ϊkey
            }

            return infoDic;
        }

        private void OnUpdateEndInternal()
        {
            //���´���ABInfo�ļ�
            StringBuilder sb = new StringBuilder();
            foreach (ABInfo info in localInfoDic.Values)
            {
                sb.AppendLine($"{info.ABName} {info.MD5} {info.Size}");
            }
            File.WriteAllText(ABInfoFileName, sb.ToString());
        }

        public void UpdateLocalABInfo(ABInfo serverinfo)
        {
            //ע�⣺�������˱����ֵ䣬������OnUpdateEnd()�и�����.txt��
            localInfoDic[serverinfo.ABName] = serverinfo;
        }

        public void DownloadNext(ABDownloader downloader)
        {
            curDownloadSize += downloader.GetSize();

            if (needUpdateInfoQueue.Count > 0)//�оͼ�����
            {
                ABInfo info = needUpdateInfoQueue.Dequeue();
                StartCoroutine(downloader.DownloadABPack(info));
            }
            else//û�������(������)
            {
                //�������ж��������
                bool isFinish = true;
                foreach (ABDownloader dl in downloaderList)
                {
                    if (dl.IsDownloading)
                    {
                        isFinish = false;
                        break;
                    }
                }

                if (isFinish)
                {
                    OnUpdateEndInternal();
                    OnUpdateEnd?.Invoke();

                    MLog.Print($"{typeof(MHotUpdateManager)}���ȸ������");
                }
            }
        }
    }
}
