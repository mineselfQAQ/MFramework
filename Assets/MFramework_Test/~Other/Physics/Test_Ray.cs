using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Ray : MonoBehaviour
{
    public Vector3 dir = Vector3.right;

    private void Start()
    {
        dir = dir.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(Vector3.zero, dir * 100);
    }
}
