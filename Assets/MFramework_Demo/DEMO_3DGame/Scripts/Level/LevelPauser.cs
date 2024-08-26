using MFramework;
using UnityEngine;
using UnityEngine.Events;

public class LevelPauser : ComponentSingleton<LevelPauser>
{
    public UnityEvent OnPause;
    public UnityEvent OnUnpause;

    public bool canPause { get; set; }
    public bool paused { get; protected set; }

    protected int pauseLevel = 0;

    protected bool canDo = true;

    /// <summary>
    /// ÔÝÍ£
    /// </summary>
    /// <param Name="value">True---ÔÝÍ£ False---È¡ÏûÔÝÍ£</param>
    public virtual void Pause(bool value)
    {
        if (paused != value && canDo)
        {
            if (!paused)//ÔÝÍ£
            {
                if (canPause)
                {
                    canDo = false;

                    Game.LockCursor(false);
                    paused = true;
                    Time.timeScale = 0;
                    UIController.Instance.OpenPausePanel(() => { canDo = true; });
                    OnPause?.Invoke();
                }
            }
            else//½â³ýÔÝÍ£
            {
                canDo = false;

                Game.LockCursor();
                paused = false;
                Time.timeScale = 1;
                UIController.Instance.ClosePausePanel(() => { canDo = true; });
                OnUnpause?.Invoke();
            }
        }
    }
}
