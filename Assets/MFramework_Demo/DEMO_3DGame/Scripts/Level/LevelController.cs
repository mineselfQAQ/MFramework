using UnityEngine;

public class LevelController : MonoBehaviour
{
    protected LevelFinisher m_finisher => LevelFinisher.Instance;
    protected LevelRespawner m_respawner => LevelRespawner.Instance;
    protected LevelScore m_score => LevelScore.Instance;
    protected LevelPauser m_pauser => LevelPauser.Instance;

    public virtual void Finish() => m_finisher.Finish();
    public virtual void Exit() => m_finisher.Exit();

    public virtual void Respawn(bool consumeRetries) => m_respawner.Respawn(consumeRetries);
    public virtual void Restart() => m_respawner.Restart();

    public virtual void AddCoins(int amount) => m_score.coins += amount;
    public virtual void CollectStar(int index) => m_score.CollectStar(index);
    public virtual void SaveScore() => m_score.Save();

    public virtual void Pause(bool value) => m_pauser.Pause(value);
}
