using MFramework;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class Test_Xml : MonoBehaviour
{
    private void Start()
    {
        CreateXml();

        CheckXml();
    }

    private void CreateXml()
    {
        XmlDocument document = new XmlDocument();

        //声明
        XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
        document.AppendChild(declaration);

        //根节点(带特性)
        XmlElement root = document.CreateElement("Root");
        document.AppendChild(root);
        XmlAttribute attribute = document.CreateAttribute("Attribute");
        attribute.Value = "1";
        root.Attributes.Append(attribute);

        //注释
        XmlComment comment = document.CreateComment("Child Group");
        root.AppendChild(comment);
        //子节点
        XmlElement child1 = document.CreateElement("Child");
        XmlElement child2 = document.CreateElement("Child");
        XmlElement child3 = document.CreateElement("Child");
        child1.InnerText = "No.1";
        child2.InnerText = "No.2";
        child3.InnerText = "No.3";
        root.AppendChild(child1);
        root.AppendChild(child2);
        root.AppendChild(child3);
        //子节点
        XmlElement child4 = document.CreateElement("Child");
        child4.InnerText = "No.4";
        root.InsertAfter(child4, child3);

        //document.Save("Example.xml");//存放在根目录
        string path = $@"{Application.dataPath}\MFramework_Test\Serialization\Xml\Example.xml";
        document.Save(path);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    private void CheckXml()
    {
        string path = $@"{Application.dataPath}\MFramework_Test\Serialization\Xml\Example.xml";
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlDeclaration declaration = (XmlDeclaration)doc.FirstChild;
        MLog.Print($"版本：{declaration.Version} 编码方式：{declaration.Encoding}");

        XmlElement root = doc.DocumentElement;
        MLog.Print($"根节点标签：{root.Name}");
        MLog.Print($"根节点特性：{root.Attributes[0].Value}");

        MLog.Print($"子节点1(注释)的值：{root.FirstChild.Value}");//Tip:Comment也算Child
        MLog.Print($"子节点2(Child1)的内部Xml：{root.FirstChild.NextSibling.InnerXml}");
        MLog.Print($"子节点5(Child4)的内部值：{root.LastChild.InnerText}");

        MLog.Print($"根节点的内部Xml:\n{root.InnerXml}");//可以看到InnerXml在有无子节点的表现是不一样的
    }
}
