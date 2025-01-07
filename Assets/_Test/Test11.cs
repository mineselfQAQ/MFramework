using UnityEngine;
using UnityEngine.UI;

public delegate int Fun1(int a);

public class Test11 : MonoBehaviour
{
    void Start()
    {
        Fun1 fun1 = (a) => a;
        fun1.Invoke(1);
    }

    void Update()
    {

    }
}