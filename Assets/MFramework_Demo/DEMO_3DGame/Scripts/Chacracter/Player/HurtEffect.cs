using MFramework;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HurtEffect : MonoBehaviour
{
    [Header("Flash Settings")]
    public SkinnedMeshRenderer[] renderers;
    public Color flashColor = Color.red;
    public float flashDuration = 0.5f;

    protected Health m_health;

    protected virtual void Start()
    {
        m_health = GetComponent<Health>();
        m_health.onDamage.AddListener(Hurt);
    }

    public virtual void Hurt()
    {
        foreach (var renderer in renderers)
        {
            var hurtColor = this.flashColor;
            var initialColor = renderer.material.color;
            //受伤颜色--->原来的颜色
            MTween.DoTween01NoRecord((f) =>
            {
                renderer.material.color = Color.Lerp(hurtColor, initialColor, f);
            }, MCurve.Linear, flashDuration);
        }
    }
}
