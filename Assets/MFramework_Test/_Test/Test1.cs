using MFramework;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public Image img;
    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(img, parent);
    }
}
