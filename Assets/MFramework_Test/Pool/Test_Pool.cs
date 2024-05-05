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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            for (int i = 0; i < 10; i++)
            {
                cubeInstance.Add(PoolManager.SpawnObject(cube));
            }
            for (int i = 0; i < 10; i++)
            {
                PoolManager.ReleaseObject(cubeInstance[i]);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PoolManager.SpawnObject(cube);
        }
    }
}
