using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace JYL 
{
    public class PointerHandler : MonoBehaviour,
    IEventSystemHandler,
    IPointerClickHandler,
    IPointerUpHandler,
    IPointerDownHandler,
    IPointerMoveHandler,
    IPointerEnterHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
    {
        public UnityAction<PointerEventData> Click;
        public UnityAction<PointerEventData> Up;
        public UnityAction<PointerEventData> Down;
        public UnityAction<PointerEventData> Move;
        public UnityAction<PointerEventData> Enter;
        public UnityAction<PointerEventData> Exit;
        public UnityAction<PointerEventData> BeginDrag;
        public UnityAction<PointerEventData> Drag;
        public UnityAction<PointerEventData> EndDrag;

        public void OnPointerClick(PointerEventData eventData) => Click?.Invoke(eventData);
        public void OnPointerUp(PointerEventData eventData) => Up?.Invoke(eventData);
        public void OnPointerDown(PointerEventData eventData) => Down?.Invoke(eventData);
        public void OnPointerMove(PointerEventData eventData) => Move?.Invoke(eventData);
        public void OnPointerEnter(PointerEventData eventData) => Enter?.Invoke(eventData);
        public void OnPointerExit(PointerEventData eventData) => Exit?.Invoke(eventData);
        public void OnBeginDrag(PointerEventData eventData) => BeginDrag?.Invoke(eventData);
        public void OnDrag(PointerEventData eventData) => Drag?.Invoke(eventData);
        public void OnEndDrag(PointerEventData eventData) => EndDrag?.Invoke(eventData);
    }
}

