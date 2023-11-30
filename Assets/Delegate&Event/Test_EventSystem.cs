using UnityEngine;
using MFramework;

public class Test_EventSystem : MonoBehaviour
{
    private void Start()
    {
        Test_EventSystem_Player p = new Test_EventSystem_Player();
        Test_EventSystem_Bird b = new Test_EventSystem_Bird();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EventSystem.Dispatch(Test_EventSystem_EventID.WINDY);
        }
    }
}

public class Test_EventSystem_Player
{
    public Test_EventSystem_Player()
    {
        EventSystem.AddListener(Test_EventSystem_EventID.WINDY, () => { Debug.Log("Player: wtf"); });
    }
}

public class Test_EventSystem_Bird
{
    public Test_EventSystem_Bird()
    {
        EventSystem.AddListener(Test_EventSystem_EventID.WINDY, () => { Debug.Log("Bird: wtf"); });
    }
}

public sealed class Test_EventSystem_EventID
{
    public static readonly int WINDY = 1;
}
