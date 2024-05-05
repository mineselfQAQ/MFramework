using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MFramework
{
    public abstract class CyclicScrollView<Cell, Data> : MonoBehaviour where Cell : MonoBehaviour
    {
        public CyclicScrollViewDirection direction;
        public ICollection<Data> datas { get; private set; }

        [SerializeField] protected Cell cellPrefab;//传入Prefab(必须带有Cell子类型的组件)
        [SerializeField] protected RectTransform content;//Scrollview中自带的Viewport下的Content

        [SerializeField] private RectTransform viewRange;//父物体(带有ScrollRect组件)的RectTransform
        [SerializeField] private Vector2 cellSpace;//Cell之间的间距
        [SerializeField] private int itemCellCount;//行/列中元素数(一个ViewCellBundle中有几个元素)

        private RectTransform cellRectTrans;

        private readonly Vector2 horizontalContentAnchorMin = new Vector2(0, 0);
        private readonly Vector2 horizontalContentAnchorMax = new Vector2(0, 1);
        private readonly Vector2 verticalContentAnchorMin = new Vector2(0, 1);
        private readonly Vector2 verticalContentAnchorMax = new Vector2(1, 1);

        private readonly Vector2 cellPivot = new Vector2(0, 1);
        private readonly Vector2 cellAnchorMin = new Vector2(0, 1);
        private readonly Vector2 cellAnchorMax = new Vector2(0, 1);

        private readonly LinkedList<CellBundle<Cell>> cellBundles = new LinkedList<CellBundle<Cell>>();
        private readonly Queue<CellBundle<Cell>> cellBundlePool = new Queue<CellBundle<Cell>>();

        public Vector2 ContentPos => content.position;
        public Vector2 ContentSize => content.sizeDelta;
        public Vector2 CellSize => cellRectTrans.sizeDelta;//一个Cell(本体)
        public Vector2 ItemSize => CellSize + cellSpace;//一个Cell(本体+间隔)

        //行数
        public int ItemCount
        {
            get
            {
                int cellCount = datas.Count;

                int itemCount = cellCount / itemCellCount;//一般情况
                if (cellCount % itemCellCount != 0)//行/列 不满情况(如10个元素，3个一排，放3排，还多1个)
                {
                    itemCount += 1;
                }

                return itemCount;
            }
        }

        public virtual void Init(ICollection<Data> datas, bool reset = false)
        {
            if (datas == null)
            {
                MLog.Print("没有数据用于初始化，请检查", MLogType.Error);
                return;
            }

            cellRectTrans = cellPrefab.GetComponent<RectTransform>();
            this.datas = datas;

            //重新计算Content的Size，其实就是根据当前实例个数更改"包围盒"的大小
            RecalculateContentSize(reset);//更新显示内容(屏幕中到底是哪些viewCellBundle还存在)

            //核心---刷新视图
            UpdateDisplay();
        }

        /// <summary>
        /// 计算每个元素的位置
        /// </summary>
        public void RecalculateContentSize(bool reset)
        {
            int itemCount = ItemCount;//行数列数，如果是100个元素每行放3个，那么就会有34行

            //更改锚点以及大小
            //大小---以垂直移动为例，横向x不动，竖向y为行数*高度-1份间隔
            if (direction == CyclicScrollViewDirection.Vertical)
            {
                //TODO:直接更改好像并不能改变原有的错误
                content.anchorMin = verticalContentAnchorMin;
                content.anchorMax = verticalContentAnchorMax;

                //Tip：ItemSize是本体+间隔，不要和CellSize搞混了
                //减一份间隔是因为：如3行，那么其实只有2份间隔而不是3份
                content.sizeDelta = new Vector2(content.sizeDelta.x, itemCount * ItemSize.y - cellSpace.y);
            }
            else if (direction == CyclicScrollViewDirection.Horizontal)
            {
                content.anchorMin = horizontalContentAnchorMin;
                content.anchorMax = horizontalContentAnchorMax;
                content.sizeDelta = new Vector2(itemCount * ItemSize.x - cellSpace.x, content.sizeDelta.y);
            }

            if (reset)
            {
                //重置Pos值(PosX/PosY/PosZ都为0，或者Left/Right/PosY/PosZ都为0)
                //也就是重置为默认状态
                content.anchoredPosition = Vector2.zero;
            }
        }

        /// <summary>
        /// 更新显示内容
        /// </summary>
        public void UpdateDisplay()
        {
            //RemoveHead();
            //RemoveTail();
            if (cellBundles.Count == 0)
            {
                RefreshAllCellInViewRange();//完全更新CellBundles
            }
            else
            {
                //AddHead();
                //AddTail();
            }
            //清除越界,比如数据减少,此时就要清理在视野内的,在数据之外的UI
            //RemoveItemOutOfListRange();
        }

        private void RefreshAllCellInViewRange()
        {
            int itemCount = ItemCount;//行数
            Vector2 viewRangeSize = viewRange.sizeDelta;//区域大小
            Vector2 itemSize = ItemSize;//Cell大小(完整)
            Vector2 cellSize = CellSize;//Cell大小(去除间隔)
            Vector2 cellSpace = this.cellSpace;//间隔

            if (direction == CyclicScrollViewDirection.Vertical)
            {
                //获取ScrollView的顶和底
                Vector2 topPos = -content.anchoredPosition;
                Vector2 bottomPos = new Vector2(topPos.x, topPos.y - viewRangeSize.y);

                //获取顶和底所在行数
                int startIndex = GetIndex(topPos);
                int endIndex = GetIndex(bottomPos);
                //for循环添加所有的bundle
                for (int i = startIndex; i <= endIndex && i < itemCount; i++)
                {
                    Vector2 pos = new Vector2(content.anchoredPosition.x, -i * itemSize.y);
                    var bundle = GetViewBundle(i, pos, cellSize, cellSpace);//每个bundle之间只有位置不同
                    cellBundles.AddLast(bundle);
                }
            }
            else if (direction == CyclicScrollViewDirection.Horizontal)//同上
            {
                Vector2 leftPos = -content.anchoredPosition;
                Vector2 rightPos = new Vector2(leftPos.x + viewRangeSize.x, leftPos.y);

                int startIndex = GetIndex(leftPos);
                int endIndex = GetIndex(rightPos);

                for (int i = startIndex; i <= endIndex && i < itemCount; i++)
                {
                    Vector2 pos = new Vector2(i * itemSize.x, content.anchoredPosition.y);
                    var bundle = GetViewBundle(i, pos, cellSize, cellSpace);
                    cellBundles.AddLast(bundle);
                }
            }
        }

        private CellBundle<Cell> GetViewBundle(int itemIndex, Vector2 postion, Vector2 cellSize, Vector2 cellSpace)
        {
            CellBundle<Cell> bundle;
            Vector2 cellOffset = default;

            //对于竖向模式，每个Bundle中的Cell是横向排布的，需要计算偏移值(物体宽度/横向间隔)
            if (direction == CyclicScrollViewDirection.Vertical)
            {
                cellOffset = new Vector2(cellSize.x + cellSpace.x, 0);
            }
            //同上
            else if (direction == CyclicScrollViewDirection.Horizontal)
            {
                cellOffset = new Vector2(0, -(cellSize.y + cellSpace.y));
            }

            if (cellBundlePool.Count == 0)//当初次执行时，进行初始化操作
            {
                //初始化
                bundle = new CellBundle<Cell>(itemCellCount);
                bundle.position = postion;
                bundle.index = itemIndex;
                //j---bundle中的index，i---元数据的index
                int i = itemIndex * itemCellCount, j = 0;
                int end = itemIndex * itemCellCount + bundle.Cells.Length;

                for (; j < bundle.Cells.Length && i < end; j++, i++)
                {
                    bundle.Cells[j] = Instantiate(cellPrefab, content);//核心---初始化
                    bundle.Cells[j].gameObject.SetActive(false);
                    RectTransform rectTransform = bundle.Cells[j].GetComponent<RectTransform>();
                    ResetRectTransform(rectTransform);
                    rectTransform.anchoredPosition = postion + j * cellOffset;

                    if (i < 0 || i >= datas.Count)
                    {
                        continue;
                    }

                    ResetCellData(bundle.Cells[j], datas.ElementAt(i), i);
                }
            }
            else//同上
            {
                bundle = cellBundlePool.Dequeue();
                bundle.position = postion;
                bundle.index = itemIndex;
                int i = itemIndex * itemCellCount, j = 0;
                int end = itemIndex * itemCellCount + bundle.Cells.Length;
                for (; j < bundle.Cells.Length && i < end; j++, i++)
                {
                    RectTransform rectTransform = bundle.Cells[j].GetComponent<RectTransform>();
                    ResetRectTransform(rectTransform);
                    rectTransform.anchoredPosition = postion + j * cellOffset;

                    if (i < 0 || i >= datas.Count)
                    {
                        continue;
                    }

                    ResetCellData(bundle.Cells[j], datas.ElementAt(i), i);
                }
            }
            return bundle;
        }

        protected abstract void ResetCellData(Cell cell, Data data, int dataIndex);

        private void ResetRectTransform(RectTransform rectTransform)
        {
            rectTransform.pivot = cellPivot;
            rectTransform.anchorMin = cellAnchorMin;
            rectTransform.anchorMax = cellAnchorMax;
        }

        private int GetIndex(Vector2 position)
        {
            int index = -1;
            if (direction == CyclicScrollViewDirection.Vertical)
            {
                index = Mathf.RoundToInt(-position.y / ItemSize.y);
                return index;
            }
            else if (direction == CyclicScrollViewDirection.Horizontal)
            {
                index = Mathf.RoundToInt(position.x / ItemSize.x);
            }
            return index;
        }
    }
}