using MFramework;
using MFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public Transform t;
    private void Start()
    {
        transform.SinScaleLoopNoRecord(MCurve.QuartInOut);
    }
}