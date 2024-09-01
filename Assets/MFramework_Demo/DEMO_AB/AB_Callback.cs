using MFramework;
using UnityEngine;

public class AB_Callback : MonoBehaviour
{
    private void Start()
    {
        ResourceManager.Instance.Initialize(MABUtility.GetPlatform(), AB.GetFileUrl, 0);

        ResourceManager.Instance.LoadWithCallback("Assets/AssetBundle/UI/UIRoot.prefab", true, uiRootResource =>
        {
            uiRootResource.Instantiate();

            Transform uiParent = GameObject.Find("Canvas").transform;
            ResourceManager.Instance.LoadWithCallback("Assets/AssetBundle/UI/TestUI.prefab", true, testUIResource =>
            {
                testUIResource.Instantiate(uiParent, false);
            });
        });
    }
    private void Update()
    {
        ResourceManager.Instance.Update();
    }

    private void LateUpdate()
    {
        ResourceManager.Instance.LateUpdate();
    }
}
