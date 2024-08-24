using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:可以在某些地方设置人物不启用
public class AirWall : MonoBehaviour
{
    public Collider playerCollider; // Drag the Player's collider into this field in the Inspector.

    void Start()
    {
        Collider airWallCollider = GetComponent<Collider>();

        // 忽略空气墙与其他非Player对象之间的碰撞
        foreach (Collider otherCollider in FindObjectsOfType<Collider>())
        {
            if (otherCollider != playerCollider)
            {
                Physics.IgnoreCollision(airWallCollider, otherCollider);
            }
        }
    }
}
