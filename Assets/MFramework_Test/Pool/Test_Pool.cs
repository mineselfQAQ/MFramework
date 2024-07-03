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
        Dictionary<GameObject, int> dic = new Dictionary<GameObject, int>();

        PoolManager.WarmPool(cube, 10, parent, true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            cubeInstance.Add(PoolManager.SpawnObject(cube));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            int last = cubeInstance.Count - 1;
            PoolManager.ReleaseObject(cubeInstance[last]);
            cubeInstance.RemoveAt(last);
        }
    }
}
