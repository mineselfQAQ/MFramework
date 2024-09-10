using MFramework.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : DialogPanelBase
{
    public Action OnStart;
    public Action OnEnd;

    protected Conversation currConversation;
    protected int currSentence;

    public override void Init()
    {
        m_ProfilePhoto_MImage.sprite = null;
        m_Name_MText.text = "Null";
        m_Message_MText.text = "";
    }

    public void RefreshView(Sprite sprite, string name, string message)
    {
        m_ProfilePhoto_MImage.sprite = sprite;
        m_Name_MText.text = name;
        m_Message_MText.SetMText(message);
    }

    public void StartDialog(Conversation conversation)
    {
        OpenSelf();
        currConversation = conversation;
        currSentence = 0;

        var sentence = conversation.sentences[currSentence];
        RefreshView(sentence.avatar, sentence.name, sentence.text);

        OnStart?.Invoke();
    }
    public void EndDialog()
    {
        currConversation = null;
        currSentence = -1;
        CloseSelf();

        OnEnd?.Invoke();
    }

    protected override void OnClicked(Button button) 
    {
        if (button == m_SpeechBubble_MButton)
        {
            if (currConversation == null) return;

            currSentence++;
            if (currSentence == currConversation.sentences.Count)
            {
                EndDialog();
                return;
            }

            var sentence = currConversation.sentences[currSentence];
            RefreshView(sentence.avatar, sentence.name, sentence.text);
            
            //if (m_Message_MText.mAnimator.typewriterPlaying)
            //{
            //    m_Message_MText.FinishTextImmediately();
            //}
            //else
            //{
            //    m_Message_MText.PlayText();
            //}
        }
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtitlity.LoadPanelSync(prefabPath);
    }
}