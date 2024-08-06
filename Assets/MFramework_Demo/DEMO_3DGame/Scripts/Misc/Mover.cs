using MFramework;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public Vector3 offset;
    public float duration;
    public float resetDuration;

    protected Vector3 m_initialPosition;

    protected virtual void Start()
    {
        m_initialPosition = transform.localPosition;
    }

    public virtual void Apply()
    {
        Vector3 from = m_initialPosition;
        Vector3 to = m_initialPosition + offset;
        MTween.DoTween01((f) =>
        {
            transform.localPosition = Vector3.Lerp(from, to, f);
        }, MCurve.Linear, duration);
    }
}
