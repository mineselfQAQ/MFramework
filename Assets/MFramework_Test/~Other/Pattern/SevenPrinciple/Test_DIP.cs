using MFramework;
using UnityEngine;

public class Test_DIP : MonoBehaviour
{
    //private void Start()
    //{
    //    Notification notification = new Notification(new EmailService());
    //    notification.SendNotification("OK");
    //}

    ////µ×²ã
    //public class EmailService
    //{
    //    public void SendMessage(string message)
    //    {
    //        MLog.Print($"Message: {message}");
    //    }
    //}

    ////¸ß²ã
    //public class Notification
    //{
    //    private EmailService service;

    //    public Notification(EmailService service)
    //    {
    //        this.service = service;
    //    }

    //    public void SendNotification(string message)
    //    {
    //        service.SendMessage(message);
    //    }
    //}

    private void Start()
    {
        Notification notification = new Notification(new EmailService());
        notification.SendNotification("OK");
    }

    public interface IMessageService
    {
        void SendMessage(string message);
    }

    //µ×²ã
    public class EmailService : IMessageService
    {
        public void SendMessage(string message)
        {
            MLog.Print($"Message: {message}");
        }
    }

    //¸ß²ã
    public class Notification
    {
        private IMessageService service;

        public Notification(IMessageService service)
        {
            this.service = service;
        }

        public void SendNotification(string message)
        {
            service.SendMessage(message);
        }
    }
}
