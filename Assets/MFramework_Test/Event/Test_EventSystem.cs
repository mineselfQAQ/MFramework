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
            MLog.Print($"�����¼���id-{EventID.WINDY}");
            MEventSystem.Dispatch(EventID.WINDY);
        }
    }

    public class Player
    {
        public Player()
        {
            MEventSystem.AddListener(EventID.WINDY, () => { MLog.Print("Player: �η���"); });
        }
    }
    public class Bird
    {
        public Bird()
        {
            MEventSystem.AddListener(EventID.WINDY, () => { MLog.Print("Bird: �η���"); });
        }
    }

    public sealed class EventID
    {
        public static readonly int WINDY = 1;
    }
}