using UnityEngine;
using UnityEngine.LowLevel;

public class Test1 : MonoBehaviour
{
    public Transform trans;
    private Rigidbody body;

    protected void Awake()
    {
        var playerLoop = PlayerLoop.GetDefaultPlayerLoop();

        body = trans.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        trans.position = new Vector3(trans.position.x + Time.deltaTime, trans.position.y, trans.position.z);
    }
}