using UnityEngine;

public class Life : Collectable
{
    [Header("Life Settings")]
    public int amount = 1;

    protected Game m_game => Game.Instance;

    protected override void OnCollect(Player player)
    {
        AddRetries(amount);
    }

    public virtual void AddRetries(int amount)
    {
        m_game.retries += amount;
    } 
}
