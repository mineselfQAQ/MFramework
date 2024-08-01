using MFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelFinisher : ComponentSingleton<LevelFinisher>
{
    public bool unlockNextLevel;
    public string nextScene;
    public float loadingDelay = 1f;

    public UnityEvent OnFinish;
    public UnityEvent OnExit;

    protected Game m_game => Game.Instance;
    protected GameLoader m_loader => GameLoader.Instance;
    protected Level m_level => Level.Instance;
    protected LevelScore m_score => LevelScore.Instance;
    protected LevelPauser m_pauser => LevelPauser.Instance;

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

        Game.LockCursor(false);
        m_score.Save();
        if (string.IsNullOrEmpty(nextScene))//����һ������(���һ������)
        {
            //TODO:�ص�ѡ�ؽ���
        }
        else
        {
            m_loader.Load(nextScene);
        }
        OnFinish?.Invoke();
    }

    protected virtual IEnumerator ExitRoutine()
    {
        m_pauser.Pause(false);
        m_pauser.canPause = false;
        m_level.player.inputs.enabled = false;

        yield return new WaitForSeconds(loadingDelay);

        Game.LockCursor(false);
        //TODO:�ص�ѡ�ؽ���
        OnExit?.Invoke();
    }
}
