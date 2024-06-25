using MFramework.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework
{
    public abstract class CyclicScrollView<Cell, Data> : MonoBehaviour where Cell : MonoBehaviour
    {
        public CyclicScrollViewDirection direction;
        public ICollection<Data> datas { get; private set; }

        [SerializeField] protected Cell cellPrefab;//Prefab(必须带有Cell子类型的组件)
        [SerializeField] private RectTransform viewRange;//ScrollView本体
        [SerializeField] protected RectTransform content;//ScrollView中的Content
        [SerializeField] private Vector2 cellSpace;//Cell之间的间距
        [SerializeField] private int itemCellCount;//Bundle内Cell个数

        private RectTransform cellRectTrans;
        private ScrollRect viewRangeScrollRect;

        private readonly Vector2 horizontalContentAnchorMin = new Vector2(0, 0);
        private readonly Vector2 horizontalContentAnchorMax = new Vector2(0, 1);
        private readonly Vector2 horizontalContentPivot = new Vector2(0, 0.5f);
        private readonly Vector2 verticalContentAnchorMin = new Vector2(0, 1);
        private readonly Vector2 verticalContentAnchorMax = new Vector2(1, 1);
        private readonly Vector2 verticalContentPivot = new Vector2(0.5f, 1);

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
        //元素个数
        public int CellCount => datas.Count;

        #region 子类操作
        /// <summary>
        /// 初始化操作
        /// </summary>
        protected virtual void Init(ICollection<Data> datas)
        {
            if (datas == null)
            {
                MLog.Print($"{typeof(CyclicScrollView<Cell,Data>)}没有数据用于初始化，请检查", MLogType.Warning);
                return;
            }

            cellRectTrans = cellPrefab.GetComponent<RectTransform>();
            viewRangeScrollRect = viewRange.GetComponent<ScrollRect>();
            this.datas = datas;

            //重新计算Content的Size，其实就是根据当前实例个数更改"包围盒"的大小
            RecalculateContentSize(true);//更新显示内容(屏幕中到底是哪些viewCellBundle还存在)

            //核心---刷新视图
            RefreshAllCellInViewRange();
        }

        /// <summary>
        /// 数据更新时手动刷新
        /// </summary>
        public void Refresh()
        {
            RecalculateContentSize();
            RefreshViewRangeData();
        }
        /// <summary>
        /// 数据更新时手动刷新
        /// </summary>
        public void Refresh(ICollection<Data> datas)
        {
            if (datas.Count == 0) return;
            this.datas = datas;

            RecalculateContentSize();
            RefreshViewRangeData();
        }

        protected virtual void Update()
        {
            if (datas == null) return;
            UpdateDisplay();
        }

        /// <summary>
        /// 瞬间移动到某处(取值范围[0,1])
        /// </summary>
        public void MoveTo(float x)
        {
            x = Mathf.Clamp01(x);

            if (direction == CyclicScrollViewDirection.Vertical)
            {
                float max = content.sizeDelta.y - viewRange.sizeDelta.y;
                if (max < 0) return;
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, max * x);
            }
            else if (direction == CyclicScrollViewDirection.Horizontal)
            {
                float max = content.sizeDelta.x - viewRange.sizeDelta.x;
                if (max < 0) return;
                content.anchoredPosition = new Vector2(-max * x, content.anchoredPosition.y);
            }
            viewRangeScrollRect.velocity = Vector2.zero;//防止更改位置后ScrollView还在滑动状态
        }
        /// <summary>
        /// 瞬间移动到某一Bundle(行/列)
        /// </summary>
        /// <param name="count"></param>
        public void MoveToBundle(int count)
        {
            //以垂直模式为例：
            //移动一行也就是移动ItemSize.y距离，只要知道"最大行数"即可
            //Tip："最大行数"并非最后一行，而是当该行置顶时，正好显示完所有元素
            if (direction == CyclicScrollViewDirection.Vertical)
            {
                int viewRangeMax = (int)(viewRange.sizeDelta.y / ItemSize.y) + 1;
                int max = ItemCount - viewRangeMax + 1;
                if (max <= 0) return;
                count = Mathf.Clamp(count, 0, max);//限制到最大行/列
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, ItemSize.y * (count - 1));
            }
            else if (direction == CyclicScrollViewDirection.Horizontal)
            {
                int viewRangeMax = (int)(viewRange.sizeDelta.x / ItemSize.x) + 1;
                int max = ItemCount - viewRangeMax + 1;
                if (max <= 0) return;
                count = Mathf.Clamp(count, 0, max);//限制到最大行/列

                content.anchoredPosition = new Vector2(-ItemSize.x * (count - 1), content.anchoredPosition.y);
            }
            viewRangeScrollRect.velocity = Vector2.zero;//防止更改位置后ScrollView还在滑动状态
        }
        /// <summary>
        /// 瞬间移动到某一个Cell所在Bundle(行/列)
        /// </summary>
        /// <param name="count"></param>
        public void MoveToCell(int count)
        {
            int bundleCount = count / itemCellCount;
            if (count % itemCellCount != 0) bundleCount++;

            MoveToBundle(bundleCount);
        }

        /// <summary>
        /// 更新Cell预制体
        /// </summary>
        protected abstract void ResetCellData(Cell cell, Data data, int dataIndex);
        #endregion

        #region 一级函数
        private void UpdateDisplay()
        {
            RemoveOutOfRangeBundles();
            if (cellBundles.Count == 0) RefreshAllCellInViewRange();
            AddInOfRangeBundles();
        }

        private void RemoveOutOfRangeBundles()
        {
            if (direction == CyclicScrollViewDirection.Vertical)
            {
                //越上界
                if (cellBundles.Count == 0) return;

                CellBundle<Cell> bundle = cellBundles.First.Value;
                while (AboveViewRange(bundle.position))
                {
                    ReleaseViewBundle(bundle);//移入对象池
                    cellBundles.RemoveFirst();

                    if (cellBundles.Count == 0) break;

                    bundle = cellBundles.First.Value;
                }
                //越下界
                if (cellBundles.Count == 0) return;

                bundle = cellBundles.Last.Value;
                while (UnderViewRange(bundle.position))
                {
                    ReleaseViewBundle(bundle);//移入对象池
                    cellBundles.RemoveLast();

                    if (cellBundles.Count == 0) break;

                    bundle = cellBundles.Last.Value;
                }
            }
            else if (direction == CyclicScrollViewDirection.Horizontal)
            {
                //越左界
                if (cellBundles.Count == 0) return;

                CellBundle<Cell> bundle = cellBundles.First.Value;
                while (InViewRangeLeft(bundle.position))
                {
                    ReleaseViewBundle(bundle);//移入对象池
                    cellBundles.RemoveFirst();

                    if (cellBundles.Count == 0) break;

                    bundle = cellBundles.First.Value;
                }
                //越右界
                if (cellBundles.Count == 0) return;

                bundle = cellBundles.Last.Value;
                while (InViewRangeRight(bundle.position))
                {
                    ReleaseViewBundle(bundle);//移入对象池
                    cellBundles.RemoveLast();

                    if (cellBundles.Count == 0) break;

                    bundle = cellBundles.Last.Value;
                }
            }
        }

        private void AddInOfRangeBundles()
        {
            AddHeadBundles();
            AddTailBundles();
        }
        private void AddHeadBundles()
        {
            if (cellBundles.Count == 0) return;

            //以头部元素向外计算出新头部的位置,计算该位置是否在显示区域，如果在显示区域则生成对应项目
            CellBundle<Cell> bundle = cellBundles.First.Value;

            Vector2 offset = default;
            if (direction == CyclicScrollViewDirection.Vertical)
                offset = new Vector2(0, ItemSize.y);
            else if (direction == CyclicScrollViewDirection.Horizontal)
                offset = new Vector2(-ItemSize.x, 0);

            Vector2 newHeadBundlePos = bundle.position + offset;//判断点，该处为当前越界位置

            while (OnViewRange(newHeadBundlePos))
            {
                int caculatedIndex = GetIndex(newHeadBundlePos);
                int index = bundle.index - 1;

                if (index < 0) break;
                if (caculatedIndex != index)//核验
                {
                    MLog.Print($"{typeof(CyclicScrollView<Cell, Data>)}：计算得出索引-<{caculatedIndex}>与实际计数索引-<{index}>两者索引值不相等，请检查", MLogType.Error);
                    break;
                } 

                bundle = GetViewBundle(index, newHeadBundlePos, CellSize, cellSpace);
                cellBundles.AddFirst(bundle);

                newHeadBundlePos = bundle.position + offset;
            }
        }
        private void AddTailBundles()
        {
            if (cellBundles.Count == 0) return;

            //以尾部元素向外计算出新头部的位置,计算该位置是否在显示区域，如果在显示区域则生成对应项目
            CellBundle<Cell> bundle = cellBundles.Last.Value;
            Vector2 offset = default;
            if (direction == CyclicScrollViewDirection.Vertical)
                offset = new Vector2(0, -ItemSize.y);
            else if (direction == CyclicScrollViewDirection.Horizontal)
                offset = new Vector2(ItemSize.x, 0);

            Vector2 newTailBundlePos = bundle.position + offset;

            while (OnViewRange(newTailBundlePos))
            {
                int caculatedIndex = GetIndex(newTailBundlePos);
                int index = bundle.index + 1;

                if (index >= ItemCount) break;
                if (caculatedIndex != index)//核验
                {
                    MLog.Print($"{typeof(CyclicScrollView<Cell, Data>)}：计算得出索引-<{caculatedIndex}>与实际计数索引-<{index}>两者索引值不相等，请检查", MLogType.Error);
                    break;
                }

                bundle = GetViewBundle(index, newTailBundlePos, CellSize, cellSpace);
                cellBundles.AddLast(bundle);

                newTailBundlePos = bundle.position + offset;
            }
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

        private void RefreshViewRangeData()
        {
            if (cellBundles.Count() == 0) return;

            bool flag = false;
            int count = 0;

            //遍历当前显示的所有Bundle
            foreach (var bundle in cellBundles)
            {
                if (flag == true) break;

                count++;
                int startIndex = bundle.index * itemCellCount;
                int endIndex = startIndex + bundle.Cells.Length - 1;

                //防止越界(如果是最后一行就会发生)
                //Tip：如果是最后一行，很有可能会发生5个空位只填入3个的情况，那么这会导致这一行的元素需要只显示3个，后2个需要隐藏
                //如果在一个靠上的位置，不存在最后一行的话，自然所有行/列都是满的，就没有这种情况会发生了
                if (endIndex >= datas.Count)
                {
                    flag = true;
                    endIndex = datas.Count - 1;
                }

                int i = startIndex, j = 0;
                for (; i <= endIndex && j < bundle.Cells.Length; i++, j++)
                {
                    ResetCellData(bundle.Cells[j], datas.ElementAt(i), i);//为每个Cell刷新
                }

                //如果是最后一行，需要将该行的剩余Cell隐藏
                if (flag)
                {
                    while (j < bundle.Cells.Length)
                    {
                        try
                        {
                            bundle.Cells[j++].gameObject.SetActive(false);
                        }
                        catch (System.Exception)
                        {
                            throw;
                        }
                    }
                }
            }

            //如果删除了最后几行，flag会变为true且下一轮直接退出，那么最后会剩下一些未处理Bundle，移除即可
            int remainCount = cellBundles.Count() - count;
            while (remainCount > 0)
            {
                remainCount--;
                ReleaseViewBundle(cellBundles.Last.Value);
                cellBundles.RemoveLast();
            }
        }

        private void RecalculateContentSize(bool reset = false)
        {
            int itemCount = ItemCount;//行数列数，如果是100个元素每行放3个，那么就会有34行

            //更改锚点以及大小
            //大小---以垂直移动为例，横向x不动，竖向y为行数*高度-1份间隔
            if (direction == CyclicScrollViewDirection.Vertical)
            {
                if (reset)
                {
                    content.anchorMin = verticalContentAnchorMin;
                    content.anchorMax = verticalContentAnchorMax;
                    content.pivot = verticalContentPivot;
                    content.anchoredPosition = Vector2.zero;
                    content.offsetMin = Vector2.zero;
                    content.offsetMax = Vector2.zero;
                }

                //Tip：ItemSize是本体+间隔，不要和CellSize搞混了
                //减一份间隔是因为：如3行，那么其实只有2份间隔而不是3份
                content.sizeDelta = new Vector2(content.sizeDelta.x, itemCount * ItemSize.y - cellSpace.y);
            }
            else if (direction == CyclicScrollViewDirection.Horizontal)
            {
                if (reset)
                {
                    content.anchorMin = horizontalContentAnchorMin;
                    content.anchorMax = horizontalContentAnchorMax;
                    content.pivot = horizontalContentPivot;
                    content.anchoredPosition = Vector2.zero;
                    content.offsetMin = Vector2.zero;
                    content.offsetMax = Vector2.zero;
                }

                content.sizeDelta = new Vector2(itemCount * ItemSize.x - cellSpace.x, content.sizeDelta.y);
            }
        }

        /// <summary>
        /// 更改Bundle(初始化或重设)
        /// </summary>
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
                    //初设位置
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
            else//移除Bundle后的填充
            {
                bundle = cellBundlePool.Dequeue();
                bundle.position = postion;
                bundle.index = itemIndex;
                int i = itemIndex * itemCellCount, j = 0;
                int end = itemIndex * itemCellCount + bundle.Cells.Length;
                for (; j < bundle.Cells.Length && i < end; j++, i++)
                {
                    //重设位置
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
        #endregion

        #region 功能函数
        private void ReleaseViewBundle(CellBundle<Cell> viewCellBundle)
        {
            viewCellBundle.Clear();
            cellBundlePool.Enqueue(viewCellBundle);
        }

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

        private bool AboveViewRange(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            return relativePos.y >= ItemSize.y;
        }
        private bool UnderViewRange(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            return relativePos.y <= -viewRange.sizeDelta.y;
        }
        private bool InViewRangeLeft(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            return relativePos.x <= -ItemSize.x;
        }
        private bool InViewRangeRight(Vector2 position)
        {
            Vector2 relativePos = CaculateRelativePostion(position);
            return relativePos.x >= viewRange.sizeDelta.x;
        }
        private bool OnViewRange(Vector2 position)
        {
            if (direction == CyclicScrollViewDirection.Horizontal)
            {
                return !InViewRangeLeft(position) && !InViewRangeRight(position);
            }
            else if (direction == CyclicScrollViewDirection.Vertical)
            {
                return !AboveViewRange(position) && !UnderViewRange(position);
            }
            return false;
        }

        private Vector2 CaculateRelativePostion(Vector2 curPosition)
        {
            Vector2 relativePosition = default;
            if (direction == CyclicScrollViewDirection.Horizontal)
            {
                relativePosition = new Vector2(curPosition.x + content.anchoredPosition.x, curPosition.y);
            }
            else if (direction == CyclicScrollViewDirection.Vertical)
            {
                relativePosition = new Vector2(curPosition.x, curPosition.y + content.anchoredPosition.y);
            }
            return relativePosition;
        }
        #endregion
    }
}