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
        //��ͣ������
        //1.�����ڳ� 2.����Pause�� 3.��ǰ����ͣ
        if (m_player.inputs.GetPauseDown())
        {
            bool value = m_pauser.paused;
            m_pauser.Pause(!value);
        }
    }
}
