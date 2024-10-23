using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Test_Pool : MonoBehaviour
{
    public GameObject cube;
    public Transform parent;

    public List<GameObject> cubeInstance = new List<GameObject>();

    private void Start()
    {
        MPoolManager.WarmPool(cube, 10, parent, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            cubeInstance.Add(MPoolManager.SpawnObject(cube));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            int last = cubeInstance.Count - 1;
            if (last < 0) return;
            MPoolManager.ReleaseObject(cubeInstance[last]);
            cubeInstance.RemoveAt(last);
        }
    }
}
