using MFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    public bool startDo = true;

    public Vector3 direction = new Vector3(0, 1, 0);
    public float frequency = 1.0f;
    public float distance = 50.0f;
    public float stopAccelerateMultipler = 1.0f;

    protected Vector3 initPos;
    protected bool canUpdate;
    protected float x = 0.0f;

    protected bool isUI;
    protected RectTransform rectTransform;

    protected virtual void Start() 
    {
        canUpdate = startDo ? true : false;
        direction.Normalize();

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform)
        {
            isUI = true;
            initPos = rectTransform.anchoredPosition3D;
        }
        else
        {
            isUI = false;
            initPos = transform.position;
        }
    }

    protected virtual void LateUpdate()
    {
        if (canUpdate)
        {
            UpdatePos(false);
        }
    }

    protected void UpdatePos(bool isStop)
    {
        if (!isStop) x += Time.unscaledDeltaTime;
        else x += Time.unscaledDeltaTime * stopAccelerateMultipler;

        if (!isUI)
        {
            transform.position = initPos + direction * Bounce(x, frequency) * distance;
        }
        else
        {
            rectTransform.anchoredPosition3D = initPos + direction * Bounce(x, frequency) * distance;
        }
    }

    protected float Bounce(float x, float frequency = 1f)
    {
        return Mathf.Abs(Mathf.Sin(Mathf.PI * x * frequency));
    }

    public void StartBounce()
    {
        canUpdate = true;
        x = 0.0f;
    }

    public void StopBounce()
    {
        canUpdate = false;
        StartCoroutine(StopBounceRoutine());
    }

    protected IEnumerator StopBounceRoutine()
    {
        float period = 1 / frequency;
        float time = period - (x % period);
        float endX = x + time;

        while (true)
        {
            if (x > endX)
            {
                yield break;
            }

            UpdatePos(true);

            yield return null;
        }
    }
}
