public class Heart : Collectable
{
    protected override void OnCollect(Player player)
    {
        AddHealth(player, 1);
    }

    public void AddHealth(Player player, int amount)
    {
        player.health.Increase(amount);
    }
}
