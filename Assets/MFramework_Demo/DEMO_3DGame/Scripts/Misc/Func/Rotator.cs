using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Space space;
	public Vector3 eulers = new Vector3(0, -180, 0);
    public bool startDo = true;

	protected bool canUpdate;

	protected virtual void Start()
	{
		canUpdate = startDo ? true : false;
	}

    protected virtual void LateUpdate()
	{
		if (canUpdate)
        {
            transform.Rotate(eulers * Time.deltaTime, space);
        }
	}

	public void StartRotate()
	{
		canUpdate = true;
    }
    public void StopRotate()
    {
        canUpdate = false;
    }
}
