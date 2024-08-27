using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:������ĳЩ�ط��������ﲻ����
public class AirWall : MonoBehaviour
{
    public Collider playerCollider; // Drag the Player's collider into this field in the Inspector.

    void Start()
    {
        Collider airWallCollider = GetComponent<Collider>();

        // ���Կ���ǽ��������Player����֮�����ײ
        foreach (Collider otherCollider in FindObjectsOfType<Collider>())
        {
            if (otherCollider != playerCollider)
            {
                Physics.IgnoreCollision(airWallCollider, otherCollider);
            }
        }
    }
}
