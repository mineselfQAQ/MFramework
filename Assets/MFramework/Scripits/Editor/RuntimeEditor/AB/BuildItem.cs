using System.Collections.Generic;
using System.Xml.Serialization;

namespace MFramework
{
    public class BuildItem
    {
        [XmlAttribute("AssetPath")]
        public string assetPath { get; set; }//资源路径

        [XmlAttribute("ResourceType")]
        public ResourceType resourceType { get; set; } = ResourceType.Direct;//资源类型

        [XmlAttribute("BundleType")]
        public BundleType bundleType { get; set; } = BundleType.File;//AB打包级别

        [XmlAttribute("Suffix")]
        public string suffix { get; set; } = ".prefab";//资源后缀



        [XmlIgnore]
        public List<string> ignorePaths { get; set; } = new List<string>();//忽略路径

        [XmlIgnore]
        public List<string> suffixes { get; set; } = new List<string>();//资源后缀(拆分处理后)

        [XmlIgnore]
        public int Count { get; set; }//匹配的资源个数
    }
}