using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Test_FilterPattern : MonoBehaviour
{
    private void Start()
    {
        Person p1 = new Person("A", "Male");
        Person p2 = new Person("B", "Male");
        Person p3 = new Person("C", "Female");
        List<Person> persons = new List<Person>() { p1, p2, p3 };

        CriteriaMale criteriaMale = new CriteriaMale();
        var res1 = criteriaMale.meetCriteria(persons);
        foreach (var person in res1)
        {
            MLog.Print($"Name: {person.GetName()}, Gender: {person.GetGender()}");
        }
    }

    public interface Criteria
    {
        public List<Person> meetCriteria(List<Person> persons);
    }

    public class CriteriaMale : Criteria
    {
        public List<Person> meetCriteria(List<Person> persons)
        {
            List<Person> malePersons = new List<Person>();
            foreach (var person in persons)
            {
                if (person.GetGender().ToLower() == "male")
                {
                    malePersons.Add(person);
                }
            }
            return malePersons;
        }
    }
    public class CriteriaFemale : Criteria
    {
        public List<Person> meetCriteria(List<Person> persons)
        {
            List<Person> femalePersons = new List<Person>();
            foreach (var person in persons)
            {
                if (person.GetGender().ToLower() == "female")
                {
                    femalePersons.Add(person);
                }
            }
            return femalePersons;
        }
    }

    public class Person
    {
        private string name;
        private string gender;

        public Person(string name, string gender)
        {
            this.name = name;
            this.gender = gender;
        }

        public string GetName()
        {
            return name;
        }
        public string GetGender()
        {
            return gender;
        }
    }
}
