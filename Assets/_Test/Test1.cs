using MFramework;
using MFramework.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public List<Texture2D> dir;
    public List<Texture2D> light;
    protected LightmapData[] lightmaps;

    private void Start()
    {
        lightmaps = new LightmapData[dir.Count];
        for (int i = 0; i < lightmaps.Length; i++)
        {
            LightmapData lightmapData = new LightmapData();
            lightmapData.lightmapColor = light[i];
            lightmapData.lightmapDir = dir[i];

            lightmaps[i] = lightmapData;
        }
        LightmapSettings.lightmaps = lightmaps;
        Debug.Log("OK");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            Debug.Log(LightmapSettings.lightmaps.Length);
        if (Input.GetKeyDown(KeyCode.L))
            LightmapSettings.lightmaps = null;
        if (Input.GetKeyDown(KeyCode.M))
            LightmapSettings.lightmaps = lightmaps;
    }
}