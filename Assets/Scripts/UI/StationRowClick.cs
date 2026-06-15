using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class StationRowClick : MonoBehaviour, IPointerClickHandler
{
    private Action onClick;

    public void Bind(Action callback)
    {
        onClick = callback;
        var image = GetComponent<UnityEngine.UI.Image>();
        if (image != null) image.raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData) => onClick?.Invoke();
}
