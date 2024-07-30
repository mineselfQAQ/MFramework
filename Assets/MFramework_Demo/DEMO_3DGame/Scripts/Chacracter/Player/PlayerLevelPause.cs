using UnityEngine;

public class PlayerLevelPause : MonoBehaviour
{
    protected Player m_player;
    protected LevelPauser m_pauser;

    protected virtual void Start()
    {
        m_player = GetComponent<Player>();
        m_pauser = LevelPauser.Instance;
    }

    protected virtual void Update()
    {
        //暂停条件：
        //1.人物在场 2.按下Pause键 3.当前可暂停
        if (m_player.inputs.GetPauseDown())
        {
            bool value = m_pauser.paused;
            m_pauser.Pause(!value);
        }
    }
}
