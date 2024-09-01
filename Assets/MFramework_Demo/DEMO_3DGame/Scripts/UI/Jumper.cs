using MFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jumper : MonoBehaviour
{
    private void Start()
    {
        MCoroutineManager.Instance.DelayNoRecord(() =>
        {
            ResourceManager.Instance.Load($"{ABPath.ABROOTPATH}/3DGame_TitleScreen.unity", false);//**¼ÓÔØ½øÄÚ´æ**
            SceneManager.LoadScene(UIController.titleScreenSceneName);
        }, 1f);
    }
}
