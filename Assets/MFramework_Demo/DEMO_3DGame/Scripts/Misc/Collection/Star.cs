using UnityEngine;

public class Star : Collectable
{
    [Header("Star Settings")]
    public int index = -1;

    protected LevelScore m_score => LevelScore.Instance;

    protected override void Awake()
    {
        base.Awake();

        m_score.OnScoreLoaded.AddListener(() =>
        {
            //Òþ²ØÐÇ
            if (m_score.stars[index])
            {
                gameObject.SetActive(false);
            }
        });
    }

    protected override void OnCollect(Player player)
    {
        m_score.CollectStar(index);
    }
}
