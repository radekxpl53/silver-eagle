using UnityEngine;
using UnityEngine.EventSystems;

public class Sector : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int gridPosition;

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerTeleport.Instance.Teleport(gridPosition);
    }
}
