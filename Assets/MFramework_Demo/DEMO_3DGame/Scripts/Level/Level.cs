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
                    MLog.Print($"{typeof(Level)}：场景中无Player脚本，请检查", MLogType.Error);
                }
                else if (players.Length > 1)
                {
                    MLog.Print($"{typeof(Level)}：场景中存在不止1个Player脚本，请检查", MLogType.Error);
                }
                else//正确情况
                {
                    m_player = players[0];
                }
            }

            return m_player;
        }
    }
}
