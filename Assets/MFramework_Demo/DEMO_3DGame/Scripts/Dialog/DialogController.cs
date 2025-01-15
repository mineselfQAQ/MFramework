using MFramework;
using UnityEngine;

public class DialogController : ComponentSingleton<DialogController>
{
    protected DialogPanel panel;

    protected Level m_level => Level.Instance;

    protected override void Awake()
    {
        base.Awake();

        MCoroutineManager.Instance.WaitNoRecord(() =>
        {
            panel = UIController.Instance.GetDialogPanel();
            panel.OnStart += () =>
            {
                Game.LockCursor(false);
                Time.timeScale = 0;

                m_level.player.inputs.DisableInputAction(InputActionName.pause);//½ûÓÃÔÝÍ£¼ü
                m_level.player.inputs.DisableInputAction(InputActionName.interact);//½ûÓÃ½»»¥¼ü
            };
            panel.OnEnd += () =>
            {
                Game.LockCursor();
                Time.timeScale = 1;

                m_level.player.inputs.EnableInputAction(InputActionName.pause);//ÆôÓÃÔÝÍ£¼ü
                m_level.player.inputs.EnableInputAction(InputActionName.interact);//½ûÓÃ½»»¥¼ü
            };
        }, MCore.Instance.isHotUpdateFinish, 2);
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
