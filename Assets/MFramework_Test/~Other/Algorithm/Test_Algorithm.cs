using MFramework;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Algorithm : MonoBehaviour
{
    public static int Count = 1;

    private void Start()
    {
        //var res = Q1("132", "65551");
        //var res = Q3(new int[] { 1, 0, 1, 1, 1, 2 });
        //var res = Q5(new int[] { 0, 5, 1, 2, 7, 3, 4, 6, 9, 8 }, 3);
        var res = Q7("ABC");
        foreach (var s in res)
        {
            Debug.Log(s);
        }

        //Print(res);
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

    //���������ҵ����ִ����������鳤��һ��(N/2)����
    public int Q3(int[] arr)
    {
        //������int[10]����ô���ٻ���6����ͬԪ��
        //�����ò�ͬ��Ԫ�ؽ��е�������������1�κ�8��Ԫ�ػ��Ǵ���5����ͬԪ�أ�������ȷ
        //�������ĸ�Ԫ�أ���Ϊһֱ�����Ǹ�
        //1 0 1 1 1 2
        //��ʱ10����Ϊ1112��111��ͬ�����ۼӣ�2�ٽ��е���һ�Σ�ʵ����ʣ�µľ���11������һ����֤����
        //��Ϊ��0 1 2����ʱ01����������Ӧ��Ϊ2
        int count = 0, cand = -1;
        for (int i = 0; i < arr.Length; i++)
        {
            if (count == 0)
            {
                cand = arr[i];
                count++;
            }
            else if (cand == arr[i])
            {
                count++;
            }
            else
            {
                count--;
            }
        }

        count = 0;
        foreach (var i in arr)
        {
            if (i == cand) count++;
        }
        if (count > arr.Length / 2) return cand;
        return -1;
    }

    //����4����������˳��ʹ����λ��ż��ǰ��
    public void Q4(int[] arr)
    {
        //����1��
        //�������飬�ҵ�ż����ȡ���������в�ǰ������Ԫ�أ�ֱ���������
        //���ƴ��ԭ����(����)��������(ż��)����
        //ʱ�临�Ӷ�O(n2)���ռ临�Ӷ�O(n)
        //����2��
        //��һ�����飬����ԭ���飬���������棬ż�������棬������ɺ����鸴�ƻ�ȥ����
        //ʱ�临�Ӷ�O(n)���ռ临�Ӷ�O(n)
        //����3��
        //�ڱ��㷨����ż������Ե�
    }

    //����5����һ�������еĵ�kС/�����
    public int Q5(int[] arr, int k)
    {
        //����������з�Ҷ�ӽڵ㣬�����Զ����µĶѻ�����
        for (int i = (arr.Length - 1) / 2; i >= 0; i--)
        {
            SiftDown(arr, arr.Length, i);
        }

        for (int i = arr.Length - 1; i > arr.Length - k; i--)
        {
            Swap(arr, 0, i);
            SiftDown(arr, i, 0);
        }
        return arr[0];
    }
    private void SiftDown(int[] arr, int n, int i)
    {
        while(true)
        {
            int l = i * 2 + 1;
            int r = i * 2 + 2;

            int max = i;
            if (l < n && arr[l] > arr[max])
            {
                max = l;
            }
            if (r < n && arr[r] > arr[max])
            {
                max = r;
            }

            if (max == i) break;

            Swap(arr, i, max);
            i = max;
        }
    }

    //����6�����string�����ȫ���У��磺ABC���ABC/ACB/BAC/BCA/CAB/CBA
    public string[] Q6(string S)
    {
        //˼·������һ��Ԫ�ط�������λ��һ�Σ��ٽ��ڶ���Ԫ�ط�������λ��һ��
        //�磺ABC
        //������ABC BAC CBA����ʱA����������λ��
        //Ȼ��Եڶ���Ԫ�ؽ���ͬ����������ABC ACB | BAC BCA | CBA CAB
        List<string> res = new List<string>();
        Q6DFS(res, S, 0);
        return res.ToArray();
    }
    private void Q6DFS(List<string> res, string S, int i)
    {
        if (i == S.Length) res.Add(S);

        for (int j = i; j < S.Length; j++)
        {
            S = Swap(S, i, j);
            Q6DFS(res, S, i + 1);
            S = Swap(S, i, j);
        }
    }
    //����7�����ȫ��ϣ��磺ABC���A/B/C/AB/AC/BC/ABC
    public string[] Q7(string S)
    {
        //˼·������ÿһ��λ�ã�ֻ��2�������ѡ���߲�ѡ
        //��ABC������A���ҿ���ѡҲ���Բ�ѡ��
        //�����ѡ�������ΪBC(���BҲ��Ҫ��ֻʣC��)�����ѡ����ΪAXX(������涼��ѡ��Ҳ����A)
        //һֱѡ����β���ܵõ���Ӧ�ַ���������ÿ���ݹ���ַ�������һ����ֱ�Ӵ漴��
        List<string> res = new List<string>();
        Q7DFS(res, S, "", 0);
        return res.ToArray();
    }
    private void Q7DFS(List<string> res, string S, string current, int i)
    {
        if (i == S.Length)
        {
            res.Add(current);
            return;
        }

        //��ѡ��ǰ�ַ�
        Q7DFS(res, S, current, i + 1);
        //ѡ��ǰ�ַ�
        Q7DFS(res, S, current + S[i], i + 1);
    }


    private string Swap(string str, int a, int b)
    {
        char[] charArray = str.ToCharArray();
        char temp = charArray[a];
        charArray[a] = charArray[b];
        charArray[b] = temp;

        return new string(charArray);
    }
    private void Swap(int[] arr, int a, int b)
    {
        int temp = arr[a];
        arr[a] = arr[b];
        arr[b] = temp;
    }
}
