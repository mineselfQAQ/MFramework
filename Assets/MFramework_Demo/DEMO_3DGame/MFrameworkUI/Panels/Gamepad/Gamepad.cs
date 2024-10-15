using UnityEngine;
using UnityEngine.UI;

public class Gamepad : GamepadBase
{
    public override void Init()
    {
        CheckEnable();
    }

    protected virtual void CheckEnable()
    {
#if UNITY_IOS || UNITY_ANDROID
        EnableRig(true);
#else
		EnableRig(false);
#endif
    }

    public virtual void EnableRig(bool value)
    {
        foreach (Transform t in m_Gamepad_RectTransform)
        {
            t.gameObject.SetActive(value);
        }
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return ABUtitlity.LoadPanelSync(prefabPath);
    }
}