using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace MFramework
{
    /// <summary>
    /// AB������������
    /// </summary>
    public class Builder
    {
        private static readonly Profiler ms_BuildProfiler = new Profiler(nameof(Builder));
        private static readonly Profiler ms_LoadBuildSettingProfiler = ms_BuildProfiler.CreateChild(nameof(LoadSetting));
        private static readonly Profiler ms_CollectProfiler = ms_BuildProfiler.CreateChild(nameof(Collect));
        private static readonly Profiler ms_CollectBuildSettingFileProfiler = ms_CollectProfiler.CreateChild(nameof(MFramework.BuildSetting.FileCollect));
        private static readonly Profiler ms_CollectDependencyProfiler = ms_CollectProfiler.CreateChild(nameof(CollectDependency));
        private static readonly Profiler ms_CollectBundleProfiler = ms_CollectProfiler.CreateChild(nameof(CollectBundle));
        private static readonly Profiler ms_GenerateManifestProfiler = ms_CollectProfiler.CreateChild(nameof(GenerateManifest));
        private static readonly Profiler ms_BuildBundleProfiler = ms_BuildProfiler.CreateChild(nameof(BuildBundle));
        private static readonly Profiler ms_ClearBundleProfiler = ms_BuildProfiler.CreateChild(nameof(ClearBundle));
        private static readonly Profiler ms_BuildManifestBundleProfiler = ms_BuildProfiler.CreateChild(nameof(BuildManifest));

        public static readonly Vector2[] ms_Progress = new Vector2[]
        {
            new Vector2(0.0f, 0.2f),//1---FileCollect
            new Vector2(0.2f, 0.4f),//2---CollectDependency
            new Vector2(0.4f, 0.5f),//3---CollectBundle
            new Vector2(0.5f, 0.6f),//4---GenerateManifest
            new Vector2(0.6f, 0.7f),//5---BuildBundle
            new Vector2(0.7f, 0.9f),//6---ClearBundle
            new Vector2(0.9f, 1.0f),//7---BuildManifest
        };

        //���ݵ�ǰƽ̨ѡ����ƽ̨����(����·��)
#if UNITY_IOS
        private const string PLATFORM = "IOS";
#elif UNITY_ANDROID
        private const string PLATFORM = "ANDROID";
#else
        private const string PLATFORM = "WINDOWS";
#endif
        public const string BUNDLE_SUFFIX = ".ab";
        public const string BUNDLE_MANIFEST_SUFFIX = ".manifest";
        public const string MANIFEST = "manifest";

        public static readonly string TempPath = $"{MSettings.TempAssetPath}/AB";
        public static readonly string ResourceTXTPath = $"{TempPath}/Resource.txt";
        public static readonly string ResourceBYTEPath = $"{TempPath}/Resource.bytes";
        public static readonly string BundleTXTPath = $"{TempPath}/Bundle.txt";
        public static readonly string BundleBYTEPath = $"{TempPath}/Bundle.bytes";
        public static readonly string DependencyTXTPath = $"{TempPath}/Dependency.txt";
        public static readonly string DependencyBYTEPath = $"{TempPath}/Dependency.bytes";

        /// <summary>
        /// �������
        /// </summary>
        public static readonly BuildAssetBundleOptions BuildAssetBundleOptions = 
            BuildAssetBundleOptions.ChunkBasedCompression |
            //BuildAssetBundleOptions.DeterministicAssetBundle | 
            BuildAssetBundleOptions.StrictMode | 
            BuildAssetBundleOptions.DisableLoadAssetByFileName |
            BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

        /// <summary>
        /// ��������
        /// </summary>
        public static readonly ParallelOptions ParallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2
        };

        /// <summary>
        /// ���·��
        /// </summary>
        public static readonly string BuildSettingPath = MSettings.ABBuildSettingName;

        /// <summary>
        /// ���������Ϣ
        /// </summary>
        public static BuildSetting BuildSetting { get; private set; }

        /// <summary>
        /// ���Ŀ¼
        /// </summary>
        public static string BuildPath { get; set; }

        internal static void BuildInternal()
        {
            ms_BuildProfiler.Start();

            //��������
            ms_LoadBuildSettingProfiler.Start();
            BuildSetting = LoadSetting(BuildSettingPath);
            ms_LoadBuildSettingProfiler.Stop();

            //�Ѽ�bundle��Ϣ
            ms_CollectProfiler.Start();
            Dictionary<string, List<string>> bundleDic = Collect();
            ms_CollectProfiler.Stop();

            //�����assetbundle
            ms_BuildBundleProfiler.Start();
            BuildBundle(bundleDic);
            ms_BuildBundleProfiler.Stop();

            //��ն����ļ�
            ms_ClearBundleProfiler.Start();
            ClearBundle(BuildPath, bundleDic);
            ms_ClearBundleProfiler.Stop();

            //�������ļ����assetbundle
            ms_BuildManifestBundleProfiler.Start();
            BuildManifest();
            ms_BuildManifestBundleProfiler.Stop();

            EditorUtility.ClearProgressBar();

            ms_BuildProfiler.Stop();

            EditorUtility.RevealInFinder(BuildPath);

            MLog.Print($"������{ms_BuildProfiler}");
        }

        /// <summary>
        /// ����1������BuildSetting
        /// </summary>
        private static BuildSetting LoadSetting(string settingPath)
        {
            //Tip��ĿǰReadFromXml()ֻ���ȡXmlSettings�ļ����µ��ļ������Ա������
            BuildSetting = MSerializationUtility.ReadFromXml<BuildSetting>(settingPath);
            if (BuildSetting == null)
            {
                MLog.Print($"{typeof(Builder)}��·��{settingPath}����ʧ�ܣ�����", MLogType.Warning);
                return null;
            }
            BuildSetting.Init();//buildSetting�ڲ���ʼ��(����Ϊ�ռ�itemDic��Ϣ)

            //��ȡ���Դ��·��
            BuildPath = BuildSetting.buildRoot;
            if (BuildPath.Length > 0 && BuildPath[BuildPath.Length - 1] != '/')
            {
                BuildPath += "/";
            }
            BuildPath += $"{PLATFORM}/";

            return BuildSetting;
        }

        /// <summary>
        /// ����2���ռ���Ϣ(����)
        /// </summary>
        private static Dictionary<string, List<string>> Collect()
        {
            //2.1���ռ������Դ·��
            //files---XML������������������ļ�
            ms_CollectBuildSettingFileProfiler.Start();
            HashSet<string> files = BuildSetting.FileCollect();
            ms_CollectBuildSettingFileProfiler.Stop();

            //2.2���Ѽ�files��������ϵ�ļ�
            //dependencyDic---files�������ļ��Լ������ļ��������ļ�
            ms_CollectDependencyProfiler.Start();
            Dictionary<string, List<string>> dependencyDic = CollectDependency(files);
            ms_CollectDependencyProfiler.Stop();

            //����ļ�����
            //assetDic---�����ļ�·�����ļ�����
            Dictionary<string, ResourceType> assetDic = new Dictionary<string, ResourceType>();
            //files�ض���Direct��
            foreach (string url in files)
            {
                assetDic.Add(url, ResourceType.Direct);
            }
            //dependencyDic�г���files���ļ�����Dependency��
            foreach (string url in dependencyDic.Keys)
            {
                if (!assetDic.ContainsKey(url))
                {
                    assetDic.Add(url, ResourceType.Dependency);
                }
            }

            //2.3����ȡAB����Ϣ
            //bundleDic---ÿ��bundle����Ӧ�������ļ�·��
            ms_CollectBundleProfiler.Start();
            Dictionary<string, List<string>> bundleDic = CollectBundle(BuildSetting, assetDic, dependencyDic);
            ms_CollectBundleProfiler.Stop();

            //2.4������Manifest�ļ�
            ms_GenerateManifestProfiler.Start();
            GenerateManifest(assetDic, bundleDic, dependencyDic);
            ms_GenerateManifestProfiler.Stop();

            return bundleDic;
        }

        /// <summary>
        /// ����2.2���Ѽ�files��������ϵ�ļ�
        /// </summary>
        private static Dictionary<string, List<string>> CollectDependency(ICollection<string> files)
        {
            float min = ms_Progress[1].x, max = ms_Progress[1].y;//[0.2,0.4]

            Dictionary<string, List<string>> dependencyDic = new Dictionary<string, List<string>>();
            List<string> fileList = new List<string>(files);

            //����ÿһ���ļ�·��
            int segmentCount = 10;//����
            int segmentSize = Mathf.Max(1, files.Count / segmentCount);//ÿ��ִ�и���
            for (int i = 0; i < fileList.Count; i++)
            {
                if (i % segmentSize == 0 || i == files.Count - 1)//���ﵽһ�λ����һ��Ԫ��ʱ���Ľ�����
                {
                    float progress = min + (max - min) * ((float)i / (files.Count - 1));
                    EditorUtility.DisplayProgressBar($"{nameof(CollectDependency)}", "�Ѽ��ļ�������ϵ", progress);
                }

                string assetUrl = fileList[i];
                if (dependencyDic.ContainsKey(assetUrl)) continue;//�ļ�·���Ѵ��ڣ������в���

                //ͨ��Unity��GetDependencies()��ȡ��������·��
                string[] dependencies = AssetDatabase.GetDependencies(assetUrl, false);//**����**
                List<string> dependencyList = new List<string>(dependencies.Length);

                //���˵�������Ҫ�������
                for (int j = 0; j < dependencies.Length; j++)
                {
                    string tempAssetUrl = dependencies[j];
                    string extension = Path.GetExtension(tempAssetUrl).ToLower();
                    //����Ҫcs�ļ���dll�ļ�
                    if (string.IsNullOrEmpty(extension) || extension == ".cs" || extension == ".dll") continue;
                    //������������Ҫ����ļ�����浽��ʽ��dependencyList�У��������⣬����ļ��б���û�и��ļ�����Ҫ���벢����Ѱ����������(����������)
                    dependencyList.Add(tempAssetUrl);
                    if (!fileList.Contains(tempAssetUrl)) fileList.Add(tempAssetUrl);
                }

                //Tip�����ԭ����files���࣬��Ϊ���������ļ�(files������/����������)
                dependencyDic.Add(assetUrl, dependencyList);
            }

            return dependencyDic;
        }

        /// <summary>
        /// ����2.3����ȡAB����Ϣ
        /// </summary>
        private static Dictionary<string, List<string>> CollectBundle(BuildSetting buildSetting, Dictionary<string, ResourceType> assetDic, Dictionary<string, List<string>> dependencyDic)
        {
            float min = ms_Progress[2].x, max = ms_Progress[2].y;//[0.4,0.5]

            EditorUtility.DisplayProgressBar($"{nameof(CollectBundle)}", "�Ѽ�bundle��Ϣ", min);

            Dictionary<string, List<string>> bundleDic = new Dictionary<string, List<string>>();
            List<string> notInRuleList = new List<string>();//�ⲿ��Դ

            int index = 0;
            foreach (KeyValuePair<string, ResourceType> pair in assetDic)
            {
                index++;
                string assetUrl = pair.Key;
                //�ؼ�---��ȡ��Դ����Bundle��
                string bundleName = buildSetting.GetBundleName(assetUrl, pair.Value);

                //û��bundleName����ԴΪ�ⲿ��Դ(XML��ָʾ�������Դ)
                if (bundleName == null)
                {
                    notInRuleList.Add(assetUrl);
                    continue;
                }

                List<string> list;
                //û��Key�ͼ���Key����Key��ȡ��Value
                if (!bundleDic.TryGetValue(bundleName, out list))
                {
                    list = new List<string>();
                    bundleDic.Add(bundleName, list);
                }
                list.Add(assetUrl);

                EditorUtility.DisplayProgressBar($"{nameof(CollectBundle)}", "�Ѽ�bundle��Ϣ", min + (max - min) * ((float)index / assetDic.Count));
            }

            //�ⲿ��Դ������
            if (notInRuleList.Count > 0)
            {
                string message = string.Empty;
                for (int i = 0; i < notInRuleList.Count; i++)
                {
                    message += "\n" + notInRuleList[i];
                }
                EditorUtility.ClearProgressBar();
                MLog.Print($"{typeof(Builder)}������������׺��ƥ�����Դ{message}", MLogType.Error);
            }

            //���ڲ���˳(Ҳ��������)
            foreach (List<string> list in bundleDic.Values)
            {
                list.Sort();
            }

            return bundleDic;
        }

        /// <summary>
        /// ����2.4������Manifest�ļ�
        /// </summary>
        private static void GenerateManifest(Dictionary<string, ResourceType> assetDic, Dictionary<string, List<string>> bundleDic, Dictionary<string, List<string>> dependencyDic)
        {
            float min = ms_Progress[3].x, max = ms_Progress[3].y;//[0.5,0.6]

            EditorUtility.DisplayProgressBar($"{nameof(GenerateManifest)}", "���ɴ����Ϣ", min);

            //������ʱ����ļ���Ŀ¼
            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }

            //Tip:
            //.txt�ļ����ڿ��ӻ��鿴
            //.bytes�ļ����ڴ���(�����ǹؼ�)

            //��Դӳ��id
            Dictionary<string, ushort> assetIdDic = new Dictionary<string, ushort>();

            #region ������Դ������Ϣ
            {
                //ɾ���ϴ������ļ�
                if (File.Exists(ResourceTXTPath)) File.Delete(ResourceTXTPath);
                if (File.Exists(ResourceBYTEPath)) File.Delete(ResourceBYTEPath);

                StringBuilder resourceSb = new StringBuilder();
                MemoryStream resourceMs = new MemoryStream();
                BinaryWriter resourceBw = new BinaryWriter(resourceMs);
                if (assetDic.Count > ushort.MaxValue)
                {
                    EditorUtility.ClearProgressBar();
                    MLog.Print($"{typeof(Builder)}.{nameof(GenerateManifest)}����Դ��������{ushort.MaxValue}", MLogType.Error);
                }
                
                resourceBw.Write((ushort)assetDic.Count);//1.����
                List<string> keys = new List<string>(assetDic.Keys);
                keys.Sort();

                for (ushort i = 0; i < keys.Count; i++)
                {
                    string assetUrl = keys[i];
                    assetIdDic.Add(assetUrl, i);
                    resourceSb.AppendLine($"{i}\t{assetUrl}");
                    resourceBw.Write(assetUrl);//2.����url
                }

                resourceMs.Flush();
                byte[] buffer = resourceMs.GetBuffer();
                resourceBw.Close();

                File.WriteAllText(ResourceTXTPath, resourceSb.ToString(), Encoding.UTF8);
                File.WriteAllBytes(ResourceBYTEPath, buffer);
            }
            #endregion

            EditorUtility.DisplayProgressBar($"{nameof(GenerateManifest)}", "���ɴ����Ϣ", min + (max - min) * 0.33f);

            #region ����bundle������Ϣ
            {
                //ɾ���ϴ������ļ�
                if (File.Exists(BundleTXTPath)) File.Delete(BundleTXTPath);
                if (File.Exists(BundleBYTEPath)) File.Delete(BundleBYTEPath);

                StringBuilder bundleSb = new StringBuilder();
                MemoryStream bundleMs = new MemoryStream();
                BinaryWriter bundleBw = new BinaryWriter(bundleMs);

                bundleBw.Write((ushort)bundleDic.Count);//1.Bundle����
                foreach (var kv in bundleDic)
                {
                    string bundleName = kv.Key;
                    List<string> assets = kv.Value;

                    //д��bundle
                    bundleSb.AppendLine(bundleName);
                    bundleBw.Write(bundleName);//2.Bundle��

                    bundleBw.Write((ushort)assets.Count);//3.��Դ����

                    for (int i = 0; i < assets.Count; i++)
                    {
                        string assetUrl = assets[i];
                        ushort assetId = assetIdDic[assetUrl];
                        bundleSb.AppendLine($"\t{assetUrl}");
                        bundleBw.Write(assetId);//4.��Դid  Tip����id�滻�ַ������Խ�ʡ�ڴ�
                    }
                }

                bundleMs.Flush();
                byte[] buffer = bundleMs.GetBuffer();
                bundleBw.Close();

                File.WriteAllText(BundleTXTPath, bundleSb.ToString(), Encoding.UTF8);
                File.WriteAllBytes(BundleBYTEPath, buffer);
            }
            #endregion

            EditorUtility.DisplayProgressBar($"{nameof(GenerateManifest)}", "���ɴ����Ϣ", min + (max - min) * 0.66f);

            #region ������Դ����������Ϣ
            {
                //ɾ���ϴ������ļ�
                if (File.Exists(DependencyTXTPath)) File.Delete(DependencyTXTPath);
                if (File.Exists(DependencyBYTEPath)) File.Delete(DependencyBYTEPath);

                StringBuilder dependencySb = new StringBuilder();
                MemoryStream dependencyMs = new MemoryStream();
                BinaryWriter dependencyBw = new BinaryWriter(dependencyMs);

                List<List<ushort>> dependencyList = new List<List<ushort>>();//���ڱ�����Դ������
                foreach (var kv in dependencyDic)
                {
                    List<string> dependencyAssets = kv.Value;
                    if (dependencyAssets.Count == 0) continue;//û����������ִ��

                    string assetUrl = kv.Key;

                    //����ĳ��assetUrl�������ļ���·����id(�����Լ�)
                    List<ushort> ids = new List<ushort>();
                    ids.Add(assetIdDic[assetUrl]);

                    string content = assetUrl;
                    for (int i = 0; i < dependencyAssets.Count; i++)
                    {
                        string dependencyAssetUrl = dependencyAssets[i];
                        content += $"\t{dependencyAssetUrl}";
                        ids.Add(assetIdDic[dependencyAssetUrl]);
                    }

                    dependencySb.AppendLine(content);

                    if (ids.Count > byte.MaxValue)
                    {
                        EditorUtility.ClearProgressBar();
                        MLog.Print($"{typeof(Builder)}.{nameof(GenerateManifest)}����Դ{assetUrl}����������һ���ֽ�����:{byte.MaxValue}", MLogType.Error);
                    }

                    dependencyList.Add(ids);
                }

                dependencyBw.Write((ushort)dependencyList.Count);//1.����������
                for (int i = 0; i < dependencyList.Count; i++)
                {
                    //2.ĳ��·��������Դ����+����������id
                    List<ushort> ids = dependencyList[i];
                    dependencyBw.Write((ushort)ids.Count);
                    for (int j = 0; j < ids.Count; j++)
                    {
                        dependencyBw.Write(ids[j]);
                    }
                }

                dependencyMs.Flush();
                byte[] buffer = dependencyMs.GetBuffer();
                dependencyBw.Close();

                File.WriteAllText(DependencyTXTPath, dependencySb.ToString(), Encoding.UTF8);
                File.WriteAllBytes(DependencyBYTEPath, buffer);
            }
            #endregion

            AssetDatabase.Refresh();

            EditorUtility.DisplayProgressBar($"{nameof(GenerateManifest)}", "���ɴ����Ϣ", max);

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// ����3������ABManifest�ļ�
        /// </summary>
        private static AssetBundleManifest BuildBundle(Dictionary<string, List<string>> bundleDic)
        {
            float min = ms_Progress[4].x, max = ms_Progress[4].y;//[0.6,0.7]

            EditorUtility.DisplayProgressBar($"{nameof(BuildBundle)}", "���AssetBundle", min);

            //Tip���ò������������е�ab�ļ�
            if (!Directory.Exists(BuildPath)) Directory.CreateDirectory(BuildPath);
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(BuildPath, GetBuilds(bundleDic), BuildAssetBundleOptions, EditorUserBuildSettings.activeBuildTarget);

            EditorUtility.DisplayProgressBar($"{nameof(BuildBundle)}", "���AssetBundle", max);

            return manifest;
        }
        /// <summary>
        /// ͨ��bundleDic��Ϣ���Unity�����AssetBundleBuild
        /// </summary>
        private static AssetBundleBuild[] GetBuilds(Dictionary<string, List<string>> bundleDic)
        {
            int index = 0;
            AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[bundleDic.Count];
            foreach (var pair in bundleDic)
            {
                assetBundleBuilds[index++] = new AssetBundleBuild()
                {
                    assetBundleName = pair.Key,
                    assetNames = pair.Value.ToArray(),
                };
            }

            return assetBundleBuilds;
        }

        /// <summary>
        /// ����4������AB�ļ�
        /// </summary>
        private static void ClearBundle(string path, Dictionary<string, List<string>> bundleDic)
        {
            float min = ms_Progress[5].x, max = ms_Progress[5].y;//[0.7,0.9]

            EditorUtility.DisplayProgressBar($"{nameof(ClearBundle)}", "��������AssetBundle�ļ�", min);
            
            //��ȡ·���µ������ļ�
            List<string> fileList = MPathUtility.GetFiles(path, null, null);
            HashSet<string> fileSet = new HashSet<string>(fileList);

            //��HashSet��ɾ��bundleDic�е������ļ�
            foreach (string bundle in bundleDic.Keys)
            {
                fileSet.Remove($"{path}{bundle}");
                fileSet.Remove($"{path}{bundle}{BUNDLE_MANIFEST_SUFFIX}");
            }
            fileSet.Remove($"{path}{PLATFORM}");
            fileSet.Remove($"{path}{PLATFORM}{BUNDLE_MANIFEST_SUFFIX}");

            //fileSet��ʣ��·��Ϊ�����ļ�
            Parallel.ForEach(fileSet, ParallelOptions, File.Delete);

            EditorUtility.DisplayProgressBar($"{nameof(ClearBundle)}", "��������AssetBundle�ļ�", max);
        }

        /// <summary>
        /// ����5�����Manifest
        /// </summary>
        private static void BuildManifest()
        {
            float min = ms_Progress[6].x, max = ms_Progress[6].y;//[0.9,1]

            EditorUtility.DisplayProgressBar($"{nameof(BuildManifest)}", "��Manifest�����AssetBundle", min);

            string TempBuildPath = $"{MSettings.TempRootPath}/AB";
            if (!Directory.Exists(TempBuildPath)) Directory.CreateDirectory(TempBuildPath);

            string prefix = MSettings.RootPath + "/";
            //����manifest�ļ�
            AssetBundleBuild manifest = new AssetBundleBuild();
            manifest.assetBundleName = $"{MANIFEST}{BUNDLE_SUFFIX}";
            manifest.assetNames = new string[3]
            {
                ResourceBYTEPath.Replace(prefix,""),
                BundleBYTEPath.Replace(prefix,""),
                DependencyBYTEPath.Replace(prefix,""),
            };

            EditorUtility.DisplayProgressBar($"{nameof(BuildManifest)}", "��Manifest�����AssetBundle", min + (max - min) * 0.5f);

            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(TempBuildPath, new AssetBundleBuild[] { manifest }, BuildAssetBundleOptions, EditorUserBuildSettings.activeBuildTarget);

            //����manifest�ļ�(Tip��ֻ������manifest.ab�ļ�)
            if (assetBundleManifest)
            {
                string manifestFile = $"{TempBuildPath}/{MANIFEST}{BUNDLE_SUFFIX}";
                string target = $"{BuildPath}{MANIFEST}{BUNDLE_SUFFIX}";
                if (File.Exists(manifestFile))
                {
                    File.Copy(manifestFile, target);
                }
            }

            if (Directory.Exists(TempBuildPath)) Directory.Delete(TempBuildPath, true);

            EditorUtility.DisplayProgressBar($"{nameof(BuildManifest)}", "��Manifest�����AssetBundle", max);
        }

        /// <summary>
        /// �л�ƽ̨(�첽)
        /// </summary>
        /// <returns></returns>
        internal static async Task<bool> SwitchPlatform()
        {
            //ʹ���첽�ȴ���ť����
            int platformInt = await MEditorUtility.DisplayDialogAsync("Switch Platform", "��ѡ��ƽ̨��", "Windows", "Android", "iOS");

            if (platformInt == 0)//Windows
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64) return true;
                MLog.Print("��ǰ���ƽ̨����ȷ�������л�ƽ̨...", MLogType.Warning);
                EditorDelayExecute.Instance.DelayDo(SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64));
            }
            else if (platformInt == 1)//Android
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) return true;
                MLog.Print("��ǰ���ƽ̨����ȷ�������л�ƽ̨...", MLogType.Warning);
                EditorDelayExecute.Instance.DelayDo(SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android));
            }
            else if (platformInt == 2)//iOS
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) return true;
                MLog.Print("��ǰ���ƽ̨����ȷ�������л�ƽ̨...", MLogType.Warning);
                EditorDelayExecute.Instance.DelayDo(SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS));
            }
            return false;
        }
        private static IEnumerator SwitchActiveBuildTarget(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            yield return new WaitForSeconds(3.0f);

            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        }
    }
}