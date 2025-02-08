using UnityEngine;

public static class JPSTool
{
    public static Grid IsJumpPoint(Grid[,] gridMap, Grid preGrid, Grid grid, int rowDir, int colDir)
    {
        if (IsInvalid(grid)) return null;

        //1.如果为起点或终点，则为跳点
        if (grid.isStartGrid || grid.isEndGrid)
        {
            return grid;
        }
        //2.具有强迫邻居，则为跳点
        if (HasForceNeighbour(gridMap, preGrid, grid))
        {
            return grid;
        }

        //3.斜向搜索时垂直方向具有1/2情况，则为跳点
        if (rowDir == 0 || colDir == 0) return null;//斜向
        Grid jumpGrid = JumpSearchHV(gridMap, grid, rowDir, 0);
        if (jumpGrid != null)
        {
            return jumpGrid;
        }
        jumpGrid = JumpSearchHV(gridMap, grid, 0, colDir);
        return jumpGrid;
    }
    private static bool HasForceNeighbour(Grid[,] gridMap, Grid preGrid, Grid grid)
    {
        if (preGrid == null || grid == null) return false;
        if (grid.type == GridType.Obstacle) return false;

        bool result1, result2;
        int dirX = Dir(grid.Pos.x, preGrid.Pos.x);
        int dirY = Dir(grid.Pos.y, preGrid.Pos.y);
        if (dirX == 0 || dirY == 0)
        {
            result1 = CheckHVForceNeighbour(gridMap, grid, dirX, dirY, 1);
            result2 = CheckHVForceNeighbour(gridMap, grid, dirX, dirY, -1);
        }
        else
        {
            result1 = CheckDiagonalForceNeighbour(gridMap, grid, dirX, dirY, 1);
            result2 = CheckDiagonalForceNeighbour(gridMap, grid, dirX, dirY, -1);
        }
        return result1 || result2;
    }
    private static bool CheckHVForceNeighbour(Grid[,] gridMap, Grid grid, int dirX, int dirY, int sign)
    {
        int obstacleGridPosX = grid.Pos.x + Mathf.Abs(dirY) * sign;
        int obstacleGridPosY = grid.Pos.y + Mathf.Abs(dirX) * sign;
        int neighbourPosX = obstacleGridPosX + dirX;
        int neighbourPosY = obstacleGridPosY + dirY;

        Grid obstacleGrid = gridMap[obstacleGridPosX, obstacleGridPosY];
        Grid neighbourGrid = gridMap[neighbourPosX, neighbourPosY];

        if (neighbourGrid == null || neighbourGrid.type == GridType.Obstacle)
        {
            return false;
        }
        if (obstacleGrid == null || obstacleGrid.type == GridType.Obstacle)
        {
            grid.forceNeighbour.Add(neighbourGrid);
            return true;
        }

        return false;
    }
    private static bool CheckDiagonalForceNeighbour(Grid[,] gridMap, Grid grid, int dirX, int dirY, int sign)
    {
        int obstacleDirX = 0;
        int obstacleDirY = 0;
        int neighbourDirX = 0;
        int neighbourDirY = 0;
        if (sign == 1)
        {
            obstacleDirX = dirX;
            neighbourDirX = dirX;
        }
        else
        {
            obstacleDirY = dirY;
            neighbourDirY = dirY;
        }

        int obstacleGridPosX = (grid.Pos.x - dirX) + obstacleDirX;
        int obstacleGridPosY = (grid.Pos.y - dirY) + obstacleDirY;
        int neighbourPosX = obstacleGridPosX + neighbourDirX;
        int neighbourPosY = obstacleGridPosY + neighbourDirY;

        Grid obstacleGrid = gridMap[obstacleGridPosX, obstacleGridPosY];
        Grid neighbourGrid = gridMap[neighbourPosX, neighbourPosY];

        if (neighbourGrid == null || neighbourGrid.type == GridType.Obstacle)
        {
            return false;
        }
        if (obstacleGrid == null || obstacleGrid.type == GridType.Obstacle)
        {
            grid.forceNeighbour.Add(neighbourGrid);
            return true;
        }

        return false;
    }
    private static Grid JumpSearchHV(Grid[,] gridMap, Grid grid, int rowDir, int colDir)
    {
        if (rowDir != 0 && colDir != 0)
        {
            return null;
        }
        int row = grid.Pos.x;
        int col = grid.Pos.y;
        while (true)
        {
            row += rowDir;
            col += colDir;
            Grid temp = gridMap[row, col];
            if (IsInvalid(temp)) break;

            if (IsJumpPoint(gridMap, grid, temp, rowDir, colDir) != null)
            {
                return temp;
            }
        }
        return null;
    }

    public static bool IsInvalid(Grid grid)
    {
        if (grid.type == GridType.Obstacle)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 搜索方向
    /// </summary>
    public static int Dir(int v1, int v2)
    {
        int value = v1 - v2;
        if (value > 0)
        {
            return 1;
        }
        else if (value == 0)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }
}
