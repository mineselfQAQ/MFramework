using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NPCDialog : MonoBehaviour
{
    protected Collider m_collider;
    protected bool m_showing;

    protected virtual void Awake()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameTags.Player))
        {
            ShowTint();//显示交互键(如输入enter)

            if (true)//输入交互键
            {
                ShowDialog();//弹出对话框
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameTags.Player))
        {
            HideTint();//离开隐藏交互键
        }
    }
}
