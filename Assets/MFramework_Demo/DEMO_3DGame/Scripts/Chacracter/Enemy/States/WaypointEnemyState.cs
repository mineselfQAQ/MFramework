using UnityEngine;

public class WaypointEnemyState : EnemyState
{
    protected override void OnEnter(Enemy enemy) { }

    protected override void OnStep(Enemy enemy)
    {
        enemy.Gravity();
        enemy.SnapToGround();

        Vector3 destination = enemy.waypoints.current.position;
        destination = new Vector3(destination.x, enemy.position.y, destination.z);
        Vector3 vec = destination - enemy.position;//敌人到目标点向量

        float distance = vec.magnitude;
        Vector3 direction = vec / distance;

        //到达目的地，切换至下一目标点
        if (distance <= enemy.stats.current.waypointMinDistance)
        {
            enemy.Decelerate();
            enemy.waypoints.Next();
        }
        else//向目标点前进
        {
            enemy.Accelerate(direction, enemy.stats.current.waypointAcceleration, enemy.stats.current.waypointTopSpeed);

            if (enemy.stats.current.faceWaypoint)
            {
                enemy.FaceDirectionSmooth(direction);
            }
        }
    }

    protected override void OnExit(Enemy enemy) { }

    public override void OnContact(Enemy enemy, Collider other) { }
}
