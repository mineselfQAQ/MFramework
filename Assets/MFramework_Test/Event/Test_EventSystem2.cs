using MFramework;
using UnityEngine;

public class Test_EventSystem2 : MonoBehaviour
{
    private void Awake()
    {
        MEventSystem.AddListener(BuiltInEvent.AWAKE, (b) => { MLog.Print("AWAKE"); });
        MEventSystem.AddListener(BuiltInEvent.START, (b) => { MLog.Print("START"); });
        MEventSystem.AddListener(BuiltInEvent.UPDATE, () => { MLog.Print("UPDATE"); });
        MEventSystem.AddListener(BuiltInEvent.FIXEDUPDATE, () => { MLog.Print("FIXEDUPDATE"); });
        MEventSystem.AddListener(BuiltInEvent.LATEUPDATE, () => { MLog.Print("LATEUPDATE"); });
        MEventSystem.AddListener(BuiltInEvent.ONAPPLICATIONFOCUS, (b) => { MLog.Print($"ONAPPLICATIONFOCUS focus:{b}"); });
        MEventSystem.AddListener(BuiltInEvent.ONAPPLICATIONPAUSE, (b) => { MLog.Print($"ONAPPLICATIONPAUSE puase:{b}"); });
        MEventSystem.AddListener(BuiltInEvent.ONAPPLICATIONQUIT, () => { MLog.Print("ONAPPLICATIONQUIT"); });
    }
}
