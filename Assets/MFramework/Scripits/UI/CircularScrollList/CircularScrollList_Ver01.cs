using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace MFramework
{
    namespace OldVersion
    {
        public enum Mode
        {
            LeftToRight,
            RightToLeft,
            UpToDown,
            DownToUp
        }

        public class CircularScrollList_Ver01 : MonoBehaviour
        {
            //大致逻辑：
            //使用链表从前到后存储所有图片，需要计算位置/大小/透明度，
            //对于大小/透明度，通过从midNode向两侧的方式计算
            //对于位置，通过firstNode循环一圈的方式计算
            //可改进方式：
            //链表中直接存储所有信息，对于每一个内容，都向后寻找一个

            public Mode mode;

            public bool UseCoolDown = false;
            public float Duration = 1.0f;

            [Space(5)]
            public List<Sprite> spriteList;

            public int spacing;

            public Vector2 size;

            public float scaleFactor = 0.8f;
            public float alphaFactor = 0.6f;

            public Button previousBtn;//**未制作，改进后添加**
            public Button nextBtn;

            private LinkedListNode<GameObject> midNode;

            private int count;

            private int orientFactor;//正向移动为1，负向移动为-1

            private int minXorY;

            private LinkedList<GameObject> objectList = new LinkedList<GameObject>();
            private Vector2[] posList;

            private float totalTime;

            private bool CanPressBtn = true;

            private void Start()
            {
                //初始化
                count = spriteList.Count;
                SetOrientFactor();
                minXorY = CaculateMinXorY(count, spacing);
                totalTime = Duration;
                posList = new Vector2[count];

                for (int i = 0; i < count; i++)
                {
                    //创建实例
                    GameObject go = CreateUI($"Sprite_{i + 1}", this.transform);

                    //更改位置
                    RectTransform rectTransform = go.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = CalculatePosition(i);
                    rectTransform.sizeDelta = new Vector2(size.x, size.y);
                    posList[i] = rectTransform.anchoredPosition + new Vector2(Screen.width / 2, Screen.height / 2);

                    //设置图片
                    Image img = go.AddComponent<Image>();
                    img.sprite = spriteList[i];

                    //加入链表
                    objectList.AddLast(go);
                }

                midNode = GetMidNode();
                SetScaleTweening(0);//设置缩放
                SetAlphaTweening(0);//设置透明度
                SortSprite();//设置先后顺序
                UpdateMidNode(ref midNode);//更新中心节点

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
                        SortSprite();//排序
                        SetScaleTweening(duration);//缩放
                        SetAlphaTweening(duration);//更改透明度

                        //核心操作---移动
                        MoveSpriteTweening(duration);

                        UpdateMidNode(ref midNode);//更改中心节点

                        CanPressBtn = false;
                        nextBtn.interactable = false;
                        previousBtn.interactable = false;
                    }
                }
                else
                {
                    SortSprite();//排序
                    SetScaleTweening(duration);//缩放
                    SetAlphaTweening(duration);//更改透明度

                    //核心操作---移动
                    MoveSpriteTweening(duration);

                    UpdateMidNode(ref midNode);//更改中心节点
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
                LinkedListNode<GameObject> node = GetFirstNode();
                LinkedListNode<GameObject> firstNode = node;

                Vector2 newPos;
                int i = -1;//注意：获取的FirstNode为目标状态下的，"多了一轮"，所以需要从-1开始
                while (GetNext(node) != firstNode)
                {
                    i++;
                    newPos = posList[i];
                    node.Value.transform.DOMove(newPos, duration);

                    node = GetNext(node);
                }
                newPos = posList[count - 1];
                node.Value.transform.DOMove(newPos, duration);
            }

            private LinkedListNode<GameObject> GetFirstNode()
            {
                if (midNode == null) throw new System.Exception();

                int num = count / 2;

                LinkedListNode<GameObject> firstNode = midNode;

                for (int i = 0; i < num; i++)
                {
                    firstNode = GetPrevious(firstNode);
                }

                return firstNode;
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

            private void UpdateMidNode<T>(ref LinkedListNode<T> node)
            {
                if (node.Previous == null)
                {
                    node = node.List.Last;
                    return;
                }

                node = node.Previous;
            }
        }
    }
}