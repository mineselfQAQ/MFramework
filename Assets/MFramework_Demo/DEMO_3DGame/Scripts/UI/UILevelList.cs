using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UILevelList : MonoBehaviour
{
    public bool focusFirstElement = true;
    public UILevelCard card;
    public RectTransform container;

    protected List<UILevelCard> m_cardList = new List<UILevelCard>();

    protected virtual void Awake()
    {
        var levels = Game.Instance.levels;

        for (int i = 0; i < levels.Count; i++)
        {
            m_cardList.Add(Instantiate(card, container));
            m_cardList[i].Fill(levels[i]);
        }

        if (focusFirstElement)
        {
            EventSystem.current.SetSelectedGameObject(m_cardList[0].playBtn.gameObject);
        }
    }
}
