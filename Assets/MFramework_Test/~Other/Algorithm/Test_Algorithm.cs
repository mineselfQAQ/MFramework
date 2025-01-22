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

    //在数组中找到出现次数大于数组长度一半(N/2)的数
    public int Q3(int[] arr)
    {
        //假设有int[10]，那么至少会有6个相同元素
        //可以让不同的元素进行抵消，这样进行1次后8个元素还是存在5个相同元素，依旧正确
        //到底是哪个元素，则为一直存活的那个
        //1 0 1 1 1 2
        //此时10抵消为1112，111相同进行累加，2再进行抵消一次，实际上剩下的就是11，进行一次验证即可
        //因为：0 1 2，此时01抵消，但不应该为2
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

    //问题4：调整数组顺序使奇数位于偶数前面
    public void Q4(int[] arr)
    {
        //做法1：
        //遍历数组，找到偶数则取到新数组中并前移其余元素，直到遍历完成
        //最后拼接原数组(奇数)和新数组(偶数)即可
        //时间复杂度O(n2)，空间复杂度O(n)
        //做法2：
        //创一个数组，遍历原数组，奇数放左面，偶数放右面，遍历完成后将数组复制回去即可
        //时间复杂度O(n)，空间复杂度O(n)
        //做法3：
        //哨兵算法，左偶和右奇对调
    }

    //问题5：求一个数组中的第k小/大的数
    public int Q5(int[] arr, int k)
    {
        //逆向遍历所有非叶子节点，进行自定向下的堆化操作
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

    //问题6：输出string，输出全排列，如：ABC输出ABC/ACB/BAC/BCA/CAB/CBA
    public string[] Q6(string S)
    {
        //思路：将第一个元素放在所有位置一次，再将第二个元素放在所有位置一次
        //如：ABC
        //可以有ABC BAC CBA，此时A存在与所有位置
        //然后对第二个元素进行同样操作，有ABC ACB | BAC BCA | CBA CAB
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
    //问题7：输出全组合，如：ABC输出A/B/C/AB/AC/BC/ABC
    public string[] Q7(string S)
    {
        //思路：对于每一个位置，只有2种情况：选或者不选
        //如ABC：对于A，我可以选也可以不选，
        //如果不选，则最多为BC(如果B也不要就只剩C了)，如果选，则为AXX(如果后面都不选，也就是A)
        //一直选到结尾就能得到相应字符串，由于每个递归的字符串都不一样，直接存即可
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

        //不选当前字符
        Q7DFS(res, S, current, i + 1);
        //选当前字符
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
