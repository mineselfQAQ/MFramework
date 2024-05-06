using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// Cell集合，其中存放着一行/一列Cell
    /// </summary>
    public class CellBundle<Cell> : IPoolObject where Cell : MonoBehaviour
    {
        internal int index;
        internal Vector2 position;

        internal Cell[] Cells { get; private set; }
        internal int CellCapacity => Cells.Length;

        internal CellBundle(int capacity)

        {
            Cells = new Cell[capacity];
        }

        /// <summary>
        /// 该Bundle中的全部元素禁用
        /// </summary>
        public void Clear()
        {
            index = -1;
            foreach (var cell in Cells)
            {
                if (cell != null)
                {
                    cell.gameObject.SetActive(false);
                }
            }
        }
    }
}