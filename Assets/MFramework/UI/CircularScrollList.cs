using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MFramework
{
    public enum Mode
    {
        LeftToRight,
        RightToLeft,
        UpToDown,
        DownToUp
    }

    public class CircularScrollList : MonoBehaviour
    {
        public Mode mode;

        public List<Sprite> spriteList;

        public int spacing;

        public Vector2 size;

        public float scaleFactor = 0.8f;

        public Button previousBtn;
        public Button nextBtn;

        private LinkedListNode<GameObject> midNode;

        private int count;

        private int orientFactor;//正向移动为1，负向移动为-1

        private int minXorY;

        private LinkedList<GameObject> objectList = new LinkedList<GameObject>();

        private void Start()
        {
            //初始化
            count = spriteList.Count;
            SetOrientFactor();
            minXorY = CaculateMinXorY(count, spacing);

            for (int i = 0; i < count; i++)
            {
                //创建实例
                GameObject go = CreateUI($"Sprite_{i + 1}", this.transform);

                //更改位置
                RectTransform rectTransform = go.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = CaculatePosition(i);
                rectTransform.sizeDelta = new Vector2(size.x, size.y);

                //设置图片
                Image img = go.AddComponent<Image>();
                img.sprite = spriteList[i];

                //加入链表
                objectList.AddLast(go);
            }

            midNode = GetMidNode();
            SetScale();//设置缩放
            SortSprite();//设置先后顺序

            nextBtn.onClick.AddListener(MoveNextTweening);
        }

        private GameObject CreateUI(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);

            Destroy(go.transform.GetComponent<Transform>());
            go.AddComponent<RectTransform>();

            return go;
        }

        private int CaculateMinXorY(int count, int spacing)
        {
            if (orientFactor == 0) throw new System.Exception("未对orientFactor进行赋值");

            if (spacing == 0)
            {
                return 0;
            }

            float min;
            if (count % 2 == 0)//偶数情况
            {
                min = ((count / 2) + 0.5f) * spacing;//加半份
            }
            else//奇数情况
            {
                min = (count / 2) * spacing;
            }

            if (orientFactor == 1)
            {
                return (int)-min;//负开始
            }
            else if (orientFactor == -1)
            {
                return (int)min;//正开始
            }

            throw new System.Exception("未知错误");
        }

        private Vector2 CaculatePosition(int index)
        {
            if (mode == Mode.LeftToRight || mode == Mode.RightToLeft)//横向情况
            {
                return new Vector2(minXorY + index * spacing * orientFactor, 0);
            }
            else if (mode == Mode.DownToUp || mode == Mode.UpToDown)//纵向情况
            {
                return new Vector2(0, minXorY + index * spacing * orientFactor);
            }

            throw new System.Exception("未知错误");
        }

        private void SetOrientFactor()
        {
            if (mode == Mode.LeftToRight || mode == Mode.DownToUp)
            {
                orientFactor = 1;
            }
            else if (mode == Mode.RightToLeft || mode == Mode.UpToDown)
            {
                orientFactor = -1;
            }
        }

        private void SetScale()
        {
            if (midNode == null) throw new System.Exception();

            var previousNode = midNode;
            var nextNode = midNode;
            int i = 0;//Pow指数
            while (nextNode != objectList.Last)
            {
                i++;
                previousNode = previousNode.Previous;
                nextNode = nextNode.Next;
                previousNode.Value.transform.localScale *= Mathf.Pow(scaleFactor, i);
                nextNode.Value.transform.localScale *= Mathf.Pow(scaleFactor, i);
            }
            if (count % 2 == 0)
            {
                previousNode = previousNode.Previous;
                previousNode.Value.transform.localScale *= Mathf.Pow(scaleFactor, i);
            }
        }

        private void SortSprite()
        {
            if (midNode == null) throw new System.Exception();

            midNode.Value.transform.SetAsFirstSibling();

            var previousNode = midNode;
            var nextNode = midNode;
            while (nextNode != objectList.Last)
            {
                previousNode = previousNode.Previous;
                nextNode = nextNode.Next;
                previousNode.Value.transform.SetAsFirstSibling();
                nextNode.Value.transform.SetAsFirstSibling();
            }
            if (count % 2 == 0)
            {
                previousNode = previousNode.Previous;
                previousNode.Value.transform.SetAsFirstSibling();
            }
        }

        private LinkedListNode<GameObject> GetMidNode()
        {
            if (objectList.Count == 0) throw new System.Exception("objectList没有元素");

            int num = count / 2;//迭代次数

            LinkedListNode<GameObject> node = objectList.First;

            for (int i = 0; i < num; i++)
            {
                node = node.Next;
            }

            return node;
        }

        private void MoveNextTweening()
        {
            LinkedListNode<GameObject> node = objectList.First;
            while (true)
            {
                if (node.Next == null)
                {
                    node.Value.transform.DOMove(objectList.First.Value.transform.position, 1);
                    break;
                }
                node.Value.transform.DOMove(node.Next.Value.transform.position, 1);

                node = node.Next;
            }
        }
    }
}