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

        user1.SendMessage("大家好！");
        user2.SendMessage("你好，Alice！");
        user3.SendMessage("欢迎加入聊天室！");
    }

    // 中介者接口
    public interface IChatMediator
    {
        void RegisterUser(User user); // 注册用户
        void SendMessage(string message, User sender); // 发送消息
    }

    // 具体中介者
    public class ChatMediator : IChatMediator
    {
        private List<User> _users = new List<User>();

        public void RegisterUser(User user)
        {
            if (!_users.Contains(user))
            {
                _users.Add(user);
                MLog.Print($"{user.Name} 加入了聊天室。");
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

    // 同事抽象类
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

    // 具体同事类
    public class ConcreteUser : User
    {
        public ConcreteUser(string name, IChatMediator mediator) : base(name, mediator) { }

        public override void SendMessage(string message)
        {
            MLog.Print($"{Name} 发送消息: {message}");
            _mediator.SendMessage(message, this);
        }

        public override void ReceiveMessage(string message, User sender)
        {
            MLog.Print($"{Name} 收到来自 {sender.Name} 的消息: {message}");
        }
    }
}
