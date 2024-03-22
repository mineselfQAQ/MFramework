using System;
using System.Collections.Generic;
using UnityEngine;

public class Test_LOD : MonoBehaviour
{
    public class Student
    {
        public string Name { get; set; }

        public Student(string name)
        {
            Name = name;
        }
    }

    public class Classroom
    {
        private List<Student> students = new List<Student>();

        public void AddStudent(Student student)
        {
            students.Add(student);
        }

        public void ShowStudents()
        {
            foreach (var student in students)
            {
                Console.WriteLine(student.Name);
            }
        }

        public List<Student> GetStudents()
        {
            return students;
        }
    }

    public class School
    {
        private List<Classroom> classrooms = new List<Classroom>();

        public void AddClassroom(Classroom classroom)
        {
            classrooms.Add(classroom);
        }

        public void ShowStudents()
        {
            foreach (var classroom in classrooms)
            {
                // 学校类直接访问班级类的学生列表，违反了迪米特法则
                //foreach (var student in classroom.GetStudents())
                //{
                //    Console.WriteLine(student.Name);
                //}

                classroom.ShowStudents();
            }
        }
    }
}
