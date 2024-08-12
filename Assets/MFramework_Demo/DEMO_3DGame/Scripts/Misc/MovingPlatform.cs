using UnityEngine;

//TODO:BUG：在板上或趴板时，不会随着平台运动而运动(半解决，目前方法：判断在板上后对人物进行同样的移动)
[RequireComponent(typeof(Waypoint))]
[RequireComponent(typeof(Collider))]
public class MovingPlatform : MonoBehaviour
{
    public float speed = 3f;

    public Waypoint waypoints { get; protected set; }

    protected Level m_level => Level.Instance;

    protected virtual void Awake()
    {
        tag = GameTags.Platform;
        waypoints = GetComponent<Waypoint>();
    }

    protected virtual void Update()
    {
        Vector3 oldPosition = transform.position;
        Vector3 target = waypoints.current.position;
        Vector3 newPosition = Vector3.MoveTowards(oldPosition, target, speed * Time.deltaTime);
        transform.position = newPosition;

        //Player上板后的偏移量
        //TODO:应该有更好的方法
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
