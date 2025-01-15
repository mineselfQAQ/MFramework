using MFramework;
using UnityEngine;

public class Test_Lua : MonoBehaviour
{
    private void Start()
    {
        MLuaInterpreter.Instance.RequireLua("Test");
        MLuaInterpreter.Instance.RequireLua("Test2");
    }
}
