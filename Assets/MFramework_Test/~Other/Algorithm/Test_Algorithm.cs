using MFramework;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Algorithm : MonoBehaviour
{
    public static int Count = 1;

    private void Start()
    {
        var res1 = Q1("132", "65551");

        Print(ToString(res1));
    }

    public void Print(object res)
    {
        MLog.Print($"{MLog.Bold($"��Ŀ{Count++}")}��{res}");
    }

    public string ToString<T>(T[] input)
    {
        string res = "";
        foreach (var i in input)
        {
            res += $"{i} ";
        }
        return res;
    }

    //���⣺��ȡ�����ַ��������ֵ�һ�������У��������������
    public int[] Q1(string a, string b)
    {
        int[] arr = new int[10];//��������

        string s = a + b;
        foreach (char c in s)
        {
            arr[c - '0']++;
        }

        int n = a.Length + b.Length;
        int[] res = new int[n];
        int count = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < arr[i]; j++)
            {
                res[count++] = i;
            }
        }

        return res;
    }

    //����2(LeetCode200)��
    //����һ���� '1'��½�أ��� '0'��ˮ����ɵĵĶ�ά����������������е����������
    //�������Ǳ�ˮ��Χ������ÿ������ֻ����ˮƽ�����/����ֱ���������ڵ�½�������γɡ�
    //���⣬����Լ��������������߾���ˮ��Χ
    public int Q2(char[][] grid)
    {
        int count = 0;
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[0].Length; j++)
            {
                if (grid[i][j] == '1')
                {
                    //��1��BFS
                    //˼·��
                    //ȫ�ֱ�����ֻҪ����1����˵����϶�����һ��½�أ���ô��ʱ����������½�ر�Ϊ0������һ��½�أ�
                    //��ô����������ֱ����ͷ��β��������
                    BFS2(grid, i, j);
                    //��2��DFS
                    //˼·��
                    //ͬ������ȫ�ֱ�����ֻ�Ƿ���1֮����ô����������
                    //DFS�Ļ���ͨ���ݹ鲻�ϰ���Χ1ɨ����ϵ�
                    DFS2(grid, i, j);
                    count++;
                }
            }
        }
        return count;
    }
    //��1��BFS
    private void BFS2(char[][] grid, int i, int j)
    {
        Queue<int[]> queue = new Queue<int[]>();
        queue.Enqueue(new int[] { i, j });

        while (queue.Count != 0)
        {
            int[] pos = queue.Dequeue();
            i = pos[0]; j = pos[1];
            if (i >= 0 && j >= 0 && i < grid.Length && j < grid[0].Length &&
                grid[i][j] == '1')
            {
                grid[i][j] = '0';
                queue.Enqueue(new int[] { i + 1, j });
                queue.Enqueue(new int[] { i - 1, j });
                queue.Enqueue(new int[] { i, j + 1 });
                queue.Enqueue(new int[] { i, j - 1 });
            }
        }
    }
    //��2��DFS
    private void DFS2(char[][] grid, int i, int j)
    {
        if (i < 0 || j < 0 || i >= grid.Length || j >= grid[0].Length ||
                grid[i][j] == '0') return;

        grid[i][j] = '0';
        DFS2(grid, i + 1, j);
        DFS2(grid, i - 1, j);
        DFS2(grid, i, j + 1);
        DFS2(grid, i, j - 1);
    }
}
