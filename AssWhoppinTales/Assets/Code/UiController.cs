using UnityEngine;

public class UiController : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject hudCanvas;
    public Slot[] inventorySlots; // Inventory slots
    public Slot[] hudSlots; // HUD slots

    void Start()
    {
        menuCanvas.SetActive(false);
        hudCanvas.SetActive(true);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isMenuActive = !menuCanvas.activeSelf;
            menuCanvas.SetActive(isMenuActive);
            hudCanvas.SetActive(!isMenuActive);
            Time.timeScale = isMenuActive ? 0f : 1f;
        }

        // Kiểm tra nếu nhấn phím 1, 2, 3 thì dùng item
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2);
    }

    public void AddItem(string itemName, Sprite itemSprite)
    {
        // Kiểm tra xem item đã có trong inventory chưa
        foreach (Slot slot in inventorySlots)
        {
            if (slot.isFull && slot.itemName == itemName)
            {
                Debug.Log("Item already exists in inventory: " + itemName);
                return;
            }
        }

        // Tìm slot trống trong inventory và HUD
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (!inventorySlots[i].isFull)
            {
                inventorySlots[i].AddItem(itemName, itemSprite);
                hudSlots[i].AddItem(itemName, itemSprite); // Đồng bộ với HUD
                return;
            }
        }
        Debug.Log("Inventory is full! Cannot pick up: " + itemName);
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < inventorySlots.Length)
        {
            inventorySlots[slotIndex].ClearSlot();
            hudSlots[slotIndex].ClearSlot(); // Xóa luôn trên HUD
        }
    }

    // Dùng item và xóa nó khỏi inventory
    public void UseItem(int slotIndex)
    {
        if (slotIndex < inventorySlots.Length && inventorySlots[slotIndex].isFull)
        {
            Debug.Log("Used item: " + inventorySlots[slotIndex].itemName);
            RemoveItem(slotIndex); // Gọi hàm RemoveItem để xóa item khỏi inventory và HUD
        }
    }
}
