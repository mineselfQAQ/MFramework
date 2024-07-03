using MFramework;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Test_Path : MonoBehaviour
{
    private void Start()
    {
        //==========Application��==========
        MLog.Print($"Application.dataPath��{Application.dataPath}");
        MLog.Print($"Application.streamingAssetsPath��{Application.streamingAssetsPath}");

        MLog.Blank();



        //==========Environment��==========
        //�������Ի�ȡ��ϵͳ��һЩ��Ϣ

        //��Ŀ��·��
        MLog.Print($"Environment.CurrentDirectory��{Environment.CurrentDirectory}");

        MLog.Blank();



        //==========Path��==========
        //�����·��Ĳ��ԣ����Է���Path���һ����Ҫ������
        //Path�ಢû���κε�ʵ�ʼ����ƣ���������ֻ��һ��**�ַ��������㷨��**
        //����ע��㣺
        //1.����Windows��˵Ӧ��ʹ��@"\"��ʽ����"/"(DOS�������в���/�ᵼ�³�ͻ)
        //2.���е�·���Լ�Ҫȷ������ȷ�ģ�Path�಻����ж�����

        //��ȡ����·��(����Ŀ��Ŀ¼��ʼ��ǰ���)
        MLog.Print($"Path.GetFullPath��{Path.GetFullPath("Assets")}");//OK
        MLog.Print($"Path.GetFullPath��{Path.GetFullPath("A/B/C")}");//��Ȼ��Ŀ�в����ڸ�·�������ǲ������жϣ�Ҳ��OK��

        MLog.Blank();

        //��ȡ���·��(��---basePath ��---targetPath)
        //��ʵ����basePath��ô��targetPath
        MLog.Print($"Path.GetRelativePath��{Path.GetRelativePath("Assets/A", "Assets/A/B/C")}");// B\C
        MLog.Print($"Path.GetRelativePath��{Path.GetRelativePath("Assets/aaa", "Assets/A/B/C")}");// ..\A\B\C
        MLog.Print($"Path.GetRelativePath��{Path.GetRelativePath("aaa/bbb", "Assets/A/B/C")}");// ..\..\Assets\A\B\C  �������

        MLog.Blank();

        //��ȡ�ļ���(���һ��)
        MLog.Print($"Path.GetFileName��{Path.GetFileName("Assets/A/B/C/Password.txt")}");
        MLog.Print($"Path.GetFileName��{Path.GetFileName("Assets/A/B/C/Password")}");
        //��ȡ�ļ�����(���һ����������׺)
        MLog.Print($"Path.GetFileNameWithoutExtension��{Path.GetFileNameWithoutExtension("Assets/A/B/C/Password.txt")}");
        //��ȡ��׺��(���һ���ĺ�׺)
        MLog.Print($"Path.GetExtension��{Path.GetExtension("Assets/A/B/C/Password.txt")}");

        MLog.Blank();

        //��ȡ�ļ�����(�ų����һ��)
        MLog.Print($"Path.GetDirectoryName��{Path.GetDirectoryName("Assets/A/B/C/Password.txt")}");
        MLog.Print($"Path.GetDirectoryName��{Path.GetDirectoryName("Assets/A/B/C")}");//��ʹ�����ļ����ļ�Ҳ�ǿ���

        MLog.Blank();

        //���ظ�Ŀ¼     ע�⣺�̷�C:\�͵�����\����
        MLog.Print($"Path.GetPathRoot��{Path.GetPathRoot(@"C:\MFramework\Assets\A\B\C\Password.txt")}");// C:\
        MLog.Print($"Path.GetPathRoot��{Path.GetPathRoot(@"\MFramework\Assets\A\B\C\Password.txt")}");// \

        MLog.Blank();

        //�ϲ�
        //Join---�����ĺϲ����������е����ݣ�Ψһ�Ĵ������Path֮��û��\�Ļ������һ��
        //Combine---�������ĺϲ���ͬ����Path֮��û��\�Ļ������һ�����������⻹���ų�������Path
        //ע�⣺���ᴦ��path�е�б��(��Windows��/�����Զ�ת��Ϊ\)
        MLog.Print("�ϲ���");
        //��1����׼�ϲ���������ͬ
        MLog.Print($"1��{Path.Combine(@"C:\MFramework", @"Assets\A\B\C\Password.txt")}");
        MLog.Print($"1��{Path.Join(@"C:\MFramework", @"Assets\A\B\C\Password.txt")}");
        //��2��path2�����̷����Ǹ�Ŀ¼��˵��������·��
        //����Combine��������path1
        //����Join���������ϲ�(path1��path2֮��Ҳ�����\)
        MLog.Print($"2��{Path.Combine(@"MFramework", @"C:\MFramework\Assets\A\B\C\Password.txt")}");
        MLog.Print($"2��{Path.Join(@"MFramework", @"C:\MFramework\Assets\A\B\C\Password.txt")}");
        //��3��ͷΪ\Ҳ�����Ŀ¼
        //����Combine��������path1
        //����Join���������ϲ�(path1��path2֮��Ҳ�����\)
        MLog.Print($"3��{Path.Combine(@"Assets\", @"\Assets\A\B\C\Password.txt")}");
        MLog.Print($"3��{Path.Join(@"Assets\", @"\Assets\A\B\C\Password.txt")}");

        MLog.Blank();



        //==========Directory��==========
        //������ȡ�����ļ��м��������
        MLog.Print($"Directory.GetCurrentDirectory��{Directory.GetCurrentDirectory()}");
    }
}
