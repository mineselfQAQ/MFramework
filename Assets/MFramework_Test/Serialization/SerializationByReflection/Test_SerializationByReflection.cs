using MFramework;
using Newtonsoft.Json;
using System;
using System.Reflection;
using UnityEngine;

public class Test_SerializationByReflection : MonoBehaviour
{
    private void Start()
    {
        Person person = new Person("Alice", 30);

        string json = SerializationHelper.SerializeObject(person);
        MLog.Print(json);

        Person deserializedPerson = SerializationHelper.DeserializeObject<Person>(json);
        MLog.Print($"Name:{deserializedPerson.Name} Age:{deserializedPerson.Age}");
    }

    public class SerializationHelper
    {
        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeserializeObject<T>(string json)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            T obj = System.Activator.CreateInstance<T>();//创建实例

            object instance = JsonConvert.DeserializeObject(json, type);
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    //将所有属性赋值(进行还原)
                    object value = instance.GetType().GetProperty(property.Name).GetValue(instance);
                    property.SetValue(obj, value);
                }
            }

            return obj;
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person() { }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }
}

