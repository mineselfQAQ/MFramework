using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using System;

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

        public bool UseCoolDown = false;
        public float Duration = 1.0f;

        public List<Sprite> spriteList;

        public int spacing;

        public Vector2 size;

        public float scaleFactor = 0.8f;
        public float alphaFactor = 0.6f;

        public Button previousBtn;
        public Button nextBtn;

        private LinkedListNode<GameObject> midNode;

        private int count;

        private int orientFactor;//正向移动为1，负向移动为-1

        private int minXorY;

        private LinkedList<GameObject> objectList = new LinkedList<GameObject>();

        private float totalTime;

        private bool CanPressBtn = true;

        private void Start()
        {
            //初始化
            count = spriteList.Count;
            SetOrientFactor();
            minXorY = CaculateMinXorY(count, spacing);
            totalTime = Duration;

            for (int i = 0; i < count; i++)
            {
                //创建实例
                GameObject go = CreateUI($"Sprite_{i + 1}", this.transform);

                //更改位置
                RectTransform rectTransform = go.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = CalculatePosition(i);
                rectTransform.sizeDelta = new Vector2(size.x, size.y);

                //设置图片
                Image img = go.AddComponent<Image>();
                img.sprite = spriteList[i];

                //加入链表
                objectList.AddLast(go);
            }

            midNode = GetMidNode();
            SetScale();//设置缩放
            SetAlpha();//设置透明度
            SortSprite();//设置先后顺序

            nextBtn.onClick.AddListener(() => 
            {
                MoveNextTweening(Duration);
            });
        }

        private void Update()
        {
            //按钮冷却
            if (UseCoolDown)
            {
                if (!CanPressBtn)
                {
                    totalTime -= Time.deltaTime;

                    if (totalTime < 0.0f)
                    {
                        totalTime = Duration;
                        CanPressBtn = true;

                        nextBtn.interactable = true;
                        previousBtn.interactable = true;
                    }
                }
            }
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
                min = ((count / 2) - 0.5f) * spacing;//减半份
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

        private Vector2 CalculatePosition(int index)
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

        private void MoveNextTweening(float duration)
        {
            if (UseCoolDown)
            {
                if (CanPressBtn)
                {
                    UpdateMidNode(ref midNode);//寻找中心节点

                    SortSprite();//排序
                    SetScaleTweening(duration);//缩放
                    SetAlphaTweening(duration);//更改透明度

                    //核心操作---移动
                    MoveSpriteTweening(duration);

                    CanPressBtn = false;
                    nextBtn.interactable = false;
                    previousBtn.interactable = false;
                }
            }
            else
            {
                UpdateMidNode(ref midNode);//寻找中心节点

                SortSprite();//排序
                SetScaleTweening(duration);//缩放
                SetAlphaTweening(duration);//更改透明度

                //核心操作---移动
                MoveSpriteTweening(duration);
            }
        }

        private void SetScale()
        {
            if (midNode == null) throw new System.Exception();

            int i = 0;//Pow指数
            LinkedListNode<GameObject> previousNode;
            LinkedListNode<GameObject> nextNode;
            if (count % 2 == 0)//偶数形式
            {
                previousNode = GetPrevious(midNode);
                nextNode = midNode;
            }
            else//奇数形式
            {
                previousNode = midNode;
                nextNode = midNode;
            }

            while (GetPrevious(previousNode) != nextNode)
            {
                i++;
                previousNode = GetPrevious(previousNode);
                nextNode = GetNext(nextNode);
                previousNode.Value.transform.localScale *= Mathf.Pow(scaleFactor, i);
                nextNode.Value.transform.localScale *= Mathf.Pow(scaleFactor, i);
            }
        }
        private void SetScaleTweening(float duration)
        {
            if (midNode == null) throw new System.Exception();

            int i = 0;//Pow指数
            LinkedListNode<GameObject> previousNode;
            LinkedListNode<GameObject> nextNode;
            if (count % 2 == 0)//偶数形式
            {
                previousNode = GetPrevious(midNode);
                nextNode = midNode;

                previousNode.Value.transform.DOScale(1, duration);
                nextNode.Value.transform.DOScale(1, duration);
            }
            else//奇数形式
            {
                previousNode = midNode;
                nextNode = midNode;

                previousNode.Value.transform.DOScale(1, duration);
            }

            while (GetPrevious(previousNode) != nextNode)
            {
                i++;
                previousNode = GetPrevious(previousNode);
                nextNode = GetNext(nextNode);

                float num = Mathf.Pow(scaleFactor, i);

                previousNode.Value.transform.DOScale(num, duration);
                nextNode.Value.transform.DOScale(num, duration);
            }
        }

        private void SetAlpha()
        {
            if (midNode == null) throw new System.Exception();

            int i = 0;//Pow指数
            LinkedListNode<GameObject> previousNode;
            LinkedListNode<GameObject> nextNode;
            if (count % 2 == 0)//偶数形式
            {
                previousNode = GetPrevious(midNode);
                nextNode = midNode;
            }
            else//奇数形式
            {
                previousNode = midNode;
                nextNode = midNode;
            }

            while (GetPrevious(previousNode) != nextNode)
            {
                i++;
                previousNode = GetPrevious(previousNode);
                nextNode = GetNext(nextNode);

                float num = Mathf.Pow(alphaFactor, i);

                previousNode.Value.GetComponent<Image>().DOFade(num, 0);
                nextNode.Value.GetComponent<Image>().DOFade(num, 0);
            }
        }
        private void SetAlphaTweening(float duration)
        {
            if (midNode == null) throw new System.Exception();

            int i = 0;//Pow指数
            LinkedListNode<GameObject> previousNode;
            LinkedListNode<GameObject> nextNode;
            if (count % 2 == 0)//偶数形式
            {
                previousNode = GetPrevious(midNode);
                nextNode = midNode;

                previousNode.Value.GetComponent<Image>().DOFade(1, duration);
                nextNode.Value.GetComponent<Image>().DOFade(1, duration);
            }
            else//奇数形式
            {
                previousNode = midNode;
                nextNode = midNode;

                previousNode.Value.GetComponent<Image>().DOFade(1, duration);
            }

            while (GetPrevious(previousNode) != nextNode)
            {
                i++;
                previousNode = GetPrevious(previousNode);
                nextNode = GetNext(nextNode);

                float num = Mathf.Pow(alphaFactor, i);

                previousNode.Value.GetComponent<Image>().DOFade(num, duration);
                nextNode.Value.GetComponent<Image>().DOFade(num, duration);
            }
        }

        private void SortSprite()
        {
            if (midNode == null) throw new System.Exception();

            LinkedListNode<GameObject> previousNode;
            LinkedListNode<GameObject> nextNode;
            if (count % 2 == 0)//偶数形式
            {
                previousNode = GetPrevious(midNode);
                nextNode = midNode;

                previousNode.Value.transform.SetAsFirstSibling();
                nextNode.Value.transform.SetAsFirstSibling();
            }
            else//奇数形式
            {
                previousNode = midNode;
                nextNode = midNode;

                previousNode.Value.transform.SetAsFirstSibling();
            }

            while (GetPrevious(previousNode) != nextNode)
            {
                previousNode = GetPrevious(previousNode);
                nextNode = GetNext(nextNode);

                nextNode.Value.transform.SetAsFirstSibling();
                previousNode.Value.transform.SetAsFirstSibling();//previous放最后，保证最后一个元素压底
            }
        }

        private LinkedListNode<T> GetNext<T>(LinkedListNode<T> node)
        {
            if (node.Next == null)
            {
                return node.List.First;
            }

            return node.Next;
        }
        private LinkedListNode<T> GetPrevious<T>(LinkedListNode<T> node)
        {
            if (node.Previous == null)
            {
                return node.List.Last;
            }

            return node.Previous;
        }

        private void MoveSpriteTweening(float duration)
        {
            LinkedListNode<GameObject> node = objectList.First;

            //需要改进移动方案
            //while (true)
            //{
            //    if (node.Next == null)
            //    {
            //        node.Value.transform.DOMove(objectList.First.Value.transform.position, duration);
            //        break;
            //    }
            //    node.Value.transform.DOMove(node.Next.Value.transform.position, duration);

            //    node = node.Next;
            //}

            while (node != null)
            {
                Vector2 newPos = GetNextNodePos(node);
                node.Value.transform.DOMove(newPos, duration);

                node = node.Next;
            }
        }
        //================================
        private Vector2 GetNextNodePos(LinkedListNode<GameObject> node)
        {
            Vector2 pos = node.Value.transform.position;

            if (mode == Mode.LeftToRight || mode == Mode.RightToLeft)
            {
                if (IsLastNode(node))
                {
                    return new Vector2(pos.x - spacing * orientFactor * (count - 1), pos.y);
                }

                return new Vector2(pos.x + spacing * orientFactor, pos.y);
            }
            else if (mode == Mode.DownToUp || mode == Mode.UpToDown)
            {
                if (IsLastNode(node))
                {
                    return new Vector2(pos.x, pos.y - spacing * orientFactor * (count - 1));
                }

                return new Vector2(pos.x, pos.y + spacing * orientFactor);
            }

            throw new System.Exception("未知错误");
        }
        private bool IsLastNode(LinkedListNode<GameObject> node)
        {
            return GetLastNode() == node;
        }
        private LinkedListNode<GameObject> GetLastNode()
        {
            if (midNode == null) throw new System.Exception();

            int num = count / 2;

            LinkedListNode<GameObject> lastNode = midNode;

            for (int i = 0; i < num; i++)
            {
                lastNode = GetPrevious(lastNode);
            }
            //已经得到首节点
            lastNode = GetPrevious(lastNode);//继续得到尾节点

            return lastNode;
        }
        //==============================

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

        private void UpdateMidNode<T>(ref LinkedListNode<T> node)
        {
            if (node.Previous == null)
            {
                node = node.List.Last;
                return;
            }

            node = node.Previous;
        }
        //private void UpdateList<T>(LinkedList<T> list)
        //{
        //    //使用更新值的方式(每个值向后移动一格)
        //    LinkedListNode<T> node = list.Last;
        //    T tempValue = node.Value;

        //    while (true)
        //    {
        //        if (node.Previous == null)
        //        {
        //            node.Value = tempValue;
        //            break;
        //        }

        //        node.Value = node.Previous.Value;
        //        node = node.Previous;
        //    }
        //}
    }
}