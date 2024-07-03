using MFramework;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class Test_XmlObject : MonoBehaviour
{
    private void Start()
    {
        Person p1 = new Person();
        p1.Name = "Joe"; p1.Age = "20"; p1.Gender = "Male";
        Person p2 = new Person();
        p2.Name = "Alice"; p2.Age = "22"; p2.Gender = "Female";
        Person p3 = new Person();
        p3.Name = "Jame"; p3.Age = "19"; p3.Gender = "Male";

        People p = new People();
        p.people.Add(p1);
        p.people.Add(p2);
        p.people.Add(p3);

        //两种形式：
        //1.绝对路径
        //2.基于根路径---在编辑器与发布版本中都为"项目名/XmlSettings/<filePath>"
        //MSerializationUtility.SaveToXml($@"C:\Users\Administrator\Desktop\ABC\Test", p, true);
        MSerializationUtility.SaveToXml($@"XMLTest\People", p, true);

        People outP = MSerializationUtility.ReadFromXml<People>($@"XMLTest\People");
        MLog.Print($"{outP.people[0].Name} {outP.people[0].Age} {outP.people[0].Gender}");
        MLog.Print($"{outP.people[1].Name} {outP.people[1].Age} {outP.people[1].Gender}");
        MLog.Print($"{outP.people[2].Name} {outP.people[2].Age} {outP.people[2].Gender}");

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public class People
    {
        [XmlElement("Person")]
        public List<Person> people { get; set; } = new List<Person>();
    }

    public class Person
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Age")]
        public string Age { get; set; }

        [XmlAttribute("Gender")]
        public string Gender { get; set; }
    }
}