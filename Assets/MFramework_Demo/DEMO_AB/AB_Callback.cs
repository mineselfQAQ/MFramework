using MFramework;
using UnityEngine;

public class AB_Callback : MonoBehaviour
{
    private void Start()
    {
        MResourceManager.Instance.LoadWithCallback("Assets/AssetBundle/UI/UIRoot.prefab", true, uiRootResource =>
        {
            uiRootResource.Instantiate();

            Transform uiParent = GameObject.Find("Canvas").transform;
            MResourceManager.Instance.LoadWithCallback("Assets/AssetBundle/UI/TestUI.prefab", true, testUIResource =>
            {
                testUIResource.Instantiate(uiParent, false);
            });
        });
    }
}
