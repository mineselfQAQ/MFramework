using UnityEngine;

public class Heart : Collectable
{
    protected override void OnCollectInternal(Player player)
    {
        AddHealth(player, 1);
    }

    public void AddHealth(Player player, int amount)
    {
        player.health.Increase(amount);
    }
}
