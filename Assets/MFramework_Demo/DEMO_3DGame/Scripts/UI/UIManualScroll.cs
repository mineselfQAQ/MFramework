using MFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class UIManualScroll : MonoBehaviour
{
    public float scrollDuration = 0.25f;
    public List<string> btnNames;

    protected int m_currentChild;
    protected int m_totalChildren;

    protected bool m_moving;
    protected float m_moveInitTime;
    protected float m_moveRepeatTime;

    protected List<List<GameObject>> widgetsBtnNames = new List<List<GameObject>>();

    protected ScrollRect m_scrollRect;
    protected InputSystemUIInputModule m_input;

    protected const float k_inputRepeatDelay = 0.1f;

    protected virtual void Start()
    {
        m_scrollRect = GetComponent<ScrollRect>();
        m_input = EventSystem.current.GetComponent<InputSystemUIInputModule>();
        m_totalChildren = m_scrollRect.content.childCount;

        //获取所有Widget下的对应Btns
        List<Transform> childs = new List<Transform>();
        foreach (Transform child in m_scrollRect.content)
        {
            List<GameObject> gos = new List<GameObject>();
            foreach (string name in btnNames)
            {
                var go = child.FindChildByName(name).gameObject;
                gos.Add(go);
            }
            widgetsBtnNames.Add(gos);
        }

        //特殊处理---解决默认会有-1输入的问题
        m_input.move.action.Disable();
        m_input.move.action.Enable();
    }

    protected virtual void Update()
    {
        var horizontal = m_input.move.action.ReadValue<Vector2>().x;

        if (horizontal != 0)//按住
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                //选择第一个按钮
                EventSystem.current.SetSelectedGameObject(GetActiveGo(widgetsBtnNames[0]));
                m_currentChild = -1;//-1的话无论如何都会变为0
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
        MTween.UnscaledDoTween01NoRecord((f) =>
        {
            m_scrollRect.horizontalNormalizedPosition = Mathf.Lerp(from, to, f);
        }, MCurve.Linear, scrollDuration);
    }

    protected virtual GameObject GetActiveGo(List<GameObject> gos)
    {
        foreach (GameObject go in gos) 
        {
            if (go.activeSelf == true) return go;
        }
        return null;
    }
}
