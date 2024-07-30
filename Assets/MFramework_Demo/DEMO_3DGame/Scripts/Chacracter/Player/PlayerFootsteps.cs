using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [System.Serializable]
    public class Surface
    {
        public string tag;
        public AudioClip[] footsteps;
        public AudioClip[] landings;
    }

    //特殊表面
    public Surface[] surfaces;
    //默认表面
    public AudioClip[] defaultFootsteps;
    public AudioClip[] defaultLandings;

    [Header("General Settings")]
    public float stepOffset = 1.25f;
    public float footstepVolume = 0.5f;

    protected Vector3 m_lastLateralPosition;
    protected Dictionary<string, AudioClip[]> m_footsteps = new Dictionary<string, AudioClip[]>();
    protected Dictionary<string, AudioClip[]> m_landings = new Dictionary<string, AudioClip[]>();

    protected Player m_player;
    protected AudioSource m_audio;

    protected virtual void Start()
    {
        m_player = GetComponent<Player>();
        m_audio = gameObject.GetOrAddComponent<AudioSource>();

        m_player.entityEvents.OnGroundEnter.AddListener(Landing);

        foreach (var surface in surfaces)
        {
            m_footsteps.Add(surface.tag, surface.footsteps);
            m_landings.Add(surface.tag, surface.landings);
        }
    }

    protected virtual void Update()
    {
        //脚步出现条件：
        //1.在地面 2.在Walk状态
        if (m_player.isGrounded && m_player.states.IsCurrentOfType(typeof(WalkPlayerState)))
        {
            Vector3 position = transform.position;
            Vector3 lateralPosition = new Vector3(position.x, 0, position.z);
            float distance = (m_lastLateralPosition - lateralPosition).magnitude;

            //当步幅超过stepOffset时，可以出声，否则为静步
            if (distance >= stepOffset)
            {
                //特殊地面
                if (m_footsteps.ContainsKey(m_player.groundHit.collider.tag))
                {
                    PlayRandomClip(m_footsteps[m_player.groundHit.collider.tag]);
                }
                else//一般地面
                {
                    PlayRandomClip(defaultFootsteps);
                }

                m_lastLateralPosition = lateralPosition;
            }
        }
    }

    protected virtual void Landing()
    {
        if (!m_player.onWater)
        {
            //特殊地面
            if (m_landings.ContainsKey(m_player.groundHit.collider.tag))
            {
                PlayRandomClip(m_landings[m_player.groundHit.collider.tag]);
            }
            else//一般地面
            {
                PlayRandomClip(defaultLandings);
            }
        }
    }

    /// <summary>
    /// 随机播放一种音效
    /// </summary>
    protected virtual void PlayRandomClip(AudioClip[] clips)
    {
        if (clips.Length > 0)
        {
            var index = Random.Range(0, clips.Length);
            m_audio.PlayOneShot(clips[index], footstepVolume);
        }
    }
}
