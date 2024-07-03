using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public MText text;
    public Button btn;

    private void Start()
    {
        btn.onClick.AddListener(() =>
        {
            text.FinishTextImmediately();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            text.PlayText();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            text.PlayTextFastly();
        }
    }
}