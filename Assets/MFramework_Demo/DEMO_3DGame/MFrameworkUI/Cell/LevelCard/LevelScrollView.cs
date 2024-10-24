using MFramework;
using System.Collections.Generic;

public class LevelScrollView : CyclicScrollView<LevelCell, GameLevel>
{
    protected bool locked;

    public string scene { get; set; }
    
    protected override void ResetCellData(LevelCell cell, GameLevel level, int dataIndex)
    {
        cell.gameObject.SetActive(true);
        cell.UpdateView(level);
    }

    public void InitView(List<GameLevel> levels)
    {
        Init(levels);
    }
    public void RefreshView()
    {
        Refresh();
    }
}
