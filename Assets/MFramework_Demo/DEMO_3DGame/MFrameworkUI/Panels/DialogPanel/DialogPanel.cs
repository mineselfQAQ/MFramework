using MFramework;
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
    protected bool stopDo;

    public override void Init()
    {
        m_ProfilePhoto_MImage.sprite = null;
        m_Name_MText.text = "Null";
        m_Message_MText.text = "";
        stopDo = false;

        //打字结束后等待输入，此时小箭头弹跳表示点击
        m_Message_MText.mAnimator.onTypeWriterFinished.AddListener(OnTypeWriterFinished);
    }

    protected void OnTypeWriterFinished()
    {
        m_Icon_Arrow_Bouncer.StartBounce();

        stopDo = true;//给内联余量，禁止操作
        MCoroutineManager.Instance.DelayNoRecord(() =>
        {
            stopDo = false;
        }, 0.5f);
    }

    public void RefreshView(Sprite sprite, string name, string message, int id = -1)
    {
        m_ProfilePhoto_MImage.sprite = sprite;
        m_Name_MText.text = name;

        if (id != -1)
        {
            m_Message_MText.SetMText(id);
        }
        else
        {
            m_Message_MText.SetMText(message);
        }
    }

    public void StartDialog(Conversation conversation)
    {
        OpenSelf();
        currConversation = conversation;
        currSentence = 0;

        var sentence = conversation.sentences[currSentence];
        RefreshView(sentence.avatar, sentence.name, sentence.text, sentence.localID);

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
            if (stopDo) return;

            //正在打字，再次点击进行加速
            if (m_Message_MText.mAnimator.typewriterPlaying)
            {
                m_Message_MText.FinishTextImmediately();
                return;
            }

            currSentence++;
            if (currSentence == currConversation.sentences.Count)
            {
                EndDialog();
                m_Icon_Arrow_Bouncer.StopBounce();
                return;
            }

            var sentence = currConversation.sentences[currSentence];
            RefreshView(sentence.avatar, sentence.name, sentence.text, sentence.localID);
            m_Icon_Arrow_Bouncer.StopBounce();
        }
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtility.LoadPanelSync(prefabPath);
    }
}