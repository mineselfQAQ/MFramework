using MFramework;
using MFramework.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class UIManualScroll : MonoBehaviour
{
    public float scrollDuration = 0.25f;

    protected int m_currentChild;
    protected int m_totalChildren;

    protected bool m_moving;
    protected float m_moveInitTime;
    protected float m_moveRepeatTime;

    protected List<MButton> m_btns = new List<MButton>();

    protected ScrollRect m_scrollRect;
    protected InputSystemUIInputModule m_input;

    protected const float k_inputRepeatDelay = 0.1f;

    protected virtual void Start()
    {
        m_scrollRect = GetComponent<ScrollRect>();
        m_input = EventSystem.current.GetComponent<InputSystemUIInputModule>();
        m_totalChildren = m_scrollRect.content.childCount;

        m_scrollRect.content.GetComponentsInChildren<MButton>(false, m_btns);
        for (int i = 0; i < m_btns.Count; i++)
        {
            var btn = m_btns[i];
            if (btn.name != "LoadBtn" || btn.name != "NewGameBtn")
            {
                m_btns.Remove(btn);
            }
        }
    }

    protected virtual void Update()
    {
        var horizontal = m_input.move.action.ReadValue<Vector2>().x;

        if (horizontal != 0)//按住
        {
            //TODO:需要在未选中时进入选中
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(m_btns[m_currentChild].gameObject);
            }

            //第一次触发间隔---moveRepeatDelay(EventSystem中可设置)
            if (m_moveInitTime + m_input.moveRepeatDelay < Time.time)
            {
                if (!m_moving)//按下一刻
                {
                    m_moving = true;
                    m_moveInitTime = Time.time;
                }

                //第n次触发间隔---k_inputRepeatDelay
                if (m_moveRepeatTime + k_inputRepeatDelay < Time.time)
                {
                    if (horizontal > 0) m_currentChild++;
                    else m_currentChild--;

                    m_moveRepeatTime = Time.time;
                    m_currentChild = Mathf.Clamp(m_currentChild, 0, m_totalChildren - 1);
                    Scroll();
                }
            }
        }
        else//松开
        {
            m_moving = false;
            m_moveInitTime = 0;
            m_moveRepeatTime = 0;
        }
    }

    protected virtual void Scroll()
    {
        float from = m_scrollRect.horizontalNormalizedPosition;
        float to = m_currentChild / ((float)m_totalChildren - 1);
        MTween.DoTween01NoRecord((f) =>
        {
            m_scrollRect.horizontalNormalizedPosition = Mathf.Lerp(from, to, f);
        }, MCurve.Linear, scrollDuration);
    }
}
