using MFramework;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HurtEffect : MonoBehaviour
{
    [Header("Flash Settings")]
    public SkinnedMeshRenderer[] renderers;
    public string colorPropertyName = "_Color";
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
            var initialColor = renderer.material.GetColor(colorPropertyName);
            //受伤颜色--->原来的颜色
            MTween.DoTween01NoRecord((f) =>
            {
                var color = Color.Lerp(hurtColor, initialColor, f);
                renderer.material.SetColor(colorPropertyName, color);
            }, MCurve.Linear, flashDuration);
        }
    }
}
