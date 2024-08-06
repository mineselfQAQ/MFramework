using MFramework;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public Vector3 offset;
    public float applyDuration = 0.25f;
    public float undoDuration = 0.25f;

    protected Vector3 m_initialPosition;

    protected virtual void Start()
    {
        m_initialPosition = transform.localPosition;
    }

    /// <summary>
    /// 应用移动
    /// </summary>
    public virtual void Apply()
    {
        Vector3 from = m_initialPosition;
        Vector3 to = m_initialPosition + offset;
        MTween.DoTween01((f) =>
        {
            transform.localPosition = Vector3.Lerp(from, to, f);
        }, MCurve.Linear, applyDuration);
    }

    /// <summary>
    /// 撤回移动
    /// </summary>
    public virtual void Undo()
    {
        Vector3 from = transform.localPosition;
        Vector3 to = m_initialPosition;
        MTween.DoTween01((f) =>
        {
            transform.localPosition = Vector3.Lerp(from, to, f);
        }, MCurve.Linear, undoDuration);
    }
}
