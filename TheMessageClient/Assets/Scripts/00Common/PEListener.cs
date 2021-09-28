//触控事件监听


using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PEListener : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerDownHandler
{
    public Action<GameObject> onClickDown;
    public Action<GameObject> onClickUp;
    public Action<GameObject> onEnter;

    public void OnPointerDown(PointerEventData eventData)
    {
        onClickDown?.Invoke(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onClickUp?.Invoke(gameObject);
    }
}
