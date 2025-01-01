using MFramework;
using System.Collections.Generic;
using UnityEngine;

public class Test_Pool : MonoBehaviour
{
    public GameObject cubePrefab;
    public Transform parent;

    public List<ObjectPoolContainer<GameObject>> cubeInstance = new List<ObjectPoolContainer<GameObject>>();

    private void Start()
    {
        MPoolManager.Instance.WarmPool(cubePrefab, parent, 10);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            cubeInstance.Add(MPoolManager.Instance.SpawnObject(cubePrefab));
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            int last = cubeInstance.Count - 1;
            if (last < 0) return;
            MPoolManager.Instance.ReleaseObject(cubeInstance[last]);
            cubeInstance.RemoveAt(last);
        }
    }
}
