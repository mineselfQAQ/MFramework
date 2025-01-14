using MFramework.DLC;
using UnityEngine;
using Edge = MFramework.DLC.MUndirGraphAdjMat.Edge;

public class Test_MUndirGraphAdjMat : MonoBehaviour
{
    private void Start()
    {
        //无权图(默认设置为1)  Tip：也可以有权，只是暂时是无权的
        MUndirGraphAdjMat unweightGraph = new MUndirGraphAdjMat(
            new int[] { 1, 2, 3, 4, 5 },
            new Edge[]
            {
                new Edge(0, 2),
                new Edge(0, 4),
                new Edge(1, 4),
                new Edge(2, 3),
                new Edge(3, 4)
            });
        unweightGraph.Print();

        //有权图(默认设置为1)
        MUndirGraphAdjMat weightGraph = new MUndirGraphAdjMat(
            new int[] { 1, 2, 3, 4, 5 },
            new Edge[]
            {
                new Edge(0, 2, 0.1f),
                new Edge(0, 4, 0.5f),
                new Edge(1, 4, 1.0f),
                new Edge(2, 3, 0.5f),
                new Edge(3, 4, 0.1f)
            });
        weightGraph.Print();
    }
}
