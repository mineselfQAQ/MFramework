using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISaveList : MonoBehaviour
{
    public bool focusFirstElement = true;
    public UISaveCard card;//UISaveCard Prefab
    public RectTransform container;//ָ����ScrollView�е�Content

    protected List<UISaveCard> m_cardList = new List<UISaveCard>();//�浵�б�

    protected virtual void Awake()
    {
        //��ȡJSON�д洢��GameData����
        var data = GameSaver.Instance.LoadList();

        //������ͼ
        for (int i = 0; i < data.Length; i++)
        {
            m_cardList.Add(Instantiate(this.card, container));
            m_cardList[i].Fill(i, data[i]);
        }

        if (focusFirstElement)
        {
            if (m_cardList[0].isFilled)
            {
                EventSystem.current.SetSelectedGameObject(m_cardList[0].loadBtn.gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(m_cardList[0].newGameBtn.gameObject);
            }
        }
    }
}
