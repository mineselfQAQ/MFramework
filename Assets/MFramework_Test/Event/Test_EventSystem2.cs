using MFramework;
using UnityEngine;

public class Test_EventSystem2 : MonoBehaviour
{
    private void Awake()
    {
        EventSystem.AddListener(BuiltInEvent.AWAKE, (b) => { MLog.Print("AWAKE"); });
        EventSystem.AddListener(BuiltInEvent.START, (b) => { MLog.Print("START"); });
        EventSystem.AddListener(BuiltInEvent.UPDATE, () => { MLog.Print("UPDATE"); });
        EventSystem.AddListener(BuiltInEvent.FIXEDUPDATE, () => { MLog.Print("FIXEDUPDATE"); });
        EventSystem.AddListener(BuiltInEvent.LATEUPDATE, () => { MLog.Print("LATEUPDATE"); });
        EventSystem.AddListener(BuiltInEvent.ONAPPLICATIONFOCUS, (b) => { MLog.Print($"ONAPPLICATIONFOCUS focus:{b}"); });
        EventSystem.AddListener(BuiltInEvent.ONAPPLICATIONPAUSE, (b) => { MLog.Print($"ONAPPLICATIONPAUSE puase:{b}"); });
        EventSystem.AddListener(BuiltInEvent.ONAPPLICATIONQUIT, () => { MLog.Print("ONAPPLICATIONQUIT"); });
    }
}
