using UnityEngine;

public class Items : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;

    [Header("Power-Up Settings")]
    [SerializeField] private bool enableInvincibility;
    [SerializeField] private bool enableInfiniteAmmo;
    [SerializeField] private bool enableSpeedBoost;

    private UiController inventoryManager;
    private PowerUpManager powerUpManager;

    void Start()
    {
        inventoryManager = GameObject.Find("U.I").GetComponent<UiController>();
        powerUpManager = PowerUpManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player picked up: " + itemName);

            if (inventoryManager != null)
            {
                bool added = inventoryManager.AddItem(itemName, sprite);
                if (added)
                {
                    // Store power-up type in inventory slot
                    int slotIndex = GetLastAddedSlotIndex();
                    if (slotIndex >= 0)
                    {
                        inventoryManager.SetPowerUpType(slotIndex, GetPowerUpType());
                    }
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Item trùng hoặc inventory đầy, không xóa khỏi map.");
                }
            }
            else
            {
                Debug.LogError("inventoryManager is NULL! Kiểm tra 'U.I' có UiController không.");
            }
        }
    }

    private int GetLastAddedSlotIndex()
    {
        for (int i = 0; i < inventoryManager.inventorySlots.Length; i++)
        {
            if (inventoryManager.inventorySlots[i].isFull && inventoryManager.inventorySlots[i].itemName == itemName)
            {
                return i;
            }
        }
        return -1;
    }

    private string GetPowerUpType()
    {
        if (enableInvincibility) return "Invincibility";
        if (enableInfiniteAmmo) return "InfiniteAmmo";
        if (enableSpeedBoost) return "SpeedBoost";
        return "None";
    }
}