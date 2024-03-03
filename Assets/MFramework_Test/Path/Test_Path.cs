using MFramework;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Test_Path : MonoBehaviour
{
    private void Start()
    {
        //==========Application类==========
        Log.Print($"Application.dataPath：{Application.dataPath}");
        Log.Print($"Application.streamingAssetsPath：{Application.streamingAssetsPath}");

        Log.Blank();



        //==========Environment类==========
        //这个类可以获取到系统的一些信息

        //项目根路径
        Log.Print($"Environment.CurrentDirectory：{Environment.CurrentDirectory}");

        Log.Blank();



        //==========Path类==========
        //根据下方的测试，可以发现Path类的一个重要特征：
        //Path类并没有任何的实际检测机制，它基本上只是一个**字符串操作算法类**
        //几个注意点：
        //1.对于Windows来说应该使用@"\"形式而非"/"(DOS中命令行参数/会导致冲突)
        //2.所有的路径自己要确保是正确的，Path类不会进行额外检测

        //获取完整路径(从项目根目录开始向前填充)
        Log.Print($"Path.GetFullPath：{Path.GetFullPath("Assets")}");//OK
        Log.Print($"Path.GetFullPath：{Path.GetFullPath("A/B/C")}");//虽然项目中不存在该路径，但是并不会判断，也是OK的

        Log.Blank();

        //获取相对路径(左---basePath 右---targetPath)
        //其实就是basePath怎么到targetPath
        Log.Print($"Path.GetRelativePath：{Path.GetRelativePath("Assets/A", "Assets/A/B/C")}");// B\C
        Log.Print($"Path.GetRelativePath：{Path.GetRelativePath("Assets/aaa", "Assets/A/B/C")}");// ..\A\B\C
        Log.Print($"Path.GetRelativePath：{Path.GetRelativePath("aaa/bbb", "Assets/A/B/C")}");// ..\..\Assets\A\B\C  特殊情况

        Log.Blank();

        //获取文件名(最后一个)
        Log.Print($"Path.GetFileName：{Path.GetFileName("Assets/A/B/C/Password.txt")}");
        Log.Print($"Path.GetFileName：{Path.GetFileName("Assets/A/B/C/Password")}");
        //获取文件夹名(最后一个，不带后缀)
        Log.Print($"Path.GetFileNameWithoutExtension：{Path.GetFileNameWithoutExtension("Assets/A/B/C/Password.txt")}");
        //获取后缀名(最后一个的后缀)
        Log.Print($"Path.GetExtension：{Path.GetExtension("Assets/A/B/C/Password.txt")}");

        Log.Blank();

        //获取文件夹名(排除最后一个)
        Log.Print($"Path.GetDirectoryName：{Path.GetDirectoryName("Assets/A/B/C/Password.txt")}");
        Log.Print($"Path.GetDirectoryName：{Path.GetDirectoryName("Assets/A/B/C")}");//即使不是文件是文件也是可以

        Log.Blank();

        //返回根目录     注意：盘符C:\和单独的\都是
        Log.Print($"Path.GetPathRoot：{Path.GetPathRoot(@"C:\MFramework\Assets\A\B\C\Password.txt")}");// C:\
        Log.Print($"Path.GetPathRoot：{Path.GetPathRoot(@"\MFramework\Assets\A\B\C\Password.txt")}");// \

        Log.Blank();

        //合并
        //Join---单纯的合并，不管其中的内容，唯一的处理就是Path之间没有\的话会填充一个
        //Combine---更完整的合并，同样在Path之间没有\的话会填充一个，除此以外还会排除不可用Path
        //注意：不会处理path中的斜杠(在Windows中/不会自动转换为\)
        Log.Print("合并：");
        //例1：标准合并，两者相同
        Log.Print($"1：{Path.Combine(@"C:\MFramework", @"Assets\A\B\C\Password.txt")}");
        Log.Print($"1：{Path.Join(@"C:\MFramework", @"Assets\A\B\C\Password.txt")}");
        //例2：path2存在盘符，是根目录，说明是完整路径
        //对于Combine，会舍弃path1
        //对于Join，会完整合并(path1和path2之间也会添加\)
        Log.Print($"2：{Path.Combine(@"MFramework", @"C:\MFramework\Assets\A\B\C\Password.txt")}");
        Log.Print($"2：{Path.Join(@"MFramework", @"C:\MFramework\Assets\A\B\C\Password.txt")}");
        //例3：头为\也代表根目录
        //对于Combine，会舍弃path1
        //对于Join，会完整合并(path1和path2之间也会添加\)
        Log.Print($"3：{Path.Combine(@"Assets\", @"\Assets\A\B\C\Password.txt")}");
        Log.Print($"3：{Path.Join(@"Assets\", @"\Assets\A\B\C\Password.txt")}");

        Log.Blank();



        //==========Directory类==========
        //这个类获取的是文件夹级别的内容
        Log.Print($"Directory.GetCurrentDirectory：{Directory.GetCurrentDirectory()}");
    }
}
