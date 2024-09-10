using MFramework;
using UnityEngine;

public class DialogController : ComponentSingleton<DialogController>
{
    protected DialogPanel panel;

    protected Level m_level => Level.Instance;

    protected void Start()
    {
        panel = UIController.Instance.GetDialogPanel();
        panel.OnStart += () =>
        {
            Game.LockCursor(false);
            Time.timeScale = 0;

            m_level.player.inputs.DisableInputAction(InputActionName.pause);//Ω˚”√‘›Õ£º¸
        };
        panel.OnEnd += () =>
        {
            Game.LockCursor();
            Time.timeScale = 1;

            m_level.player.inputs.EnableInputAction(InputActionName.pause);//∆Ù”√‘›Õ£º¸
        };
    }

    public void StartDialog(Conversation conversation)
    {
        panel.StartDialog(conversation);
    }
    public void EndDialog()
    {
        panel.EndDialog();
    }
}
