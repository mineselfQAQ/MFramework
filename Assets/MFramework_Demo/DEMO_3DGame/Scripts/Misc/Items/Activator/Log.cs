using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class Log : Activator
{
    [Header("Log Settings")]
    public GameObject hideObj;

    protected Mover mover => GetComponent<Mover>();

    public override void OnActivateInternal()
    {
        mover.Apply();
        if(hideObj) hideObj.SetActive(true);
    }

    public override void OnDeactivateInternal()
    {

    }
}
