using System.Collections.Generic;
using UnityEngine;
using MFramework;

public class Test_CompositePattern : MonoBehaviour
{
    private void Start()
    {
        File file1 = new File("file1.txt");
        File file2 = new File("file2.txt");
        File file3 = new File("file3.txt");

        Folder folder1 = new Folder("Folder 1");
        folder1.Add(file1);
        folder1.Add(file2);

        Folder folder2 = new Folder("Folder 2");
        folder2.Add(file3);

        Folder rootFolder = new Folder("Root Folder");
        rootFolder.Add(folder1);
        rootFolder.Add(folder2);

        rootFolder.Print(0);
    }

    public interface IFileSystemComponent
    {
        void Print(int depth);
    }

    //叶子节点：文件类
    public class File : IFileSystemComponent
    {
        private string name;

        public File(string name)
        {
            this.name = name;
        }

        public void Print(int depth)
        {
            MLog.Print(new string('-', depth) + " " + name);
        }
    }

    //组合节点：文件夹类
    public class Folder : IFileSystemComponent
    {
        private string name;
        private List<IFileSystemComponent> children = new List<IFileSystemComponent>();

        public Folder(string name)
        {
            this.name = name;
        }

        public void Add(IFileSystemComponent component)
        {
            children.Add(component);
        }

        public void Remove(IFileSystemComponent component)
        {
            children.Remove(component);
        }

        public void Print(int depth)
        {
            MLog.Print(new string('-', depth) + " " + name);
            foreach (var child in children)
            {
                child.Print(depth + 2);
            }
        }
    }
}
