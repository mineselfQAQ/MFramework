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
            ShowTint();//��ʾ������(������enter)

            if (true)//���뽻����
            {
                ShowDialog();//�����Ի���
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameTags.Player))
        {
            HideTint();//�뿪���ؽ�����
        }
    }
}
