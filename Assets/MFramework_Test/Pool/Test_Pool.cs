using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Test_Pool : MonoBehaviour
{
    public GameObject cubePrefab;
    public Transform parent;

    public List<GameObject> cubeInstance = new List<GameObject>();

    private void Start()
    {
        MPoolManager.WarmPool(cubePrefab, 10, parent, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            cubeInstance.Add(MPoolManager.SpawnObject(cubePrefab));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            int last = cubeInstance.Count - 1;
            if (last < 0) return;
            MPoolManager.ReleaseObject(cubeInstance[last]);
            cubeInstance.RemoveAt(last);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            var go = MPoolManager.ReleaseObject(cubePrefab, false);
            if (go != null)
            {
                cubeInstance.Remove(go);
            }
        }
    }
}
