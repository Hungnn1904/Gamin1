using UnityEngine;

public class Items : MonoBehaviour
{
    [SerializeField] private string itemName; // Tên của vật phẩm (được hiển thị trong Inspector)
    [SerializeField] private Sprite sprite;   // Hình ảnh của vật phẩm (được hiển thị trong Inspector)

    [Header("Power-Up Settings")] // Tiêu đề để nhóm các thiết lập Power-Up trong Inspector
    [SerializeField] private bool enableInvincibility; // Bật/tắt hiệu ứng bất tử cho vật phẩm này
    [SerializeField] private bool enableInfiniteAmmo;  // Bật/tắt hiệu ứng vô hạn đạn cho vật phẩm này
    [SerializeField] private bool enableSpeedBoost;    // Bật/tắt hiệu ứng tăng tốc cho vật phẩm này

    private UiController inventoryManager; // Tham chiếu đến script UiController để quản lý Inventory
    private Counter counter; // Đã đổi từ PowerUpManager sang Counter

    void Start()
    {
        // Tìm và lấy tham chiếu đến UiController (giả sử nó nằm trên GameObject tên "U.I")
        inventoryManager = GameObject.Find("U.I").GetComponent<UiController>();
        // Lấy tham chiếu đến instance duy nhất của Counter (do Singleton pattern)
        counter = Counter.Instance; // Đã đổi từ PowerUpManager.Instance sang Counter.Instance
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là người chơi không
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player picked up: " + itemName); // Ghi log khi người chơi nhặt vật phẩm

            // Đảm bảo inventoryManager không rỗng
            if (inventoryManager != null)
            {
                // Cố gắng thêm vật phẩm vào Inventory của người chơi
                bool added = inventoryManager.AddItem(itemName, sprite);
                if (added)
                {
                    // Nếu thêm thành công vào Inventory:
                    // Tìm chỉ số slot vừa được thêm vào
                    int slotIndex = GetLastAddedSlotIndex();
                    if (slotIndex >= 0)
                    {
                        // Lưu loại Power-Up vào slot Inventory thông qua UiController
                        // Power-Up CHƯA được kích hoạt ở đây. Nó chỉ được lưu để sau này có thể dùng.
                        inventoryManager.SetPowerUpType(slotIndex, GetPowerUpType());
                    }
                    // Hủy GameObject của vật phẩm trên bản đồ sau khi nhặt
                    Destroy(gameObject);
                }
                else
                {
                    // Nếu Inventory đầy hoặc vật phẩm trùng, không xóa khỏi bản đồ
                    Debug.Log("Item trùng hoặc inventory đầy, không xóa khỏi map.");
                }
            }
            else
            {
                Debug.LogError("inventoryManager is NULL! Kiểm tra 'U.I' có UiController không.");
            }
        }
    }

    // Phương thức trợ giúp để lấy chỉ số của slot cuối cùng mà vật phẩm được thêm vào
    // (Giả sử vật phẩm vừa được thêm vào là duy nhất hoặc là vật phẩm mới nhất cùng tên)
    private int GetLastAddedSlotIndex()
    {
        for (int i = 0; i < inventoryManager.inventorySlots.Length; i++)
        {
            if (inventoryManager.inventorySlots[i].isFull && inventoryManager.inventorySlots[i].itemName == itemName)
            {
                return i;
            }
        }
        return -1; // Trả về -1 nếu không tìm thấy (lỗi logic nếu AddItem trả về true mà không tìm thấy slot)
    }

    // Phương thức trợ giúp để trả về chuỗi tên Power-Up dựa trên các boolean đã bật
    private string GetPowerUpType()
    {
        if (enableInvincibility) return "Invincibility";
        if (enableInfiniteAmmo) return "InfiniteAmmo";
        if (enableSpeedBoost) return "SpeedBoost";
        return "None"; // Không có Power-Up nào được bật
    }
}
