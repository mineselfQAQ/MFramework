using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Board : MonoBehaviour
{
    [Header("Sign Settings")]
    [TextArea(15, 20)]
    public string text = "Hello World";
    public float viewAngle = 90f;

    [Header("Canvas Settings")]
    public Canvas canvas;
    public MText boardText;
    public float scaleDuration = 0.25f;

    [Space(15)]
    public UnityEvent onShow;
    public UnityEvent onHide;

    protected Vector3 m_initialScale;
    protected bool m_showing;
    protected Collider m_collider;
    protected Camera m_camera;


    protected virtual void Awake()
    {
        boardText.text = text;
        m_initialScale = canvas.transform.localScale;
        canvas.transform.localScale = Vector3.zero;
        canvas.gameObject.SetActive(true);
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
        m_camera = Camera.main;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameTags.Player))
        {
            Hide();//离开隐藏
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTags.Player))
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, direction);
            
            bool inCameraSight = Vector3.Dot(m_camera.transform.forward, transform.forward) < 0;

            //显示要求：
            //1.站在板前
            //2.(Camera)看向板子正面
            if (angle < viewAngle && inCameraSight)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    public virtual void Show()
    {
        if (!m_showing)
        {
            m_showing = true;
            onShow?.Invoke();

            //缩放0->1
            MTween.DoTween01((f) =>
            {
                Vector3 scale = Vector3.Lerp(Vector3.zero, m_initialScale, f);
                canvas.transform.localScale = scale;
            }, MCurve.Linear, scaleDuration);
        }
    }

    public virtual void Hide()
    {
        if (m_showing)
        {
            m_showing = false;
            onHide?.Invoke();

            //缩放1->0
            Vector3 from = canvas.transform.localScale;
            MTween.DoTween01((f) =>
            {
                Vector3 scale = Vector3.Lerp(from, Vector3.zero, f);
                canvas.transform.localScale = scale;
            }, MCurve.Linear, scaleDuration);
        }
    }
}
