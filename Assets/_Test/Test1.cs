using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public Transform player;
    private void Start()
    {

    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log(player.position);
            player.position = Vector3.zero;
            Debug.Log(player.position);
        }
    }
}