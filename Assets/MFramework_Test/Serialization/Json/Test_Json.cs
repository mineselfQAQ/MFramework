using MFramework;
using System.IO;
using UnityEngine;

public class Test_Json : MonoBehaviour
{
    private void Start()
    {
        //===Save===
        JTest jt1 = new JTest(1, 1.1f, "1");
        JTest jt2 = new JTest(2, 3.4f, "5");

        MSerializationUtility.SaveToJson(@"Jtest\jt1", jt1);
        MSerializationUtility.SaveToJson(@"Jtest\jt2", jt2, true);

        //===Get===
        JTest jt1Ret = MSerializationUtility.ReadFromJson<JTest>(@"Jtest\jt1");
        MLog.Print(jt1Ret.i + " " + jt1Ret.f + " " + jt1Ret.s);
        JTest jt2Ret = MSerializationUtility.ReadFromJson<JTest>(@"Jtest\jt2");
        MLog.Print(jt2Ret.i + " " + jt2Ret.f + " " + jt2Ret.s);
    }
}

public class JTest
{
    public int i;
    public float f;
    public string s;

    public JTest(int i, float f, string s)
    {
        this.i = i;
        this.f = f;
        this.s = s;
    }
}