using UnityEngine;

public class Log : MActivator
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
