using MFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelFinisher : ComponentSingleton<LevelFinisher>
{
    public bool unlockNextLevel;
    public string nextScene;
    public float loadingDelay = 1f;

    public UnityEvent<bool> OnFinish;//True---����ѡ�ؽ��� False---ǰ����һ��
    public UnityEvent OnExit;

    protected Game m_game => Game.Instance;
    protected GameLoader m_loader => GameLoader.Instance;
    protected Level m_level => Level.Instance;
    protected LevelScore m_score => LevelScore.Instance;
    protected LevelPauser m_pauser => LevelPauser.Instance;

    protected UIController m_controller => UIController.Instance;

    /// <summary>
    /// ͨ��
    /// </summary>
    public virtual void Finish()
    {
        StartCoroutine(FinishRoutine());
    }

    /// <summary>
    /// �˳�
    /// </summary>
    public virtual void Exit()
    {
        StartCoroutine(ExitRoutine());
    }

    protected virtual IEnumerator FinishRoutine()
    {
        m_pauser.Pause(false);//��֤�����ͣ
        m_pauser.canPause = false;
        m_score.stopTime = true;
        m_level.player.inputs.enabled = false;

        yield return new WaitForSeconds(loadingDelay);

        if (unlockNextLevel)
        {
            m_game.UnlockNextLevel();
        }

        m_score.Save();
        if (string.IsNullOrEmpty(nextScene))//����һ������(���һ������)
        {
            m_loader.Load(UIController.titleScreenSceneName, () =>
            {
                m_controller.DestroyHUD();

                var levelSelectPanel = m_controller.bottomRoot.
                    GetPanel<LevelSelectPanel>(UIController.levelSelectPanelName);
                m_controller.bottomRoot.OpenPanel(UIController.levelSelectPanelName);

                levelSelectPanel.Refresh();
                Game.LockCursor(false);
                OnFinish?.Invoke(true);
            });
        }
        else
        {
            m_loader.Load(nextScene, () =>
            {
                Game.LockCursor(false);
                OnFinish?.Invoke(false);
            });
        }
    }

    protected virtual IEnumerator ExitRoutine()
    {
        m_pauser.Pause(false);
        m_pauser.canPause = false;
        m_level.player.inputs.enabled = false;

        yield return new WaitForSeconds(loadingDelay);

        m_loader.Load(UIController.titleScreenSceneName, () =>
        {
            m_controller.DestroyHUD();

            m_controller.bottomRoot.OpenPanel(UIController.levelSelectPanelName);

            Game.LockCursor(false);
            OnExit?.Invoke();
        });
    }
}
