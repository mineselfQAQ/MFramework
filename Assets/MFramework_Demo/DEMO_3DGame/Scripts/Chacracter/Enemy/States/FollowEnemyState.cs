using UnityEngine;

public class FollowEnemyState : EnemyState
{
    protected override void OnEnter(Enemy enemy) { }

    protected override void OnStep(Enemy enemy)
    {
        enemy.Gravity();
        enemy.SnapToGround();

        var head = enemy.player.position - enemy.position;
        var direction = new Vector3(head.x, 0, head.z).normalized;

        enemy.Accelerate(direction, enemy.stats.current.followAcceleration, enemy.stats.current.followTopSpeed);
        enemy.FaceDirectionSmooth(direction);
    }

    protected override void OnExit(Enemy enemy) { }

    public override void OnContact(Enemy enemy, Collider other) { }
}
