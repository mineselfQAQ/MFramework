using UnityEngine;
using UnityEngine.UI;

public class Test11 : MonoBehaviour
{
    public Button btn;
    public int i;

    void Start()
    {
        i = 0;
        btn.onClick.AddListener(() => 
        {
            Debug.Log(i);
        });
        i++;
    }

    void Update()
    {

    }
}