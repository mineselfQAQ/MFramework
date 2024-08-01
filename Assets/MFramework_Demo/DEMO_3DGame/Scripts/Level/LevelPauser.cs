using MFramework;
using UnityEngine;
using UnityEngine.Events;

public class LevelPauser : ComponentSingleton<LevelPauser>
{
    public UnityEvent OnPause;

    public UnityEvent OnUnpause;

    public bool canPause { get; set; } = true;

    public bool paused { get; protected set; }

    public virtual void Pause(bool value)
    {
        if (paused != value)
        {
            if (!paused)//ÔÝÍ£
            {
                if (canPause)
                {
                    Game.LockCursor(false);
                    paused = true;
                    Time.timeScale = 0;
                    UIController.Instance.OpenPausePanel();
                    OnPause?.Invoke();
                }
            }
            else//½â³ýÔÝÍ£
            {
                Game.LockCursor();
                paused = false;
                Time.timeScale = 1;
                UIController.Instance.ClosePausePanel();
                OnUnpause?.Invoke();
            }
        }
    }
}
