using OfficeOpenXml.Table.PivotTable;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MFramework
{
    public class BuildItem
    {
        [XmlAttribute("AssetPath")]
        public string assetPath { get; set; }//资源路径

        [XmlAttribute("ResourceType")]
        public EResourceType resourceType { get; set; } = EResourceType.Direct;//资源类型

        [XmlAttribute("BundleType")]
        public EBundleType bundleType { get; set; } = EBundleType.File;//AB粒度类型

        [XmlAttribute("Suffix")]
        public string suffix { get; set; } = ".prefab";//资源后缀

        [XmlIgnore]
        public List<string> ignorePaths { get; set; } = new List<string>();

        [XmlIgnore]
        public List<string> suffixes { get; set; } = new List<string>();

        //匹配该打包设置的个数
        [XmlIgnore]
        public int Count { get; set; }
    }
}