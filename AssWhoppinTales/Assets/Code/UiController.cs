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

        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2);
    }

    // ✅ Đổi từ void -> bool
    public bool AddItem(string itemName, Sprite itemSprite)
    {
        // Kiểm tra trùng item theo tên
        foreach (Slot slot in inventorySlots)
        {
            if (slot.isFull && slot.itemName == itemName)
            {
                Debug.Log("Item already exists in inventory: " + itemName);
                return false; // Không thêm nếu trùng
            }
        }

        // Tìm slot trống để thêm
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (!inventorySlots[i].isFull)
            {
                inventorySlots[i].AddItem(itemName, itemSprite);
                hudSlots[i].AddItem(itemName, itemSprite);
                return true; // Thêm thành công
            }
        }

        Debug.Log("Inventory is full! Cannot pick up: " + itemName);
        return false; // Không thêm được
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < inventorySlots.Length)
        {
            inventorySlots[slotIndex].ClearSlot();
            hudSlots[slotIndex].ClearSlot();
        }
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < inventorySlots.Length && inventorySlots[slotIndex].isFull)
        {
            Debug.Log("Used item: " + inventorySlots[slotIndex].itemName);
            RemoveItem(slotIndex);
        }
    }
}
