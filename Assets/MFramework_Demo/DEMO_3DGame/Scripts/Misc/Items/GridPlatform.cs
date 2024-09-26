using MFramework;
using UnityEngine;

public class GridPlatform : MonoBehaviour
{
    public float rotationDuration = 0.5f;

    protected Transform platform;
    protected bool m_clockwise = true;

    protected virtual void Start()
    {
        platform = transform.Find("Platform");
        Level.Instance.player.playerEvents.OnJump.AddListener(Rotate);
    }

    public virtual void Rotate()
    {
        var from = platform.localRotation;
        var to = Quaternion.Euler(0, 0, m_clockwise ? 180 : 0);

        MTween.UnscaledDoTween01NoRecord((f) =>
        {
            platform.localRotation = Quaternion.Lerp(from, to, f);
        }, MCurve.Linear, rotationDuration);

        m_clockwise = !m_clockwise;
    }
}
