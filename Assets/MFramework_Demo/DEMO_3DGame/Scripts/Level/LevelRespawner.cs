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

        //无命可用时，游戏结束
        if (consumeRetries && m_game.retries == 1)
        {
            StartCoroutine(GameOverRoutine());
            yield break;
        }

        yield return new WaitForSeconds(respawnStartDelay);

        //重生
        if (consumeRetries)
        {
            m_controller.OpenTakeBloodRestartWidget(() =>
            {
                StartCoroutine(RespawnRoutine(consumeRetries));
            });
        }
        else
        {
            m_controller.OpenRestartWidget(() =>
            {
                StartCoroutine(RespawnRoutine(consumeRetries));
            });
        }
    }
    protected virtual IEnumerator GameOverRoutine()
    {
        m_score.stopTime = true;
        yield return new WaitForSeconds(gameOverDelay);

        //方案1：直接加载(无限续命)
        //GameLoader.Instance.Reload();
        //方案2：返回选关界面
        m_score.GameOverSave();
        m_controller.OpenGameOverWidget(() =>
        {
            MCoroutineManager.Instance.DelayNoRecord(() =>
            {
                m_loader.Load(UIController.titleScreenSceneName, $"{ABPath.ABROOTPATH}/3DGame_TitleScreen.unity", () =>
                {
                    m_controller.CloseHUD();
                    m_controller.OnlyCloseGameOverWidget();
                    m_controller.OpenFileSelectPanel();

                    Game.LockCursor(false);
                });

                OnGameOver?.Invoke();
            }, 3.0f);
        });
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

        if (consumeRetries)
        {
            m_controller.CloseTakeBloodRestartWidget(() =>
            {
                m_pauser.canPause = true;
                m_level.player.inputs.enabled = true;
            });
        }
        else
        {
            m_controller.CloseRestartWidget(() =>
            {
                m_pauser.canPause = true;
                m_level.player.inputs.enabled = true;
            });
        }
    }

    protected virtual IEnumerator RestartRoutine()
    {
        m_pauser.Pause(false);
        m_pauser.canPause = false;
        m_level.player.inputs.enabled = false;
        yield return new WaitForSeconds(restartDelay);
        m_loader.Reload($"{ABPath.ABROOTPATH}/{m_loader.currentScene}.unity");
    }

    /// <summary>
    /// 重置所有PlayerCamera相机
    /// </summary>
    protected virtual void ResetCameras()
    {
        foreach (var camera in m_cameras)
        {
            camera.ResetCamera();
        }
    }
}