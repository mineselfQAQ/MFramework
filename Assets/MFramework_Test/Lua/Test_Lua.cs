using MFramework;
using UnityEngine;
using XLua;

public class Test_Lua : MonoBehaviour
{
    private void Start()
    {
        MLuaInterpreter.Instance.RequireLua("Test");
        MLuaInterpreter.Instance.RequireLua("Test2");
    }
}

[LuaCallCSharp]
public class MyClass
{
    public string Name { get; set; }

    public void Greet()
    {
        Debug.Log("Hello from C#!");
    }
}