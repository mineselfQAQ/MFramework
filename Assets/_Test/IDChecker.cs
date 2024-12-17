using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IDChecker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var graphic = GetComponent<Graphic>();
        if (graphic != null && graphic.material != null)
        {
            // 获取 MaterialID
            int materialID = graphic.material.GetInstanceID();
            Debug.Log($"{graphic.material.name}");
            Debug.Log($"{name} Material ID:{materialID}");
            
            // 获取 TextureID
            if (graphic.mainTexture != null)
            {
                int textureID = graphic.mainTexture.GetInstanceID();
                Debug.Log($"{name} Texture ID:{textureID}");
            }
        }
    }
}