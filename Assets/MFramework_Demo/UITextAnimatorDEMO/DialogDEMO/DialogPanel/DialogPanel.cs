using MFramework.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : DialogPanelBase
{
    private List<string> dialogues = new List<string>() 
    {
        "Hello, I'm Mineself.",
        "I'm <Exclaim(1,1.2,255,0,0)>so Scared</>.",
        "<wave>HaHaHaHaHaHaHaHaHaHaHaHaHaHaHaHaHaHaHaHaHa</>",
        "i <color(255,0,0)>cant't</> <shake>do well</>!",
        "Press 1 to <scale(1.2)>Scale</>, Press 2 to <color(0,0,255)>Blue</>, Press 3 to <rotate(15,1)>Rotate</>"
    };

    private int curIndex = 1;

    public void Init(string name, Sprite profilePhoto)
    {
        m_Name_MText.text = name;
        m_ProfilePhoto_MImage.sprite = profilePhoto;

        m_Text_MText.text = dialogues[0];
    }

    public void Update()
    {
        if (curIndex == dialogues.Count)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                m_Text_MText.ApplyEffect(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                m_Text_MText.ApplyEffect(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                m_Text_MText.ApplyEffect(2);
            }
        }
    }

    protected override void OnClicked(Button button)
    {
        if (button == m_Background_MButton)
        {
            if (m_Text_MText.mAnimator.typewriterPlaying)
            {
                m_Text_MText.FinishTextImmediately();
            }
            else
            {
                if (curIndex >= dialogues.Count) return;
                if (curIndex == dialogues.Count - 1) m_Text_MText.mAnimator.inlineEffectsAutoDo = false;
                m_Text_MText.SetMText(dialogues[curIndex++]);
            }
        }
    }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }
}