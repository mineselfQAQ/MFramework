using MFramework.UI;
using UnityEngine;

public class InlineEffectDEMO : MonoBehaviour
{
    public MButton fadeInBtn;
    public MButton fadeOutBtn;

    private void Start()
    {
        fadeInBtn.onClick.AddListener(() =>
        {
            GameObject.Find("BlackIn").GetComponent<MText>().ApplyEffects();
        });
        fadeOutBtn.onClick.AddListener(() =>
        {
            GameObject.Find("BlackOut").GetComponent<MText>().ApplyEffects();
        });
    }
}
