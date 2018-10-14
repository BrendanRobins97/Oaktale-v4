using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler
{

    private Vector2 offset;

    public void OnPointerDown(PointerEventData eventData)
    {
        offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
        this.transform.position = eventData.position - offset;

    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position - offset;
    }
}
