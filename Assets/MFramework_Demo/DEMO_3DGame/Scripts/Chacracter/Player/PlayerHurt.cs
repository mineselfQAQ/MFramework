using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerHurt : MonoBehaviour
{
    [Header("Flash Settings")]
    public SkinnedMeshRenderer[] renderers;
    public Color flashColor = Color.red;
    public float flashDuration = 0.5f;

    protected Health m_health;

    public virtual void Hurt()
    {
        foreach (var renderer in renderers)
        {
            StartCoroutine(HurtRoutine(renderer.material));
        }
    }

    protected virtual IEnumerator HurtRoutine(Material material)
    {
        var elapsedTime = 0f;
        var hurtColor = this.flashColor;//受伤后的颜色
        var initialColor = material.color;//原始颜色

        //在一定时间内由受伤颜色变回原来的颜色
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            material.color = Color.Lerp(hurtColor, initialColor, elapsedTime / flashDuration);
            yield return null;
        }

        material.color = initialColor;
    }

    protected virtual void Start()
    {
        m_health = GetComponent<Health>();
        m_health.onDamage.AddListener(Hurt);
    }
}
