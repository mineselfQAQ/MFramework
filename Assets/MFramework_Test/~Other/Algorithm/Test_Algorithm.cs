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
        MLog.Print($"{MLog.Bold($"题目{Count++}")}：{res}");
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

    //问题：提取两个字符串的数字到一个数组中，并将他们排序好
    public int[] Q1(string a, string b)
    {
        int[] arr = new int[10];//计数排序

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

    //问题2(LeetCode200)：
    //给你一个由 '1'（陆地）和 '0'（水）组成的的二维网格，请你计算网格中岛屿的数量。
    //岛屿总是被水包围，并且每座岛屿只能由水平方向和/或竖直方向上相邻的陆地连接形成。
    //此外，你可以假设该网格的四条边均被水包围
    public int Q2(char[][] grid)
    {
        int count = 0;
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[0].Length; j++)
            {
                if (grid[i][j] == '1')
                {
                    //解1：BFS
                    //思路：
                    //全局遍历，只要发现1，就说明这肯定连着一块陆地，那么此时将所有相连陆地变为0，算作一块陆地，
                    //那么继续遍历，直到从头到尾都看过了
                    BFS2(grid, i, j);
                    //解2：DFS
                    //思路：
                    //同样必须全局遍历，只是发现1之后怎么搜索的区别
                    //DFS的话是通过递归不断把周围1扫描完毕的
                    DFS2(grid, i, j);
                    count++;
                }
            }
        }
        return count;
    }
    //解1：BFS
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
    //解2：DFS
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
