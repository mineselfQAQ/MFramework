using MFramework;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Test_MediatorPattern : MonoBehaviour
{
    private void Start()
    {
        IChatMediator mediator = new ChatMediator();

        User user1 = new ConcreteUser("Alice", mediator);
        User user2 = new ConcreteUser("Bob", mediator);
        User user3 = new ConcreteUser("Charlie", mediator);

        mediator.RegisterUser(user1);
        mediator.RegisterUser(user2);
        mediator.RegisterUser(user3);

        user1.SendMessage("��Һã�");
        user2.SendMessage("��ã�Alice��");
        user3.SendMessage("��ӭ���������ң�");
    }

    // �н��߽ӿ�
    public interface IChatMediator
    {
        void RegisterUser(User user); // ע���û�
        void SendMessage(string message, User sender); // ������Ϣ
    }

    // �����н���
    public class ChatMediator : IChatMediator
    {
        private List<User> _users = new List<User>();

        public void RegisterUser(User user)
        {
            if (!_users.Contains(user))
            {
                _users.Add(user);
                MLog.Print($"{user.Name} �����������ҡ�");
            }
        }

        public void SendMessage(string message, User sender)
        {
            foreach (var user in _users)
            {
                if (user != sender)
                {
                    user.ReceiveMessage(message, sender);
                }
            }
        }
    }

    // ͬ�³�����
    public abstract class User
    {
        protected IChatMediator _mediator;
        public string Name { get; private set; }

        public User(string name, IChatMediator mediator)
        {
            Name = name;
            _mediator = mediator;
        }

        public abstract void SendMessage(string message);
        public abstract void ReceiveMessage(string message, User sender);
    }

    // ����ͬ����
    public class ConcreteUser : User
    {
        public ConcreteUser(string name, IChatMediator mediator) : base(name, mediator) { }

        public override void SendMessage(string message)
        {
            MLog.Print($"{Name} ������Ϣ: {message}");
            _mediator.SendMessage(message, this);
        }

        public override void ReceiveMessage(string message, User sender)
        {
            MLog.Print($"{Name} �յ����� {sender.Name} ����Ϣ: {message}");
        }
    }
}
