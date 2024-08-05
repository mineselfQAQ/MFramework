using UnityEngine;

public class Coin : Collectable
{
    [Header("Coin Settings")]
    public int amount = 1;

    protected LevelScore m_score => LevelScore.Instance;

    protected override void OnCollect(Player player)
    {
        AddCoins(amount);
    }

    public virtual void AddCoins(int amount) 
    {
        m_score.coins += amount;
    } 
}
