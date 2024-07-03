using MFramework;
using MFramework.UI;
using UnityEngine;

public class TypewriterDEMO : MonoBehaviour
{
    public MButton bottonBtn;
    public MButton replayBtn;
    public MButton fastReplayBtn;

    private GameObject uiCanvas;
    private MTextAnimator[] animators;

    private void Awake()
    {
        uiCanvas = GameObject.Find(MBuildSettings.uiCanvasName);
        animators = uiCanvas.GetComponentsInChildren<MTextAnimator>();
    }

    private void Start()
    {
        replayBtn.onClick.AddListener(() =>
        {
            foreach (var animator in animators)
            {
                animator.PlayText();
            }
        });
        fastReplayBtn.onClick.AddListener(() =>
        {
            foreach (var animator in animators)
            {
                animator.PlayTextFastly();
            }
        });
        bottonBtn.onClick.AddListener(() =>
        {
            foreach (var animator in animators)
            {
                animator.FinishTextImmediately();
            }
        }); 
    }

    private void Update()
    {
        BindPlayText(KeyCode.Alpha1, "Show");
        BindPlayText(KeyCode.Alpha2, "Fade");
        BindPlayText(KeyCode.Alpha3, "Scale");
        BindPlayText(KeyCode.Alpha4, "Translation");
        BindPlayText(KeyCode.Alpha5, "Rotation");
        BindPlayText(KeyCode.Alpha6, "Wave");
        BindPlayText(KeyCode.Alpha7, "Shake");

        //if (animators[0].mAnim.curTypeWriterFinishIndex == -1) Debug.Log("-1");
        //Debug.Log(animators[0].mAnim.curTypeWriterFinishIndex);
        //Debug.Log(animators[0].mAnim.curTypeWriterStartIndex);
        //Debug.Log(animators[0].mAnim.fastFinish);
    }

    private void BindPlayText(KeyCode keyCode, string name)
    {
        if (Input.GetKeyDown(keyCode))
        {
            foreach (var animator in animators)
            {
                if (animator.name == name) animator.PlayText();
            }
        }
    }
}
