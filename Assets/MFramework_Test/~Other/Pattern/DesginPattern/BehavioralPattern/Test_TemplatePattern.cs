using MFramework;
using UnityEngine;

public class Test_TemplatePattern : MonoBehaviour
{
    private void Start()
    {
        Football football = new Football();
        football.Play();
    }

    public class Football : Game
    {
        protected override void Init()
        {
            MLog.Print("Football: Init");
        }
        protected override void Start()
        {
            MLog.Print("Football: Start");
        }
        protected override void End()
        {
            MLog.Print("Football: End");
        }
    }

    public abstract class GameBase
    {
        protected abstract void Init();
        protected abstract void Start();
        protected abstract void End();
        public abstract void Play();
    }
    public abstract class Game : GameBase
    {
        public sealed override void Play()
        {
            Init();
            Start();
            End();
        }
    }
}
