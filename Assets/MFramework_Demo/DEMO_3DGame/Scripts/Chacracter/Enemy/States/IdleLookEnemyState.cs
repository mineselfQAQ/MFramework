using MFramework;
using UnityEngine;

public class IdleLookEnemyState : EnemyState
{
    protected Quaternion to;

    protected override void OnEnter(Enemy enemy)
    {
        to = enemy.transform.rotation;
    }

    protected override void OnStep(Enemy enemy)
    {
        enemy.Gravity();
        enemy.SnapToGround();
        enemy.Friction();
        enemy.LookPlayer(enemy.stats.current.smoothRotate);
    }

    protected override void OnExit(Enemy enemy) 
    {
        var from = enemy.transform.rotation;
        MTween.DoTween01NoRecord((f) =>
        {
            Quaternion rotation = Quaternion.Lerp(from, to, f);
            enemy.transform.rotation = rotation;
        }, MCurve.Linear, enemy.stats.current.restoreOrientationDuration);
    }

    public override void OnContact(Enemy enemy, Collider other) { }
}
