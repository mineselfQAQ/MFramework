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
        Vector3 vec = destination - enemy.position;//���˵�Ŀ�������

        float distance = vec.magnitude;
        Vector3 direction = vec / distance;

        //����Ŀ�ĵأ��л�����һĿ���
        if (distance <= enemy.stats.current.waypointMinDistance)
        {
            enemy.Decelerate();
            enemy.waypoints.Next();
        }
        else//��Ŀ���ǰ��
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
