using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MFramework
{
    [AddComponentMenu("MFramework/AdvancedUI/MDragger")]
    public class MDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //public int draggerID = -1;

        [SerializeField] private bool centerMove = false;

        [SerializeField] private RectTransform bound;//边界
        [SerializeField] private RectTransform selfBound;//自身边界

        [Header("Events")]
        [Space(5)]
        //Tip:只能分开写，因为Header会用于所有连写变量
        [SerializeField] private UnityEvent onBeginDrag;
        [SerializeField] private UnityEvent onDrag, onEndDrag, onExitBound, onEnterBound;

        public bool isDragging { get; private set; } = false;

        private RectTransform rectTransform;
        private Vector4 boundParmas;//x-左边界 y-右边界 z-下边界 w-上边界

        private Vector2 centerMoveOffset;

        private bool isOutOfBoundLastFrame = false;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            if (selfBound == null)
            {
                selfBound = GetComponent<RectTransform>();
            }

            if (bound != null)
            {
                SetBoundParmas(bound.rect);
            }
        }

        private void SetBoundParmas(Rect boundRect)
        {
            boundParmas.x = boundRect.xMin + selfBound.rect.width / 2;
            boundParmas.y = boundRect.xMax - selfBound.rect.width / 2;
            boundParmas.z = boundRect.yMin + selfBound.rect.height / 2;
            boundParmas.w = boundRect.yMax - selfBound.rect.height / 2;
        }

        public void SetBound(RectTransform rect)
        {
            bound = rect;
            SetBoundParmas(bound.rect);
        }
        //public void SetSelfBound(RectTransform rect)
        //{
        //    selfBound = rect;
        //}

        public void AddListener(MDraggerEvent mEvent, Action action)
        {
            switch (mEvent)
            {
                case MDraggerEvent.BeginDrag:
                    onBeginDrag.AddListener(()=> { action(); });
                    break;
                case MDraggerEvent.Drag:
                    onDrag.AddListener(() => { action(); });
                    break;
                case MDraggerEvent.EndDrag:
                    onEndDrag.AddListener(() => { action(); });
                    break;
                case MDraggerEvent.ExitBound:
                    onExitBound.AddListener(() => { action(); });
                    break;
                case MDraggerEvent.EnterBound:
                    onEnterBound.AddListener(() => { action(); });
                    break;
                default:
                    MLog.Print($"{typeof(MDragger)}:不支持的类型<{mEvent}>,请检查", MLogType.Warning);
                    break;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag.Invoke();

            //启用centerMove时，强制将图片拉到鼠标处
            Vector2 pointPos = eventData.position - MUtility.ScreenMidPos;
            Vector2 objPos = rectTransform.anchoredPosition;
            centerMoveOffset = centerMove ? pointPos - objPos : Vector2.zero;
            rectTransform.anchoredPosition += centerMoveOffset;

            isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag.Invoke();

            MoveBasedPointer(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag.Invoke();

            isDragging = false;
        }

        private void MoveBasedPointer(PointerEventData eventData)
        {
            if (bound == null)//常规移动
            {
                rectTransform.anchoredPosition += eventData.delta;
            }
            else//边界限制移动
            {
                LimitedMove(eventData);
            }
        }

        private void LimitedMove(PointerEventData eventData)
        {
            Vector2 pointerPos = eventData.position - MUtility.ScreenMidPos;
            Vector2 delta = eventData.delta;

            bool isOutOfBound = false;

            //Tip:必须这样写，是因为:
            //如果将左右/上下合并(越左右边界就不让横向移动)，这样会导致鼠标快速移出后物体就被限制移动了，
            //但是如果如下述写法快速移出后，比如说出上界，此时鼠标必然还是在向上移动，那么只是限制了向下移动，还是能够正常移动的
            if (pointerPos.x < boundParmas.x)//越左边界
            {
                isOutOfBound = true;
                if (delta.x > 0) delta.x = 0;//不允许向右移动
            }
            else if (pointerPos.x > boundParmas.y)//越右边界
            {
                isOutOfBound = true;
                if (delta.x < 0) delta.x = 0;//不允许向左移动
            }

            if (pointerPos.y < boundParmas.z)//越下边界
            {
                isOutOfBound = true;
                if (delta.y > 0) delta.y = 0;//不允许向上移动
            }
            else if (pointerPos.y > boundParmas.w)//越上边界
            {
                isOutOfBound = true;
                if (delta.y < 0) delta.y = 0;//不允许向下移动
            }

            if (isOutOfBound && !isOutOfBoundLastFrame)
            {
                onExitBound.Invoke();
            }
            if (!isOutOfBound && isOutOfBoundLastFrame)
            {
                onEnterBound.Invoke();
            }
            isOutOfBoundLastFrame = isOutOfBound;

            rectTransform.anchoredPosition += delta;
            float rangeX = Mathf.Clamp(rectTransform.anchoredPosition.x, boundParmas.x, boundParmas.y);
            float rangeY = Mathf.Clamp(rectTransform.anchoredPosition.y, boundParmas.z, boundParmas.w);
            rectTransform.anchoredPosition = new Vector2(rangeX, rangeY);
        }
    }

    public enum MDraggerEvent
    {
        BeginDrag,
        Drag,
        EndDrag,
        ExitBound,
        EnterBound
    }
}