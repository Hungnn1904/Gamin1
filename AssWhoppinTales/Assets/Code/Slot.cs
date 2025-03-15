using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Dragging draggedItem = eventData.pointerDrag.GetComponent<Dragging>();
        if (draggedItem != null)
        {
            // Nếu slot đã có item, hoán đổi
            if (transform.childCount > 0)
            {
                Transform existingItem = transform.GetChild(0);
                existingItem.SetParent(draggedItem.transform.parent);
                existingItem.localPosition = Vector3.zero;
            }

            // Đặt item mới vào slot
            draggedItem.transform.SetParent(transform);
            draggedItem.transform.localPosition = Vector3.zero;
        }
    }
}
