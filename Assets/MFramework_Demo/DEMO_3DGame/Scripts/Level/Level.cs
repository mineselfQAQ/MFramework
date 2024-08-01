using MFramework;
using UnityEngine;

[RequireComponent(typeof(LevelStarter))]
[RequireComponent(typeof(LevelFinisher))]
[RequireComponent(typeof(LevelPauser))]
[RequireComponent(typeof(LevelRespawner))]
[RequireComponent(typeof(LevelScore))]
public class Level : ComponentSingleton<Level>
{
    protected Player m_player;

    public Player player
    {
        get
        {
            if (!m_player)
            {
                var players = FindObjectsOfType<Player>();
                if (players.Length == 0)
                {
                    MLog.Print($"{typeof(Level)}����������Player�ű�������", MLogType.Error);
                }
                else if (players.Length > 1)
                {
                    MLog.Print($"{typeof(Level)}�������д��ڲ�ֹ1��Player�ű�������", MLogType.Error);
                }
                else//��ȷ���
                {
                    m_player = players[0];
                }
            }

            return m_player;
        }
    }
}
