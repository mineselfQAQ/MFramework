using MFramework;
using UnityEngine;

//TODO:Ìæ»»Float½Å±¾
public class SinMover : MonoBehaviour
{
    public bool autoPlay = true;
    public bool randomStart = false;
    public Vector3 offset;
    public float frequency = 1.0f;

    protected string name;
    protected Vector3 m_initialPosition;

    protected static int i = 0;
    protected string Name => $"SinMover_{i++}";

    protected virtual void Start()
    {
        GameLoader.Instance.OnLoadStart.AddListener(StopMove);

        m_initialPosition = transform.localPosition;
        name = Name;

        if(autoPlay)
        {
            StartMove();
        }
    }

    public void StartMove()
    {
        Vector3 from = m_initialPosition;
        MTween.SinLoop(name, (f) =>
        {
            transform.localPosition = from + f * offset;
        }, MCurve.Linear, 1, frequency, randomStart);
    }

    public void StopMove()
    {
        MCoroutineManager.Instance.EndCoroutine(name);
    }
}
