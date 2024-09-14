using MFramework;
using MFramework.UI;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    public float duration = 1.0f;
    public float fadeDistance = 0.5f;
    public float detectRadius = 3.0f;
    public Conversation conversation;

    public CanvasGroup canvas;

    protected Camera m_camera;
    protected CapsuleCollider m_collider;
    protected bool m_hintShowing;

    protected Vector3 m_srcPos;
    protected Vector3 m_desPos;

    protected bool m_isTriggering = false;

    protected Level m_level => Level.Instance;

    protected virtual void Start()
    {
        m_camera = Camera.main;

        m_desPos = canvas.transform.localPosition;
        m_srcPos = m_desPos - new Vector3(0, fadeDistance, 0);

        m_collider = gameObject.AddComponent<CapsuleCollider>();
        m_collider.isTrigger = true;
        m_collider.radius = detectRadius;

        canvas.gameObject.SetActive(false);
        var text = canvas.gameObject.FindChildByName("MText").GetComponent<MText>();
        var inputAction = m_level.player.inputs.GetInputAction(InputActionName.interact);
        string keyName = m_level.player.inputs.GetKey(inputAction, "Keyboard");//TODO:只支持键盘
        text.text = keyName;
    }

    public virtual void OnEnter()
    {
        m_isTriggering = true;
        ShowHint();
    }

    public virtual void OnStay()
    {
        canvas.transform.LookAt(m_camera.transform);

        if (m_level.player.inputs.GetInteractDown())
        {
            DialogController.Instance.StartDialog(conversation);//弹出对话框
        }
    }

    public virtual void OnExit()
    {
        m_isTriggering = false;
        HideHint();//离开隐藏交互键
    }

    protected virtual void ShowHint()
    {
        m_hintShowing = true;
        canvas.gameObject.SetActive(true);
        
        //上升+渐入
        MTween.FixedDoTween01NoRecord((f) =>
        {
            Vector3 pos = Vector3.Lerp(m_srcPos, m_desPos, f);
            float alpha = Mathf.Lerp(0, 1, f);
            canvas.transform.localPosition = pos;
            canvas.alpha = alpha;
        }, MCurve.Linear, duration);
    }
    protected virtual void HideHint()
    {
        //下降+渐出
        MTween.FixedDoTween01NoRecord((f) =>
        {
            Vector3 pos = Vector3.Lerp(m_desPos, m_srcPos, f);
            float alpha = Mathf.Lerp(1, 0, f);
            canvas.transform.localPosition = pos;
            canvas.alpha = alpha;
        }, MCurve.Linear, duration, () =>
        {
            canvas.gameObject.SetActive(false);
        });
    }
}