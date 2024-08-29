using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelRespawner : ComponentSingleton<LevelRespawner>
{
    public float respawnStartDelay = 1f;
    public float respawnEndDelay = 0.5f;
    public float gameOverDelay = 5f;
    public float restartDelay = 0.5f;

    public UnityEvent OnRespawn;
    public UnityEvent OnGameOver;

    protected List<PlayerCamera> m_cameras;

    protected Level m_level => Level.Instance;
    protected LevelScore m_score => LevelScore.Instance;
    protected LevelPauser m_pauser => LevelPauser.Instance;
    protected Game m_game => Game.Instance;
    protected GameLoader m_loader => GameLoader.Instance;
    protected UIController m_controller => UIController.Instance;

    protected virtual void Start()
    {
        m_cameras = new List<PlayerCamera>(FindObjectsOfType<PlayerCamera>());
        m_level.player.playerEvents.OnDie.AddListener(() => Respawn(true));
    }

    public virtual void Respawn(bool consumeRetries)
    {
        StartCoroutine(Routine(consumeRetries));
    }

    public virtual void Restart()
    {
        StartCoroutine(RestartRoutine());
    }

    protected virtual IEnumerator Routine(bool consumeRetries)
    {
        m_pauser.Pause(false);
        m_pauser.canPause = false;
        m_level.player.inputs.enabled = false;

        //��������ʱ����Ϸ����
        if (consumeRetries && m_game.retries == 1)
        {
            StartCoroutine(GameOverRoutine());
            yield break;
        }

        yield return new WaitForSeconds(respawnStartDelay);

        //����
        MUIUtitlity.BlackIn(() => 
        {
            StartCoroutine(RespawnRoutine(consumeRetries)); 
        });
    }
    protected virtual IEnumerator GameOverRoutine()
    {
        m_score.stopTime = true;
        yield return new WaitForSeconds(gameOverDelay);

        //����1��ֱ�Ӽ���(��������)
        //GameLoader.Instance.Reload();
        //����2������ѡ�ؽ���
        m_score.GameOverSave();
        m_loader.Load(UIController.titleScreenSceneName, () =>
        {
            m_controller.CloseHUD();

            var fileSelectPanel = m_controller.bottomRoot.
                GetPanel<FileSelectPanel>(UIController.fileSelectPanelName);
            m_controller.bottomRoot.OpenPanel(UIController.fileSelectPanelName);

            fileSelectPanel.Refresh();
            Game.LockCursor(false);
        });

        OnGameOver?.Invoke();
    }
    protected virtual IEnumerator RespawnRoutine(bool consumeRetries)
    {
        if (consumeRetries)
        {
            m_game.retries--;
        }

        if(m_level.player.pickable) m_level.player.pickable.Respawn();
        m_level.player.Respawn();
        //m_score.coins = 0;
        ResetCameras();
        OnRespawn?.Invoke();

        yield return new WaitForSeconds(respawnEndDelay);

        MUIUtitlity.BlackOut(() =>
        {
            m_pauser.canPause = true;
            m_level.player.inputs.enabled = true;
        });
    }

    protected virtual IEnumerator RestartRoutine()
    {
        m_pauser.Pause(false);
        m_pauser.canPause = false;
        m_level.player.inputs.enabled = false;
        yield return new WaitForSeconds(restartDelay);
        GameLoader.Instance.Reload();
    }

    /// <summary>
    /// ��������PlayerCamera���
    /// </summary>
    protected virtual void ResetCameras()
    {
        foreach (var camera in m_cameras)
        {
            camera.ResetCamera();
        }
    }
}
