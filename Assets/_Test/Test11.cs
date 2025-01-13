using MFramework;
using UnityEngine;

public class Test11 : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            LuaInterpreter.Instance.RequireLua("CreatePrimitive");
        }
    }
}