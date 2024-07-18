using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    private void Start()
    {

    }

    void Update()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        float radius = 0.5f;
        float maxDistance = 1f;

        if (Physics.SphereCast(origin, radius, Vector3.down, out hit, maxDistance))
        {
            Debug.Log("Hit " + hit.point);
            // �ڳ����л���SphereCast�Ĺ켣
            Debug.DrawLine(origin, hit.point, Color.red);
        }
    }
}