using UnityEngine;
using MFramework;

public class Test_EventSystem : MonoBehaviour
{
    private void Start()
    {
        Player p = new Player();
        Bird b = new Bird();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            MLog.Print($"发生事件：id-{EventID.WINDY}");
            MEventSystem.Dispatch(EventID.WINDY);
        }
    }

    public class Player
    {
        public Player()
        {
            MEventSystem.AddListener(EventID.WINDY, () => { MLog.Print("Player: 刮风了"); });
        }
    }
    public class Bird
    {
        public Bird()
        {
            MEventSystem.AddListener(EventID.WINDY, () => { MLog.Print("Bird: 刮风了"); });
        }
    }

    public sealed class EventID
    {
        public static readonly int WINDY = 1;
    }
}