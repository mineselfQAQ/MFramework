using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelRespawner : ComponentSingleton<LevelRespawner>
{
    public float respawnFadeOutDelay = 1f;
    public float respawnFadeInDelay = 0.5f;
    public float gameOverFadeOutDelay = 5f;
    public float restartFadeOutDelay = 0.5f;

    public UnityEvent OnRespawn;
    public UnityEvent OnGameOver;

    protected List<PlayerCamera> m_cameras;

    protected Level m_level => Level.Instance;
    protected LevelScore m_score => LevelScore.Instance;
    protected LevelPauser m_pauser => LevelPauser.Instance;
    protected Game m_game => Game.Instance;

    protected Fader m_fader => Fader.Instance;//TODO:�滻���ҵ���ʽ

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
        if (consumeRetries && m_game.retries == 0)
        {
            StartCoroutine(GameOverRoutine());
            yield break;
        }

        yield return new WaitForSeconds(respawnFadeOutDelay);

        //����
        //Ŀǰ������������ִ��RespawnRoutine()����ɺ�ȡ������
        m_fader.FadeOut(() => StartCoroutine(RespawnRoutine(consumeRetries)));
    }
    protected virtual IEnumerator GameOverRoutine()
    {
        m_score.stopTime = true;
        yield return new WaitForSeconds(gameOverFadeOutDelay);
        GameLoader.Instance.Reload();
        OnGameOver?.Invoke();
    }
    protected virtual IEnumerator RespawnRoutine(bool consumeRetries)
    {
        if (consumeRetries)
        {
            m_game.retries--;
        }

        m_level.player.Respawn();
        m_score.coins = 0;
        ResetCameras();
        OnRespawn?.Invoke();

        yield return new WaitForSeconds(respawnFadeInDelay);

        m_fader.FadeIn(() =>
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
        yield return new WaitForSeconds(restartFadeOutDelay);
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
