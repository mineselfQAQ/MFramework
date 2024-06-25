using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

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
        public Dictionary<string, BuildItem> itemDic = new Dictionary<string, BuildItem>();

        public void Init()
        {
            buildRoot = Path.GetFullPath(buildRoot).Replace("\\", "/");

            itemDic.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                BuildItem buildItem = items[i];

                if (buildItem.bundleType == EBundleType.All || buildItem.bundleType == EBundleType.Directory)
                {
                    if (!Directory.Exists(buildItem.assetPath))
                    {
                        throw new Exception($"不存在资源路径:{buildItem.assetPath}");
                    }
                }

                //处理后缀
                string[] prefixes = buildItem.suffix.Split('|');
                for (int ii = 0; ii < prefixes.Length; ii++)
                {
                    string prefix = prefixes[ii].Trim();
                    if (!string.IsNullOrEmpty(prefix))
                        buildItem.suffixes.Add(prefix);
                }

                if (itemDic.ContainsKey(buildItem.assetPath))
                {
                    throw new Exception($"重复的资源路径:{buildItem.assetPath}");
                }
                itemDic.Add(buildItem.assetPath, buildItem);
            }
        }
    }
}