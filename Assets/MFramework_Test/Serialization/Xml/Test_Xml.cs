using MFramework;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class Test_Xml : MonoBehaviour
{
    private void Start()
    {
        CreateXml();
        CreateXml2();

        CheckXml();
    }

    private void CreateXml()
    {
        XmlDocument document = new XmlDocument();

        //����
        XmlDeclaration declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
        document.AppendChild(declaration);

        //���ڵ�(������)
        XmlElement root = document.CreateElement("Root");
        document.AppendChild(root);
        XmlAttribute attribute = document.CreateAttribute("Attribute");
        attribute.Value = "1";
        root.Attributes.Append(attribute);

        //ע��
        XmlComment comment = document.CreateComment("Child Group");
        root.AppendChild(comment);
        //�ӽڵ�
        XmlElement child1 = document.CreateElement("Child");
        XmlElement child2 = document.CreateElement("Child");
        XmlElement child3 = document.CreateElement("Child");
        child1.InnerText = "No.1";
        child2.InnerText = "No.2";
        child3.InnerText = "No.3";
        root.AppendChild(child1);
        root.AppendChild(child2);
        root.AppendChild(child3);
        //�ӽڵ�
        XmlElement child4 = document.CreateElement("Child");
        child4.InnerText = "No.4";
        root.InsertAfter(child4, child3);

        //document.Save("Example.xml");//����ڸ�Ŀ¼
        string path = $@"{Application.dataPath}\MFramework_Test\Serialization\Xml\Example.xml";
        if (File.Exists(path)) File.Delete(path);
        document.Save(path);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    private void CreateXml2()
    {
        XmlDocument document = new XmlDocument();

        //**��������**
        //ֻҪ�����XmlDeclaration�Ͳ�����֣���ʽ������ʽ������ʾ��
        //XmlWriterSettings settings = new XmlWriterSettings();
        //settings.OmitXmlDeclaration = true;//����XML����(�ڰ汾Ϊ1.0����ʹ��UTF-8ʱ����)

        //������People
        XmlElement people = document.CreateElement("People");
        document.AppendChild(people);

        //������Persons
        CreatePersonNode(people, "Joe", "20", "Male");
        CreatePersonNode(people, "Alice", "22", "Female");
        CreatePersonNode(people, "Jame", "19", "Male");

        //document.Save("Example2.xml");//����ڸ�Ŀ¼
        string path = $@"{Application.dataPath}\MFramework_Test\Serialization\Xml\Example2.xml";
        if (File.Exists(path)) File.Delete(path);
        document.Save(path);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        void CreatePersonNode(XmlElement root, string name, string age, string gender)
        {
            XmlElement person = document.CreateElement("Person");

            XmlAttribute nameAttribute = document.CreateAttribute("Name");
            XmlAttribute ageAttribute = document.CreateAttribute("Age");
            XmlAttribute genderAttribute = document.CreateAttribute("Gender");
            nameAttribute.Value = name;
            ageAttribute.Value = age;
            genderAttribute.Value = gender;
            person.Attributes.Append(nameAttribute);
            person.Attributes.Append(ageAttribute);
            person.Attributes.Append(genderAttribute);

            root.AppendChild(person);
        }
    }

    private void CheckXml()
    {
        string path = $@"{Application.dataPath}\MFramework_Test\Serialization\Xml\Example.xml";
        XmlDocument doc = new XmlDocument();
        doc.Load(path);

        XmlDeclaration declaration = (XmlDeclaration)doc.FirstChild;
        MLog.Print($"�汾��{declaration.Version} ���뷽ʽ��{declaration.Encoding}");

        XmlElement root = doc.DocumentElement;
        MLog.Print($"���ڵ��ǩ��{root.Name}");
        MLog.Print($"���ڵ����ԣ�{root.Attributes[0].Value}");

        MLog.Print($"�ӽڵ�1(ע��)��ֵ��{root.FirstChild.Value}");//Tip:CommentҲ��Child
        MLog.Print($"�ӽڵ�2(Child1)���ڲ�Xml��{root.FirstChild.NextSibling.InnerXml}");
        MLog.Print($"�ӽڵ�5(Child4)���ڲ�ֵ��{root.LastChild.InnerText}");

        MLog.Print($"���ڵ���ڲ�Xml:\n{root.InnerXml}");//���Կ���InnerXml�������ӽڵ�ı����ǲ�һ����
    }
}
