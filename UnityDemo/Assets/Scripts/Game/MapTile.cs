using UnityEngine;
using UnityEngine.EventSystems;

public class MapTile : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        NotificationCenter.Instance.PushEvent(NotificationType.Operate_MapPosition, transform.position);
    }
}
