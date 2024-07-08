using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Space space;
	public Vector3 eulers = new Vector3(0, -180, 0);

	protected virtual void LateUpdate()
	{
		transform.Rotate(eulers * Time.deltaTime, space);
	}
}
