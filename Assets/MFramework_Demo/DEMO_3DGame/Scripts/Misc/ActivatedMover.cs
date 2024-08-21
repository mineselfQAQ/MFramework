using UnityEngine;

public class ActivatedMover : Mover
{
    [Header("ActivatedMover Settings")]
    public int requestCount = 1;

    protected int activatedNum = 0;

    public int ActivatedNum
    {
        get { return activatedNum; }
        set
        {
            activatedNum = value;

            if (activatedNum == requestCount)
            {
                Apply();
            }
        }
    }

    public void Add()
    {
        ActivatedNum++;
    }
}
