using UnityEngine;
using MFramework;

public class Test_SingletonPattern : MonoBehaviour
{
    private void Start()
    {
        Sgt.Instance.Print();
    }
}

public class Sgt : Singleton<Sgt>
{
    private Sgt()
    {

    }

    public void Print()
    {
        MLog.Print("Sgt Print");
    }
}