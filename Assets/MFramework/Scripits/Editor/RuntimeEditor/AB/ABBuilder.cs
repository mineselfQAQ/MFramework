using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    //TODO:最好在XML中添加隔离目标(某Scene不需要打进包，但是只要在文件夹中就必须打入，会导致依赖增多)
    /// <summary>
    /// AB包构建核心类
    /// </summary>
    public class ABBuilder
    {
        private static readonly Profiler ms_BuildProfiler = new Profiler(nameof(ABBuilder));
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

        //根据当前平台选择打包平台名称(用于路径)
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
        /// 打包设置
        /// </summary>
        public static readonly BuildAssetBundleOptions BuildAssetBundleOptions =
            BuildAssetBundleOptions.ChunkBasedCompression | //LZ4压缩
            BuildAssetBundleOptions.StrictMode | //报错则打包不成狗
            BuildAssetBundleOptions.DisableLoadAssetByFileName | //禁用(Player)搜索方式
            BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension; //禁用(Player.prefab)搜索方式

        /// <summary>
        /// 并行设置
        /// </summary>
        public static readonly ParallelOptions ParallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount * 2
        };

        /// <summary>
        /// 打包路径
        /// </summary>
        public static readonly string BuildSettingPath = MSettings.ABBuildSettingName;

        /// <summary>
        /// 打包设置信息
        /// </summary>
        public static BuildSetting BuildSetting { get; private set; }

        /// <summary>
        /// 打包目录
        /// </summary>
        public static string BuildPath { get; set; }

        /// <summary>
        /// 打包时的AB打包
        /// </summary>
        public static void Build_Windows(string pathToBuiltProject)
        {
            BuildSetting = LoadSetting(BuildSettingPath);
            BuildPath = $"{pathToBuiltProject.CD()}/{Application.productName}_AssetBundle/{PLATFORM}/";//更改BuildPath为项目根目录
            Dictionary<string, List<string>> bundleDic = Collect();
            BuildBundle(bundleDic);
            ClearBundle(BuildPath, bundleDic);
            BuildManifest();

            EditorUtility.ClearProgressBar();
        }

        internal static void BuildInternal()
        {
            ms_BuildProfiler.Start();

            //加载设置
            ms_LoadBuildSettingProfiler.Start();
            BuildSetting = LoadSetting(BuildSettingPath);
            ms_LoadBuildSettingProfiler.Stop();

            //搜集bundle信息
            ms_CollectProfiler.Start();
            Dictionary<string, List<string>> bundleDic = Collect();
            ms_CollectProfiler.Stop();

            //打包成assetbundle
            ms_BuildBundleProfiler.Start();
            BuildBundle(bundleDic);
            ms_BuildBundleProfiler.Stop();

            //清空多余文件
            ms_ClearBundleProfiler.Start();
            ClearBundle(BuildPath, bundleDic);
            ms_ClearBundleProfiler.Stop();

            //把描述文件打包assetbundle
            ms_BuildManifestBundleProfiler.Start();
            BuildManifest();
            ms_BuildManifestBundleProfiler.Stop();

            EditorUtility.ClearProgressBar();

            ms_BuildProfiler.Stop();

            EditorUtility.RevealInFinder(BuildPath);

            MLog.Print($"打包完成{ms_BuildProfiler}");
        }

        /// <summary>
        /// 步骤1：加载BuildSetting
        /// </summary>
        private static BuildSetting LoadSetting(string settingPath)
        {
            //Tip：目前ReadFromXml()只会读取XmlSettings文件夹下的文件，所以必须放入
            BuildSetting = MSerializationUtility.ReadFromXml<BuildSetting>(settingPath);
            if (BuildSetting == null)
            {
                MLog.Print($"{typeof(ABBuilder)}：路径{settingPath}加载失败，请检查", MLogType.Error);
                return null;
            }
            BuildSetting.Init();//buildSetting内部初始化(核心为收集itemDic信息)

            //获取绝对打包路径
            BuildPath = BuildSetting.buildRoot;
            if (BuildPath.Length > 0 && BuildPath[BuildPath.Length - 1] != '/')
            {
                BuildPath += "/";
            }
            BuildPath += $"{PLATFORM}/";

            return BuildSetting;
        }

        /// <summary>
        /// 步骤2：收集信息(整体)
        /// </summary>
        private static Dictionary<string, List<string>> Collect()
        {
            //2.1：收集打包资源路径
            //files---XML中描述的所需的所有文件
            ms_CollectBuildSettingFileProfiler.Start();
            HashSet<string> files = BuildSetting.FileCollect();
            ms_CollectBuildSettingFileProfiler.Stop();

            //2.2：搜集files的依赖关系文件
            //dependencyDic---files的依赖文件以及依赖文件的依赖文件
            ms_CollectDependencyProfiler.Start();
            Dictionary<string, List<string>> dependencyDic = CollectDependency(files);
            ms_CollectDependencyProfiler.Stop();

            //标记文件类型
            //assetDic---所有文件路径的文件类型
            Dictionary<string, ResourceType> assetDic = new Dictionary<string, ResourceType>();
            //files必定是Direct的
            foreach (string url in files)
            {
                assetDic.Add(url, ResourceType.Direct);
            }
            //dependencyDic中除了files的文件都是Dependency的
            foreach (string url in dependencyDic.Keys)
            {
                if (!assetDic.ContainsKey(url))
                {
                    assetDic.Add(url, ResourceType.Dependency);
                }
            }

            //2.3：获取AB包信息
            //bundleDic---每个bundle所对应的所属文件路径
            ms_CollectBundleProfiler.Start();
            Dictionary<string, List<string>> bundleDic = CollectBundle(BuildSetting, assetDic, dependencyDic);
            ms_CollectBundleProfiler.Stop();

            //2.4：生成Manifest文件
            ms_GenerateManifestProfiler.Start();
            GenerateManifest(assetDic, bundleDic, dependencyDic);
            ms_GenerateManifestProfiler.Stop();

            return bundleDic;
        }

        /// <summary>
        /// 步骤2.2：搜集files的依赖关系文件
        /// </summary>
        private static Dictionary<string, List<string>> CollectDependency(ICollection<string> files)
        {
            float min = ms_Progress[1].x, max = ms_Progress[1].y;//[0.2,0.4]

            Dictionary<string, List<string>> dependencyDic = new Dictionary<string, List<string>>();
            List<string> fileList = new List<string>(files);

            //对于每一个文件路径
            int segmentCount = 10;//段数
            int segmentSize = Mathf.Max(1, files.Count / segmentCount);//每段执行个数
            for (int i = 0; i < fileList.Count; i++)
            {
                if (i % segmentSize == 0 || i == files.Count - 1)//当达到一段或最后一个元素时更改进度条
                {
                    float progress = min + (max - min) * ((float)i / (files.Count - 1));
                    EditorUtility.DisplayProgressBar($"{nameof(CollectDependency)}", "搜集文件依赖关系", progress);
                }

                string assetUrl = fileList[i];
                if (dependencyDic.ContainsKey(assetUrl)) continue;//文件路径已存在，不进行操作

                //通过Unity的GetDependencies()获取所有依赖路径
                string[] dependencies = AssetDatabase.GetDependencies(assetUrl, false);//**核心**
                List<string> dependencyList = new List<string>(dependencies.Length);

                //过滤掉不符合要求的依赖
                for (int j = 0; j < dependencies.Length; j++)
                {
                    string tempAssetUrl = dependencies[j];
                    string extension = Path.GetExtension(tempAssetUrl).ToLower();
                    //不需要cs文件与dll文件
                    if (string.IsNullOrEmpty(extension) || extension == ".cs" || extension == ".dll") continue;
                    //对于其它符合要求的文件都会存到正式的dependencyList中，除此以外，如果文件列表中没有该文件，需要加入并继续寻找它的依赖(依赖的依赖)
                    dependencyList.Add(tempAssetUrl);
                    if (!fileList.Contains(tempAssetUrl)) fileList.Add(tempAssetUrl);
                }

                //Tip：会比原来的files更多，因为存在依赖文件(files的依赖/依赖的依赖)
                dependencyDic.Add(assetUrl, dependencyList);
            }

            try
            {
                bool hasCycle = HasCycle(dependencyDic);
                if (hasCycle) throw new Exception();
            }
            catch (Exception) 
            {
                MLog.Print($"{nameof(CollectDependency)}：发生依赖循环，请检查", MLogType.Warning);
            }

            return dependencyDic;
        }
        public static bool HasCycle(Dictionary<string, List<string>> dependencyDic)
        {
            HashSet<string> visited = new HashSet<string>();//记录已访问的节点
            HashSet<string> stack = new HashSet<string>();//记录当前递归路径中的节点
            List<string> path = new List<string>();//当前路径
            List<List<string>> allCycles = new List<List<string>>();

            //遍历每个节点，检查是否有循环
            foreach (var asset in dependencyDic.Keys)
            {
                CheckDFS(asset, dependencyDic, allCycles, visited, stack, path);
            }

            //输出所有检测到的循环依赖链
            if (allCycles.Count > 0)
            {
                MLog.Print("检测到的循环依赖路径有：");
                foreach (var cycle in allCycles)
                {
                    MLog.Print(string.Join(" -> ", cycle));
                }
                return true;//存在循环依赖
            }
            else
            {
                return false;//没有循环依赖
            }
        }
        private static bool CheckDFS(string asset, Dictionary<string, List<string>> dependencyDic, List<List<string>> allCycles, HashSet<string> visited, HashSet<string> stack, List<string> path)
        {
            //如果该节点已经在当前路径中，说明存在循环依赖
            if (stack.Contains(asset))
            {
                //找到循环，记录并存储循环依赖路径
                int cycleStartIndex = path.IndexOf(asset);
                List<string> cycle = new List<string>(path.GetRange(cycleStartIndex, path.Count - cycleStartIndex));
                cycle.Add(asset);//加上起始节点形成完整循环
                allCycles.Add(cycle);//保存该循环

                return true;
            }

            //如果该节点已经访问过，且没有循环，跳过
            if (visited.Contains(asset))
            {
                return false;
            }

            //标记该节点为正在访问
            stack.Add(asset);
            path.Add(asset);//添加到当前路径

            //递归检查该节点的依赖
            if (dependencyDic.ContainsKey(asset))
            {
                foreach (var dependency in dependencyDic[asset])
                {
                    CheckDFS(dependency, dependencyDic, allCycles, visited, stack, path);
                }
            }

            //递归完成，移除该节点
            stack.Remove(asset);
            path.RemoveAt(path.Count - 1);//回溯时移除路径上的节点
            visited.Add(asset);

            return false;
        }

        /// <summary>
        /// 步骤2.3：获取AB包信息
        /// </summary>
        private static Dictionary<string, List<string>> CollectBundle(BuildSetting buildSetting, Dictionary<string, ResourceType> assetDic, Dictionary<string, List<string>> dependencyDic)
        {
            float min = ms_Progress[2].x, max = ms_Progress[2].y;//[0.4,0.5]

            EditorUtility.DisplayProgressBar($"{nameof(CollectBundle)}", "搜集bundle信息", min);

            Dictionary<string, List<string>> bundleDic = new Dictionary<string, List<string>>();
            List<string> notInRuleList = new List<string>();//外部资源

            int index = 0;
            foreach (KeyValuePair<string, ResourceType> pair in assetDic)
            {
                index++;
                string assetUrl = pair.Key;
                //关键---获取资源所属Bundle名
                string bundleName = buildSetting.GetBundleName(assetUrl, pair.Value);

                //没有bundleName的资源为外部资源(XML所指示以外的资源)
                if (bundleName == null)
                {
                    notInRuleList.Add(assetUrl);
                    continue;
                }

                List<string> list;
                //没有Key就加上Key，有Key就取出Value
                if (!bundleDic.TryGetValue(bundleName, out list))
                {
                    list = new List<string>();
                    bundleDic.Add(bundleName, list);
                }
                list.Add(assetUrl);

                EditorUtility.DisplayProgressBar($"{nameof(CollectBundle)}", "搜集bundle信息", min + (max - min) * ((float)index / assetDic.Count));
            }

            //外部资源不可用(即不在ABBuildSetting.xml中的资源)
            if (notInRuleList.Count > 0)
            {
                string message = string.Empty;
                for (int i = 0; i < notInRuleList.Count; i++)
                {
                    message += "\n" + notInRuleList[i];
                }
                EditorUtility.ClearProgressBar();
                MLog.Print($"{typeof(ABBuilder)}：存在意外或后缀不匹配的资源：   {MLog.Color("Tip：如果是Prefab，需要先获取其引用", Color.red)}{message}", MLogType.Error);
            }

            //将内部理顺(也就是排序)
            foreach (List<string> list in bundleDic.Values)
            {
                list.Sort();
            }

            return bundleDic;
        }

        /// <summary>
        /// 步骤2.4：生成Manifest文件
        /// </summary>
        private static void GenerateManifest(Dictionary<string, ResourceType> assetDic, Dictionary<string, List<string>> bundleDic, Dictionary<string, List<string>> dependencyDic)
        {
            float min = ms_Progress[3].x, max = ms_Progress[3].y;//[0.5,0.6]

            EditorUtility.DisplayProgressBar($"{nameof(GenerateManifest)}", "生成打包信息", min);

            //生成临时存放文件的目录
            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }

            //Tip:
            //.txt文件用于可视化查看
            //.bytes文件用于创建(所以是关键)

            //资源映射id
            Dictionary<string, ushort> assetIdDic = new Dictionary<string, ushort>();

            #region 生成资源描述信息
            {
                //删除上次遗留文件
                if (File.Exists(ResourceTXTPath)) File.Delete(ResourceTXTPath);
                if (File.Exists(ResourceBYTEPath)) File.Delete(ResourceBYTEPath);

                StringBuilder resourceSb = new StringBuilder();
                MemoryStream resourceMs = new MemoryStream();
                BinaryWriter resourceBw = new BinaryWriter(resourceMs);
                if (assetDic.Count > ushort.MaxValue)
                {
                    EditorUtility.ClearProgressBar();
                    MLog.Print($"{typeof(ABBuilder)}.{nameof(GenerateManifest)}：资源个数超出{ushort.MaxValue}", MLogType.Error);
                }

                resourceBw.Write((ushort)assetDic.Count);//1.个数
                List<string> keys = new List<string>(assetDic.Keys);
                keys.Sort();

                for (ushort i = 0; i < keys.Count; i++)
                {
                    string assetUrl = keys[i];
                    assetIdDic.Add(assetUrl, i);
                    resourceSb.AppendLine($"{i}\t{assetUrl}");
                    resourceBw.Write(assetUrl);//2.所有url
                }

                resourceMs.Flush();
                byte[] buffer = resourceMs.GetBuffer();
                resourceBw.Close();

                File.WriteAllText(ResourceTXTPath, resourceSb.ToString(), Encoding.UTF8);
                File.WriteAllBytes(ResourceBYTEPath, buffer);
            }
            #endregion

            EditorUtility.DisplayProgressBar($"{nameof(GenerateManifest)}", "生成打包信息", min + (max - min) * 0.33f);

            #region 生成bundle描述信息
            {
                //删除上次遗留文件
                if (File.Exists(BundleTXTPath)) File.Delete(BundleTXTPath);
                if (File.Exists(BundleBYTEPath)) File.Delete(BundleBYTEPath);

                StringBuilder bundleSb = new StringBuilder();
                MemoryStream bundleMs = new MemoryStream();
                BinaryWriter bundleBw = new BinaryWriter(bundleMs);

                bundleBw.Write((ushort)bundleDic.Count);//1.Bundle个数
                foreach (var kv in bundleDic)
                {
                    string bundleName = kv.Key;
                    List<string> assets = kv.Value;

                    //写入bundle
                    bundleSb.AppendLine(bundleName);
                    bundleBw.Write(bundleName);//2.Bundle名

                    bundleBw.Write((ushort)assets.Count);//3.资源个数

                    for (int i = 0; i < assets.Count; i++)
                    {
                        string assetUrl = assets[i];
                        ushort assetId = assetIdDic[assetUrl];//Id跟着assetIdDic走
                        bundleSb.AppendLine($"\t{assetUrl}");
                        bundleBw.Write(assetId);//4.资源id  Tip：用id替换字符串可以节省内存
                    }
                }

                bundleMs.Flush();
                byte[] buffer = bundleMs.GetBuffer();
                bundleBw.Close();

                File.WriteAllText(BundleTXTPath, bundleSb.ToString(), Encoding.UTF8);
                File.WriteAllBytes(BundleBYTEPath, buffer);
            }
            #endregion

            EditorUtility.DisplayProgressBar($"{nameof(GenerateManifest)}", "生成打包信息", min + (max - min) * 0.66f);

            #region 生成资源依赖描述信息
            {
                //删除上次遗留文件
                if (File.Exists(DependencyTXTPath)) File.Delete(DependencyTXTPath);
                if (File.Exists(DependencyBYTEPath)) File.Delete(DependencyBYTEPath);

                StringBuilder dependencySb = new StringBuilder();
                MemoryStream dependencyMs = new MemoryStream();
                BinaryWriter dependencyBw = new BinaryWriter(dependencyMs);

                List<List<ushort>> dependencyList = new List<List<ushort>>();//用于保存资源依赖链
                foreach (var kv in dependencyDic)
                {
                    List<string> dependencyAssets = kv.Value;
                    if (dependencyAssets.Count == 0) continue;//没有依赖无需执行

                    string assetUrl = kv.Key;

                    //所有某个assetUrl的依赖文件的路径的id(包括自己)
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
                        MLog.Print($"{typeof(ABBuilder)}.{nameof(GenerateManifest)}：资源{assetUrl}的依赖超出一个字节上限:{byte.MaxValue}", MLogType.Error);
                    }

                    dependencyList.Add(ids);
                }

                dependencyBw.Write((ushort)dependencyList.Count);//1.依赖链个数
                for (int i = 0; i < dependencyList.Count; i++)
                {
                    //2.某个路径依赖资源个数+其所有依赖id
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

            EditorUtility.DisplayProgressBar($"{nameof(GenerateManifest)}", "生成打包信息", max);

            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 步骤3：生成ABManifest文件
        /// </summary>
        private static AssetBundleManifest BuildBundle(Dictionary<string, List<string>> bundleDic)
        {
            float min = ms_Progress[4].x, max = ms_Progress[4].y;//[0.6,0.7]

            EditorUtility.DisplayProgressBar($"{nameof(BuildBundle)}", "打包AssetBundle", min);

            //Tip：该操作可生成所有的ab文件
            if (!Directory.Exists(BuildPath)) Directory.CreateDirectory(BuildPath);
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(BuildPath, GetBuilds(bundleDic), BuildAssetBundleOptions, EditorUserBuildSettings.activeBuildTarget);

            EditorUtility.DisplayProgressBar($"{nameof(BuildBundle)}", "打包AssetBundle", max);

            return manifest;
        }
        /// <summary>
        /// 通过bundleDic信息组成Unity所需的AssetBundleBuild
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
        /// 步骤4：清理AB文件
        /// </summary>
        private static void ClearBundle(string path, Dictionary<string, List<string>> bundleDic)
        {
            float min = ms_Progress[5].x, max = ms_Progress[5].y;//[0.7,0.9]

            EditorUtility.DisplayProgressBar($"{nameof(ClearBundle)}", "清除多余的AssetBundle文件", min);

            //获取路径下的所有文件
            List<string> fileList = MPathUtility.GetFiles(path, null, null);
            HashSet<string> fileSet = new HashSet<string>(fileList);

            //在HashSet中删除bundleDic中的所有文件
            foreach (string bundle in bundleDic.Keys)
            {
                fileSet.Remove($"{path}{bundle}");
                fileSet.Remove($"{path}{bundle}{BUNDLE_MANIFEST_SUFFIX}");
            }
            fileSet.Remove($"{path}{PLATFORM}");
            fileSet.Remove($"{path}{PLATFORM}{BUNDLE_MANIFEST_SUFFIX}");

            //fileSet中剩余路径为多余文件
            Parallel.ForEach(fileSet, ParallelOptions, File.Delete);

            EditorUtility.DisplayProgressBar($"{nameof(ClearBundle)}", "清除多余的AssetBundle文件", max);
        }

        /// <summary>
        /// 步骤5：打包Manifest
        /// </summary>
        private static void BuildManifest()
        {
            float min = ms_Progress[6].x, max = ms_Progress[6].y;//[0.9,1]

            EditorUtility.DisplayProgressBar($"{nameof(BuildManifest)}", "将Manifest打包成AssetBundle", min);

            string TempBuildPath = $"{MSettings.TempRootPath}/AB";
            if (!Directory.Exists(TempBuildPath)) Directory.CreateDirectory(TempBuildPath);

            string prefix = MSettings.RootPath + "/";
            //创建manifest文件
            AssetBundleBuild manifest = new AssetBundleBuild();
            manifest.assetBundleName = $"{MANIFEST}{BUNDLE_SUFFIX}";
            manifest.assetNames = new string[3]
            {
                ResourceBYTEPath.Replace(prefix,""),
                BundleBYTEPath.Replace(prefix,""),
                DependencyBYTEPath.Replace(prefix,""),
            };

            EditorUtility.DisplayProgressBar($"{nameof(BuildManifest)}", "将Manifest打包成AssetBundle", min + (max - min) * 0.5f);

            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(TempBuildPath, new AssetBundleBuild[] { manifest }, BuildAssetBundleOptions, EditorUserBuildSettings.activeBuildTarget);

            //复制manifest文件(Tip：只复制了manifest.ab文件)
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

            EditorUtility.DisplayProgressBar($"{nameof(BuildManifest)}", "将Manifest打包成AssetBundle", max);
        }

        /// <summary>
        /// 切换平台(异步)
        /// </summary>
        /// <returns></returns>
        internal static async Task<bool> SwitchPlatform()
        {
            //使用异步等待按钮按下
            int platformInt = await MEditorUtility.DisplayDialogAsync("Switch Platform", "请选择平台：", "Windows", "Android", "iOS");

            if (platformInt == 0)//Windows
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64) return true;
                MLog.Print("当前打包平台不正确，即将切换平台...", MLogType.Warning);
                EditorDelayExecute.Instance.DelayDo(SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64));
            }
            else if (platformInt == 1)//Android
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) return true;
                MLog.Print("当前打包平台不正确，即将切换平台...", MLogType.Warning);
                EditorDelayExecute.Instance.DelayDo(SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android));
            }
            else if (platformInt == 2)//iOS
            {
                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) return true;
                MLog.Print("当前打包平台不正确，即将切换平台...", MLogType.Warning);
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