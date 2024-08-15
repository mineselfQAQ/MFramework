using UnityEngine;

public class IdleEnemyState : EnemyState
{
    protected override void OnEnter(Enemy enemy) { }

    protected override void OnStep(Enemy enemy)
    {
        enemy.Gravity();
        enemy.SnapToGround();
        enemy.Friction();
    }

    protected override void OnExit(Enemy enemy) { }

    public override void OnContact(Enemy enemy, Collider other) { }
}
