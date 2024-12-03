using MFramework;
using System;
using UnityEngine;

public class Test_StatePattern : MonoBehaviour
{
    private void Start()
    {
        Character character = new Character();

        character.Update();  // ��ʼ״̬��վ��

        character.HandleInput("run");
        character.Update();  // �л�������״̬

        character.HandleInput("jump");
        character.Update();  // �л�����Ծ״̬

        character.HandleInput("land");
        character.Update();  // �л���վ��״̬
    }

    // ����״̬�ӿ�
    public interface ICharacterState
    {
        void HandleInput(Character context, string input);
        void Update(Character context);
    }

    // ����״̬�� - վ��״̬
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
            Console.WriteLine("��ɫ����վ��...");
        }
    }

    // ����״̬�� - ����״̬
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
            Console.WriteLine("��ɫ���ڱ���...");
        }
    }

    // ����״̬�� - ��Ծ״̬
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
            Console.WriteLine("��ɫ������Ծ...");
        }
    }

    // ��������
    public class Character
    {
        private ICharacterState _currentState;

        public Character()
        {
            // ��ʼ��Ϊվ��״̬
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
