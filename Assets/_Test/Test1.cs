using UnityEngine;
using UnityEngine.LowLevel;

public class Test1 : MonoBehaviour
{
    public RectTransform trans;

    protected void Awake()
    {
        Debug.Log(trans.anchoredPosition);
        Debug.Log(trans.offsetMin);
        Debug.Log(trans.offsetMax);
    }

    private void Update()
    {

    }
}