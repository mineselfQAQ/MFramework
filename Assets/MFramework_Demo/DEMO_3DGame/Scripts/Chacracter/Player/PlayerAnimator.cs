using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //强制转换信息
    [System.Serializable]
    public class ForcedTransition
    {
        public int fromStateId;//结束状态ID
        public int animationLayer;//所在Animator中的Layer
        public string toAnimationState;//强制到达状态String
    }

    public Animator animator;

    [Header("Parameters Names")]
    public string stateName = "State";
    public string lastStateName = "Last State";
    public string lateralSpeedName = "Lateral Speed";
    public string verticalSpeedName = "Vertical Speed";
    public string lateralAnimationSpeedName = "Lateral Animation Speed";
    public string healthName = "Health";
    public string jumpCounterName = "Jump Counter";
    public string isGroundedName = "Is Grounded";
    public string isHoldingName = "Is Holding";
    public string onStateChangedName = "On State Changed";

    [Header("Settings")]
    public float minLateralAnimationSpeed = 0.5f;
    public List<ForcedTransition> forcedTransitions;

    protected int m_stateHash;
    protected int m_lastStateHash;
    protected int m_lateralSpeedHash;
    protected int m_verticalSpeedHash;
    protected int m_lateralAnimationSpeedHash;
    protected int m_healthHash;
    protected int m_jumpCounterHash;
    protected int m_isGroundedHash;
    protected int m_isHoldingHash;
    protected int m_onStateChangedHash;

    protected Dictionary<int, ForcedTransition> m_forcedTransitions;

    protected Player m_player;

    protected virtual void Start()
    {
        InitializePlayer();
        InitializeForcedTransitions();
        InitializeParametersHash();
        InitializeAnimatorTriggers();
    }

    protected virtual void LateUpdate()
    {
        HandleAnimatorParameters();
    }

    protected virtual void InitializePlayer()
    {
        m_player = GetComponent<Player>();
        m_player.states.events.onChange.AddListener(HandleForcedTransitions);
    }
    protected virtual void InitializeForcedTransitions()
    {
        m_forcedTransitions = new Dictionary<int, ForcedTransition>();

        foreach (var transition in forcedTransitions)
        {
            if (!m_forcedTransitions.ContainsKey(transition.fromStateId))
            {
                m_forcedTransitions.Add(transition.fromStateId, transition);
            }
        }
    }
    protected virtual void InitializeAnimatorTriggers()
    {
        m_player.states.events.onChange.AddListener(() => animator.SetTrigger(m_onStateChangedHash));
    }
    protected virtual void InitializeParametersHash()
    {
        //为了提升性能与内存开销，使用hash是更好的
        m_stateHash = Animator.StringToHash(stateName);
        m_lastStateHash = Animator.StringToHash(lastStateName);
        m_lateralSpeedHash = Animator.StringToHash(lateralSpeedName);
        m_verticalSpeedHash = Animator.StringToHash(verticalSpeedName);
        m_lateralAnimationSpeedHash = Animator.StringToHash(lateralAnimationSpeedName);
        m_healthHash = Animator.StringToHash(healthName);
        m_jumpCounterHash = Animator.StringToHash(jumpCounterName);
        m_isGroundedHash = Animator.StringToHash(isGroundedName);
        m_isHoldingHash = Animator.StringToHash(isHoldingName);
        m_onStateChangedHash = Animator.StringToHash(onStateChangedName);
    }

    /// <summary>
    /// 当从某状态到达新状态时，强制执行动画
    /// </summary>
    protected virtual void HandleForcedTransitions()
    {
        int lastStateIndex = m_player.states.lastIndex;

        if (m_forcedTransitions.ContainsKey(lastStateIndex))
        {
            int layer = m_forcedTransitions[lastStateIndex].animationLayer;
            animator.Play(m_forcedTransitions[lastStateIndex].toAnimationState, layer);
        }
    }

    /// <summary>
    /// 实时更新Animator参数(保证在状态切换时执行相应的动画)
    /// </summary>
    protected virtual void HandleAnimatorParameters()
    {
        float lateralSpeed = m_player.lateralVelocity.magnitude;
        float verticalSpeed = m_player.verticalVelocity.y;
        float lateralAnimationSpeed = Mathf.Max(minLateralAnimationSpeed, lateralSpeed / m_player.stats.current.topSpeed);

        animator.SetInteger(m_stateHash, m_player.states.index);
        animator.SetInteger(m_lastStateHash, m_player.states.lastIndex);
        animator.SetFloat(m_lateralSpeedHash, lateralSpeed);
        animator.SetFloat(m_verticalSpeedHash, verticalSpeed);
        animator.SetFloat(m_lateralAnimationSpeedHash, lateralAnimationSpeed);
        animator.SetInteger(m_healthHash, m_player.health.current);
        animator.SetInteger(m_jumpCounterHash, m_player.jumpCounter);
        animator.SetBool(m_isGroundedHash, m_player.isGrounded);
        animator.SetBool(m_isHoldingHash, m_player.holding);
    }
}