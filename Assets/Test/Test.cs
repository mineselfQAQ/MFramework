using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Button btn;

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(() => {
            Debug.Log(1);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
