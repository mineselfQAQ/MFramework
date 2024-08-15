using UnityEngine;

[RequireComponent(typeof(Waypoint))]
[RequireComponent(typeof(Collider))]
public class MovingPlatform : MonoBehaviour
{
    public float speed = 3f;

    public Waypoint waypoints { get; protected set; }

    protected Level m_level => Level.Instance;

    protected virtual void Awake()
    {
        tag = GameTags.MovingPlatform;
        waypoints = GetComponent<Waypoint>();
    }

    protected virtual void Update()
    {
        Vector3 oldPosition = transform.position;
        Vector3 target = waypoints.current.position;
        Vector3 newPosition = Vector3.MoveTowards(oldPosition, target, speed * Time.deltaTime);
        transform.position = newPosition;

        //Player上板后的偏移量
        if (m_level.player.onPlatform)
        {
            Vector3 offset = newPosition - oldPosition;
            m_level.player.controller.Move(offset);
        }

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            waypoints.Next();
        }
    }
}
