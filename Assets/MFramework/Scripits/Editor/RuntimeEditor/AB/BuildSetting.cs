using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class BuildSetting
    {
        [XmlAttribute("ProjectName")]
        public string projectName { get; set; }//项目名称

        [XmlAttribute("SuffixList")]
        public List<string> suffixList { get; set; } = new List<string>();//后缀列表

        [XmlAttribute("BuildRoot")]
        public string buildRoot { get; set; }//打包文件的目标文件夹

        [XmlElement("BuildItem")]
        public List<BuildItem> items { get; set; } = new List<BuildItem>();//打包选项



        [XmlIgnore]
        public Dictionary<string, BuildItem> itemDic = new Dictionary<string, BuildItem>();//将所有BuildItem使用路径作为标记组成字典



        /// <summary>
        /// 初始化---1.获取buildRoot 2.收集itemDic信息(使用路径作为key)
        /// </summary>
        internal void Init()
        {
            //获取完整buildRoot路径(取决于.xml文件，默认在与项目同级)
            buildRoot = Path.GetFullPath(buildRoot).ReplaceSlash();
            buildRoot = buildRoot.Replace("{ProjectName}", Application.productName);

            itemDic.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                BuildItem buildItem = items[i];

                //意外情况---文件夹级别却没有文件夹(是文件(也就是有后缀))
                if (buildItem.bundleType == BundleType.All || buildItem.bundleType == BundleType.Directory)
                {
                    if (!Directory.Exists(buildItem.assetPath))
                    {
                        MLog.Print($"{typeof(BuildSetting)}.{nameof(Init)}：不存在资源路径-<{buildItem.assetPath}>，请检查路径后重试", MLogType.Error);
                        return;
                    }
                }

                //处理后缀
                string[] prefixes = buildItem.suffix.Split('|');
                for (int j = 0; j < prefixes.Length; j++)
                {
                    string prefix = prefixes[j].Trim();
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        buildItem.suffixes.Add(prefix);//存入真正的后缀列表(未拆分的为suffix)
                    }
                }

                //意外情况---出现两个相同的assetPath的文件
                if (itemDic.ContainsKey(buildItem.assetPath))
                {
                    MLog.Print($"{typeof(BuildSetting)}.{nameof(Init)}：重复的资源路径-<{buildItem.assetPath}>，请重试", MLogType.Error);
                    return;
                }

                itemDic.Add(buildItem.assetPath, buildItem);//将BuildItem放入
            }
        }

        /// <summary>
        /// 步骤2.1：收集打包资源路径
        /// </summary>
        /// <returns></returns>
        internal HashSet<string> FileCollect()
        {
            float min = ABBuilder.ms_Progress[0].x, max = ABBuilder.ms_Progress[0].y;//[0,0.2]

            EditorUtility.DisplayProgressBar($"{nameof(FileCollect)}", "搜集打包资源", min);

            //获取忽略列表
            for (int i = 0; i < items.Count; i++)
            {
                BuildItem buildItem_i = items[i];

                //只能执行XML文件中的资源(XML中必为Direct)
                if (buildItem_i.resourceType != ResourceType.Direct) continue;

                //忽略路径就是：
                //比如说i的路径为"Assets/Common"，如果j的路径是"Assets/Common/A"的话，
                //说明j是i的一部分，既然是这样，那么i必然不应该包含j
                //它只需要关心"Assets/Common/xxx.jpg"这种目录下的文件或者"Assets/Common/B"这种与A同级的目录即可
                buildItem_i.ignorePaths.Clear();
                for (int j = 0; j < items.Count; j++)
                {
                    BuildItem buildItem_j = items[j];
                    if (i != j && buildItem_j.resourceType == ResourceType.Direct)
                    {
                        //如果j是i的子文件夹，就说明i中需要忽略j
                        if (buildItem_j.assetPath.StartsWith(buildItem_i.assetPath, StringComparison.InvariantCulture))
                        {
                            buildItem_i.ignorePaths.Add(buildItem_j.assetPath);
                        }
                    }
                }
            }

            //获取所有文件
            HashSet<string> files = new HashSet<string>();
            for (int i = 0; i < items.Count; i++)
            {
                BuildItem buildItem = items[i];

                if (buildItem.resourceType != ResourceType.Direct) continue;//同上

                //寻找文件，要求：
                //1.在assetPath中寻找 2.无前缀要求 3.有尾缀要求(在LoadSetting()中获取了suffixes，也就是XML表中的后缀列表)
                List<string> tempFiles = MPathUtility.GetFiles(buildItem.assetPath, null, buildItem.suffixes.ToArray());
                //添加文件(需要忽略某些文件)
                for (int j = 0; j < tempFiles.Count; j++)
                {
                    string file = tempFiles[j];
                    //Tip:逻辑如下
                    //assetPath---一个基础路径
                    //ignorePaths---assetPath下的子路径(需要忽略)
                    //file---assetPath下收集到的文件(必定包括ignorePaths下的文件)
                    //那么想必会出现一种情况：
                    //assetPath为"A/B"，ignorePath有"A/B/C"
                    //那么如果file为"A/B/C/pic.png"就应该忽略(此时file为ignorePaths的子文件)
                    if (IsIgnore(buildItem.ignorePaths, file)) continue;

                    files.Add(file);
                }

                EditorUtility.DisplayProgressBar($"{nameof(FileCollect)}", "搜集打包资源", min + (max - min) * ((float)i / (items.Count - 1)));
            }

            return files;
        }

        /// <summary>
        /// 获取Bundle名
        /// 对于File级别，获取到如：assets/assetbundle/background/1.png.ab(1.png为文件)
        /// 对于Directory级别，获取到如：assets/assetbundle/atlas/role.ab(role为文件夹)
        /// 对于All级别，直接使用assetPath
        /// </summary>
        internal string GetBundleName(string assetUrl, ResourceType resourceType)
        {
            //获取assetUrl所属BuildItem
            BuildItem buildItem = GetBuildItem(assetUrl);
            if (buildItem == null) return null;

            //依赖类型一定要匹配后缀
            if (buildItem.resourceType == ResourceType.Dependency)
            {
                string extension = Path.GetExtension(assetUrl).ToLower();
                bool exist = false;
                for (int i = 0; i < buildItem.suffixes.Count; i++)
                {
                    if (buildItem.suffixes[i] == extension)
                    {
                        exist = true;
                    }
                }

                if (!exist) return null;
            }

            //***注意***
            //必须换小写，否则无法打包成功
            string name = null;
            switch (buildItem.bundleType)
            {
                case BundleType.All:
                    name = buildItem.assetPath;
                    if (buildItem.assetPath[buildItem.assetPath.Length - 1] == '/')
                    {
                        name = buildItem.assetPath.Substring(0, buildItem.assetPath.Length - 1);
                    }
                    name = $"{name}{ABBuilder.BUNDLE_SUFFIX}".ToLowerInvariant();
                    break;
                case BundleType.Directory:
                    name = $"{assetUrl.Substring(0, assetUrl.LastIndexOf('/'))}{ABBuilder.BUNDLE_SUFFIX}".ToLowerInvariant();
                    break;
                case BundleType.File:
                    name = $"{assetUrl}{ABBuilder.BUNDLE_SUFFIX}".ToLowerInvariant();
                    break;
                default:
                    MLog.Print($"{typeof(BuildSetting)}：无法获取{assetUrl}的BundleName", MLogType.Error);
                    return null;
            }

            buildItem.Count += 1;

            return name;
        }

        private bool IsIgnore(List<string> ignoreList, string file)
        {
            for (int i = 0; i < ignoreList.Count; i++)
            {
                string ignorePath = ignoreList[i];
                if (string.IsNullOrEmpty(ignorePath)) continue;
                if (file.StartsWith(ignorePath, StringComparison.InvariantCulture)) return true;
            }

            return false;
        }

        /// <summary>
        /// 获取assetUrl归属BuildItem
        /// </summary>
        private BuildItem GetBuildItem(string assetUrl)
        {
            BuildItem item = null;
            for (int i = 0; i < items.Count; ++i)
            {
                BuildItem tempItem = items[i];
                if (assetUrl.StartsWith(tempItem.assetPath, StringComparison.InvariantCulture))
                {
                    //找到优先级最高(路径最长)的item
                    if (item == null || item.assetPath.Length < tempItem.assetPath.Length)
                    {
                        item = tempItem;
                    }
                }
            }

            return item;
        }
    }
}