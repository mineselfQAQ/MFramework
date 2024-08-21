using MFramework;
using UnityEngine;

//TODO:替换Float脚本
public class SinMover : MonoBehaviour
{
    public bool autoPlay = true;
    public bool randomStart = false;
    public Vector3 offset;
    public float frequency = 1.0f;

    protected Vector3 m_initialPosition;

    protected virtual void Start()
    {
        m_initialPosition = transform.localPosition;

        if(autoPlay)
        {
            StartMove();
        }
    }

    public void StartMove()
    {
        Vector3 from = m_initialPosition;
        MTween.SinLoop((f) =>
        {
            transform.localPosition = from + f * offset;
        }, MCurve.Linear, 1, frequency, randomStart);
    }

    public void StopMove()
    {
        //TODO:需要先添加SinLoop()的记录版本
    }
}
