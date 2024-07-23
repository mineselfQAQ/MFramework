using UnityEngine;

public class DiePlayerState : PlayerState
{
    protected override void OnEnter(Player player) { }

    protected override void OnStep(Player player)
    {
        player.Gravity();
        player.Friction();
        player.SnapToGround();
    }

    protected override void OnExit(Player player) { }

    public override void OnContact(Player player, Collider other) { }
}