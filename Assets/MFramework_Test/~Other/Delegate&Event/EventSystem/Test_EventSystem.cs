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
            EventSystem.Dispatch(EventID.WINDY);
        }
    }

    public class Player
    {
        public Player()
        {
            EventSystem.AddListener(EventID.WINDY, () => { MLog.Print("Player: 刮风了"); });
        }
    }
    public class Bird
    {
        public Bird()
        {
            EventSystem.AddListener(EventID.WINDY, () => { Debug.Log("Bird: 刮风了"); });
        }
    }

    public sealed class EventID
    {
        public static readonly int WINDY = 1;
    }
}