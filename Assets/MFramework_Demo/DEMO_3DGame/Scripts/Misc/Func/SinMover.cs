using MFramework;
using UnityEngine;

public class SinMover : MonoBehaviour
{
    public bool autoPlay = true;
    public bool randomStart = false;
    public Vector3 offset;
    public float frequency = 1.0f;

    protected string id;
    protected Vector3 m_initialPosition;

    protected static int i = 0;

    protected virtual void Start()
    {
        GameLoader.Instance.OnLoadStart.AddListener(StopMove);

        m_initialPosition = transform.localPosition;
        id = $"SinMover_{i++}";

        if (autoPlay)
        {
            StartMove();
        }
    }

    public void StartMove()
    {
        Vector3 from = m_initialPosition;
        MTween.SinLoop(id, (f) =>
        {
            transform.localPosition = from + f * offset;
        }, MCurve.Linear, 1, frequency, randomStart);
    }

    public void StopMove()
    {
        MCoroutineManager.Instance.EndCoroutine(id);
    }
}
