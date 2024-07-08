using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace MFramework
{
    /// <summary>
    /// ѡ����������Ϊһ������������ѡ���б�
    /// </summary>
    public class Chooser : MonoBehaviour
    {
        public Mode mode;

        public bool UseCoolDown = false;
        public float Duration = 1.0f;

        [Space(5)]
        public List<Sprite> spriteList;

        public int spacing = 200;

        public Vector2 size;

        public float scaleFactor = 0.8f;
        public float alphaFactor = 0.8f;

        public Button previousBtn;
        public Button nextBtn;

        private int count;

        private int orientFactor;//�����ƶ�Ϊ1�������ƶ�Ϊ-1

        private int minXorY;

        private LinkedList<Node> objectList = new LinkedList<Node>();

        private float totalTime;

        private bool CanPressBtn = true;

        private int midIndex;

        private Vector2 basePos;

        private void Start()
        {
            //��ʼ��
            count = spriteList.Count;
            midIndex = CalculateMidIndex(count);
            SetOrientFactor();
            minXorY = CalculateMinXorY(count, spacing);
            totalTime = Duration;
            basePos = transform.position;

            for (int i = 0; i < count; i++)
            {
                //����ʵ��
                GameObject go = CreateUI($"Sprite_{i + 1}", this.transform);

                RectTransform rectTransform = go.GetComponent<RectTransform>();
                //λ��
                rectTransform.anchoredPosition = CalculatePosition(i);
                Vector2 objPos = rectTransform.anchoredPosition + basePos;
                //��С
                rectTransform.sizeDelta = new Vector2(size.x, size.y);
                float objScale = CalculateScale(i, midIndex);
                rectTransform.localScale *= objScale;
                //͸����
                float objAlpha = CalculateAlpha(i, midIndex);
                Image img = go.AddComponent<Image>();
                img.color = new Color(img.color.r, img.color.g, img.color.b, objAlpha);

                //����ͼƬ
                img.sprite = spriteList[i];

                //��������
                objectList.AddLast(new Node(go, objPos, objScale, objAlpha, i));
            }
            SortSprite(1);//����1��-1��Ӱ����

            nextBtn.onClick.AddListener(() =>
            {
                MoveNext(Duration);
            });

            previousBtn.onClick.AddListener(() =>
            {
                MovePrevious(Duration);
            });
        }

        private void Update()
        {
            //��ť��ȴ
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

        private int CalculateMidIndex(int count)
        {
            int num = count / 2;

            return (count % 2 == 0) ? num - 1 : num;
        }

        private void UpdateMidIndex(ref int midIndex, int orientFactor)
        {
            if (orientFactor == 1)//��ǰ
            {
                if (midIndex == 0)
                {
                    midIndex = count - 1;
                    return;
                }

                midIndex -= 1;
            }
            else//���
            {
                if (midIndex == count - 1)
                {
                    midIndex = 0;
                    return;
                }

                midIndex += 1;
            }
        }

        private int CalculateMinXorY(int count, int spacing)
        {
            if (orientFactor == 0) throw new System.Exception("δ��orientFactor���и�ֵ");

            if (spacing == 0)
            {
                return 0;
            }

            float min;
            if (count % 2 == 0)//ż�����
            {
                min = ((count / 2) - 0.5f) * spacing;//�����
            }
            else//�������
            {
                min = (count / 2) * spacing;
            }

            if (orientFactor == 1)
            {
                return (int)-min;//����ʼ
            }
            else if (orientFactor == -1)
            {
                return (int)min;//����ʼ
            }

            throw new System.Exception("δ֪����");
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

        private Vector2 CalculatePosition(int index)
        {
            if (mode == Mode.LeftToRight || mode == Mode.RightToLeft)//�������
            {
                return new Vector2(minXorY + index * spacing * orientFactor, 0);
            }
            else if (mode == Mode.DownToUp || mode == Mode.UpToDown)//�������
            {
                return new Vector2(0, minXorY + index * spacing * orientFactor);
            }

            throw new System.Exception("δ֪����");
        }

        private float CalculateScale(int index, int midIndex)
        {
            int exp = Math.Abs(midIndex - index);

            if (exp == 0) return 1;

            float num = Mathf.Pow(scaleFactor, exp);
            return num;
        }

        private float CalculateAlpha(int index, int midIndex)
        {
            int exp = Math.Abs(midIndex - index);

            if (exp == 0) return 1;

            float num = Mathf.Pow(scaleFactor, exp);
            return num;
        }

        private void Move(float duration, int orientFactor)
        {
            if (UseCoolDown)
            {
                if (CanPressBtn)
                {
                    UpdateMidIndex(ref midIndex, orientFactor);//������������

                    SortSprite(orientFactor);//����
                    MoveTweening(duration, orientFactor);

                    CanPressBtn = false;
                    nextBtn.interactable = false;
                    previousBtn.interactable = false;
                }
            }
            else
            {
                UpdateMidIndex(ref midIndex, orientFactor);//������������

                SortSprite(orientFactor);//����
                MoveTweening(duration, orientFactor);
            }
        }

        private void MoveNext(float duration)
        {
            Move(duration, 1);
        }
        private void MovePrevious(float duration)
        {
            Move(duration, -1);
        }

        private void MoveTweening(float duration, int orientFactor)
        {
            if (orientFactor == 1)//��ǰ
            {
                LinkedListNode<Node> node = objectList.First;
                Vector2 firstPos = node.Value.pos;
                float firstScale = node.Value.scale;
                float firstAlpha = node.Value.alpha;

                while (node.Next != null)
                {
                    LinkedListNode<Node> nextNode = node.Next;
                    node.Value.go.transform.DOMove(nextNode.Value.pos, duration);
                    node.Value.pos = nextNode.Value.pos;
                    node.Value.go.transform.DOScale(nextNode.Value.scale, duration);
                    node.Value.scale = nextNode.Value.scale;
                    node.Value.go.GetComponent<Image>().DOFade(nextNode.Value.alpha, duration);
                    node.Value.alpha = nextNode.Value.alpha;

                    node = nextNode;
                }
                node.Value.go.transform.DOMove(firstPos, duration);
                node.Value.pos = firstPos;
                node.Value.go.transform.DOScale(firstScale, duration);
                node.Value.scale = firstScale;
                node.Value.go.GetComponent<Image>().DOFade(firstAlpha, duration);
                node.Value.alpha = firstAlpha;
            }
            else//���
            {
                LinkedListNode<Node> node = objectList.Last;
                Vector2 lastPos = node.Value.pos;
                float lastScale = node.Value.scale;
                float lastAlpha = node.Value.alpha;

                while (node.Previous != null)
                {
                    LinkedListNode<Node> previousNode = node.Previous;
                    node.Value.go.transform.DOMove(previousNode.Value.pos, duration);
                    node.Value.pos = previousNode.Value.pos;
                    node.Value.go.transform.DOScale(previousNode.Value.scale, duration);
                    node.Value.scale = previousNode.Value.scale;
                    node.Value.go.GetComponent<Image>().DOFade(previousNode.Value.alpha, duration);

                    node = previousNode;
                }
                node.Value.go.transform.DOMove(lastPos, duration);
                node.Value.pos = lastPos;
                node.Value.go.transform.DOScale(lastScale, duration);
                node.Value.scale = lastScale;
                node.Value.go.GetComponent<Image>().DOFade(lastAlpha, duration);
                node.Value.alpha = lastAlpha;
            }
        }

        private void SortSprite(int orientFactor)
        {
            LinkedListNode<Node> midNode = GetMidNode(midIndex);

            LinkedListNode<Node> previousNode;
            LinkedListNode<Node> nextNode;
            if (count % 2 == 0)//ż����ʽ
            {
                previousNode = GetPrevious(midNode);
                nextNode = midNode;

                previousNode.Value.go.transform.SetAsFirstSibling();
                nextNode.Value.go.transform.SetAsFirstSibling();
            }
            else//������ʽ
            {
                previousNode = midNode;
                nextNode = midNode;

                previousNode.Value.go.transform.SetAsFirstSibling();
            }

            while (GetPrevious(previousNode) != nextNode)
            {
                previousNode = GetPrevious(previousNode);
                nextNode = GetNext(nextNode);

                previousNode.Value.go.transform.SetAsFirstSibling();
                nextNode.Value.go.transform.SetAsFirstSibling();
            }

            //��֤�л�"��ԾͼƬ"ʱ������ͼƬ�·�ͨ��
            if (orientFactor == 1)
            {
                previousNode.Value.go.transform.SetAsFirstSibling();
            }
        }

        private LinkedListNode<Node> GetMidNode(int midIndex)
        {
            LinkedListNode<Node> node = objectList.First;

            while (node != null)
            {
                if (node.Value.index == midIndex)
                {
                    return node;
                }

                node = node.Next;
            }

            throw new Exception();
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

        public enum Mode
        {
            LeftToRight,
            RightToLeft,
            UpToDown,
            DownToUp
        }

        public class Node
        {
            public GameObject go;

            public Vector2 pos;
            public float scale;
            public float alpha;
            public int index;

            public Node(GameObject go, Vector2 pos, float scale, float alpha, int index)
            {
                this.go = go;
                this.pos = pos;
                this.scale = scale;
                this.alpha = alpha;
                this.index = index;
            }
        }
    }
}