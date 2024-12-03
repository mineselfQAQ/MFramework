using UnityEngine;
using UnityEngine.LowLevel;

public class Test1 : MonoBehaviour
{
    private Renderer _renderer;
    private MaterialPropertyBlock _mpb;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();

        // …Ë÷√—’…´ Ù–‘
        _mpb.SetColor("_Color", Color.red);
        _renderer.SetPropertyBlock(_mpb);
    }

    void Update()
    {
    }
}