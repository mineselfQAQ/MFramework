using MFramework;
using UnityEngine;

/// <summary>
/// ƽ�Ʋ���(��ȥ�ɻ�)
/// </summary>
public class Mover : MonoBehaviour
{
    public static int num = 0;

    public Vector3 offset;
    public float applyDuration = 0.25f;
    public float undoDuration = 0.25f;

    public CurveType curveType = CurveType.Linear;

    protected Vector3 m_initialPosition;

    protected string m_applyTweenName = $"Apply{typeof(Mover)}{num++}";
    protected string m_undoTweenName = $"Undo{typeof(Mover)}{num++}";

    protected virtual void Start()
    {
        m_initialPosition = transform.localPosition;
    }

    /// <summary>
    /// Ӧ���ƶ�
    /// </summary>
    public virtual void Apply()
    {
        MCoroutineManager.Instance.EndCoroutine(m_undoTweenName);//��ֹЯ�̽��е�һ��ĳ�ͻ

        Vector3 from = m_initialPosition;
        Vector3 to = m_initialPosition + offset;
        MTween.DoTween01(m_applyTweenName, (f) =>
        {
            transform.localPosition = Vector3.Lerp(from, to, f);
        }, MCurve.GetMCurve(curveType), applyDuration);
    }

    /// <summary>
    /// �����ƶ�
    /// </summary>
    public virtual void Undo()
    {
        MCoroutineManager.Instance.EndCoroutine(m_applyTweenName);//��ֹЯ�̽��е�һ��ĳ�ͻ

        Vector3 from = transform.localPosition;
        Vector3 to = m_initialPosition;
        MTween.DoTween01(m_undoTweenName, (f) =>
        {
            transform.localPosition = Vector3.Lerp(from, to, f);
        }, MCurve.GetMCurve(curveType), undoDuration);
    }
}
