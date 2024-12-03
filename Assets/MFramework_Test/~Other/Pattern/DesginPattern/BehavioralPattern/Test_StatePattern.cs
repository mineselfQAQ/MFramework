using MFramework;
using System;
using UnityEngine;

public class Test_StatePattern : MonoBehaviour
{
    private void Start()
    {
        Character character = new Character();

        character.Update();  // 初始状态：站立

        character.HandleInput("run");
        character.Update();  // 切换到奔跑状态

        character.HandleInput("jump");
        character.Update();  // 切换到跳跃状态

        character.HandleInput("land");
        character.Update();  // 切换回站立状态
    }

    // 抽象状态接口
    public interface ICharacterState
    {
        void HandleInput(Character context, string input);
        void Update(Character context);
    }

    // 具体状态类 - 站立状态
    public class StandingState : ICharacterState
    {
        public void HandleInput(Character context, string input)
        {
            if (input == "run")
            {
                context.ChangeState(new RunningState());
            }
            else if (input == "jump")
            {
                context.ChangeState(new JumpingState());
            }
        }

        public void Update(Character context)
        {
            Console.WriteLine("角色正在站立...");
        }
    }

    // 具体状态类 - 奔跑状态
    public class RunningState : ICharacterState
    {
        public void HandleInput(Character context, string input)
        {
            if (input == "stop")
            {
                context.ChangeState(new StandingState());
            }
            else if (input == "jump")
            {
                context.ChangeState(new JumpingState());
            }
        }

        public void Update(Character context)
        {
            Console.WriteLine("角色正在奔跑...");
        }
    }

    // 具体状态类 - 跳跃状态
    public class JumpingState : ICharacterState
    {
        public void HandleInput(Character context, string input)
        {
            if (input == "land")
            {
                context.ChangeState(new StandingState());
            }
        }

        public void Update(Character context)
        {
            Console.WriteLine("角色正在跳跃...");
        }
    }

    // 上下文类
    public class Character
    {
        private ICharacterState _currentState;

        public Character()
        {
            // 初始化为站立状态
            _currentState = new StandingState();
        }

        public void ChangeState(ICharacterState newState)
        {
            _currentState = newState;
        }

        public void HandleInput(string input)
        {
            _currentState.HandleInput(this, input);
        }

        public void Update()
        {
            _currentState.Update(this);
        }
    }
}
