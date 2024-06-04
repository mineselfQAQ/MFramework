using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[AddComponentMenu("MFramework/AdvancedUI/MDragger")]
public class MDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int draggerID = -1;

    [Header("Events")][Space(5)]
    //Tip:只能分开写，因为Header会用于所有连写变量
    public UnityEvent onBeginDrag;
    public UnityEvent onDrag, onEndDrag, onEnterConstraint, onLeaveConstraint, onSpawn, onDispose;

    [HideInInspector] public bool isDragging = false, followsPointer = false;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag.Invoke();

        isDragging = true;
        followsPointer = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag.Invoke();

        MoveBasedPointer(eventData);
    }

    private void MoveBasedPointer(PointerEventData eventData)
    {
        if (followsPointer)
        {
            rectTransform.anchoredPosition += eventData.delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag.Invoke();

        isDragging = false;
        followsPointer = false;
    }
}
