using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class CyclicScrollView<Cell, Data> : MonoBehaviour where Cell : MonoBehaviour
    {
        public CyclicScrollViewDirection direction;
        public ICollection<Data> datas { get; private set; }

        [SerializeField] protected Cell cellPrefab;//传入Prefab(必须带有Cell子类型的组件)
        [SerializeField] protected RectTransform content;//Scrollview中自带的Viewport下的Content

        [SerializeField] private RectTransform viewRange;//父物体(带有ScrollRect组件)的RectTransform
        [SerializeField] private Vector2 cellSpace;//Cell之间的间距
        [SerializeField] private int itemCellCount;//行/列中元素数(一个ViewCellBundle中有几个元素)

        private RectTransform cellRectTrans;

        private readonly Vector2 HorizontalContentAnchorMin = new Vector2(0, 0);
        private readonly Vector2 HorizontalContentAnchorMax = new Vector2(0, 1);
        private readonly Vector2 VerticalContentAnchorMin = new Vector2(0, 1);
        private readonly Vector2 VerticalContentAnchorMax = new Vector2(1, 1);

        private readonly LinkedList<CellBundle<Cell>> cellBundles = new LinkedList<CellBundle<Cell>>();

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
                content.anchorMin = VerticalContentAnchorMin;
                content.anchorMax = VerticalContentAnchorMax;

                //Tip：ItemSize是本体+间隔，不要和CellSize搞混了
                //减一份间隔是因为：如3行，那么其实只有2份间隔而不是3份
                content.sizeDelta = new Vector2(content.sizeDelta.x, itemCount * ItemSize.y - cellSpace.y);
            }
            else if (direction == CyclicScrollViewDirection.Horizontal)
            {
                content.anchorMin = HorizontalContentAnchorMin;
                content.anchorMax = HorizontalContentAnchorMax;
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
                //RefreshAllCellInViewRange();//完全更新viewCellBundles
            }
            else
            {
                //AddHead();
                //AddTail();
            }
            //清除越界,比如数据减少,此时就要清理在视野内的,在数据之外的UI
            //RemoveItemOutOfListRange();
        }

        //private void RefreshAllCellInViewRange()
        //{
        //    int itemCount = ItemCount;
        //    Vector2 viewRangeSize = viewRange.sizeDelta;
        //    Vector2 itemSize = ItemSize;
        //    Vector2 cellSize = CellSize;
        //    Vector2 cellSpace = this.cellSpace;

        //    if (direction == CyclicScrollViewDirection.Vertical)
        //    {
        //        //获取ScrollView的顶和底
        //        Vector2 topPos = -content.anchoredPosition;
        //        Vector2 bottomPos = new Vector2(topPos.x, topPos.y - viewRangeSize.y);

        //        //获取顶和底所在行数
        //        int startIndex = GetIndex(topPos);
        //        int endIndex = GetIndex(bottomPos);
        //        //
        //        for (int i = startIndex; i <= endIndex && i < itemCount; i++)//for循环添加所有的bundle
        //        {
        //            Vector2 pos = new Vector2(content.anchoredPosition.x, -i * itemSize.y);
        //            var bundle = GetViewBundle(i, pos, cellSize, cellSpace);//每个bundle之间的差距只有pos不同
        //            viewCellBundles.AddLast(bundle);
        //        }
        //    }
        //    else if (viewDirection == UICyclicScrollDirection.Horizontal)
        //    {
        //        Vector2 leftPos = -content.anchoredPosition;
        //        Vector2 rightPos = new Vector2(leftPos.x + viewRangeSize.x, leftPos.y);

        //        int startIndex = GetIndex(leftPos);
        //        int endIndex = GetIndex(rightPos);

        //        for (int i = startIndex; i <= endIndex && i < itemCount; i++)
        //        {
        //            Vector2 pos = new Vector2(i * itemSize.x, content.anchoredPosition.y);
        //            var bundle = GetViewBundle(i, pos, cellSize, cellSpace);
        //            viewCellBundles.AddLast(bundle);
        //        }
        //    }
        //}

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