using MFramework;
using UnityEngine;

public class Test_MediatorPattern : MonoBehaviour
{
    private void Start()
    {
        User user1 = new User("Robert");
        User user2 = new User("John");

        user1.SendMessage("Hi, John");
        user2.SendMessage("Hi, Robert");
    }

    public class ChatRoom
    {
        public static void ShowMessage(User user, string message)
        {
            MLog.Print($"{user}: {message}");
        }
    }

    public class User
    {
        private string name;

        public User(string name)
        {
            this.name = name;
        }

        public string GetName() => name;

        public void SendMessage(string message)
        {
            ChatRoom.ShowMessage(this, message);
        }
    }
}
