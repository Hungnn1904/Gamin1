using UnityEngine;

public class Counter : MonoBehaviour
{
    private static Counter instance;
    public static Counter Instance => instance;

    public bool IsInvincible => isInvincible; // Public getter cho trạng thái bất tử

    private bool isInvincible;
    private bool hasInfiniteAmmo;
    private bool hasSpeedBoost;
    private float powerUpTimer;
    private const float POWER_UP_DURATION = 30f; // Thời gian Power-Up hoạt động

    // Các tham chiếu đến script của người chơi và vũ khí
    private PlayerMovement playerMovement;
    private PistolGun pistolGun;
    private Rifle rifle;
    private Shotgun shotgun;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Giữ GameObject này tồn tại giữa các Scene (nếu bạn có nhiều Scene)
            DontDestroyOnLoad(gameObject);
            Debug.Log("[Counter_DEBUG] Instance Chính đã được Khởi Tạo và Đặt DontDestroyOnLoad.");
        }
        else
        {
            // Nếu đã có một instance khác, hủy instance này để đảm bảo chỉ có một Singleton
            Debug.LogWarning("[Counter_DEBUG] Phát hiện Instance trùng lặp của Counter. Đang Hủy bỏ Instance này.");
            Destroy(gameObject);
            return; // Quan trọng: Thoát ngay để không thực hiện các logic khởi tạo khác cho instance bị hủy
        }

        // Tìm các tham chiếu đến script khác trong Scene
        // Lưu ý: FindObjectOfType có thể tốn kém. Hãy đảm bảo PlayerMovement, PistolGun, Rifle, Shotgun
        // đã tồn tại trong Scene khi Counter Awake.
        playerMovement = FindObjectOfType<PlayerMovement>();
        pistolGun = FindObjectOfType<PistolGun>();
        rifle = FindObjectOfType<Rifle>();
        shotgun = FindObjectOfType<Shotgun>();

        if (playerMovement == null) Debug.LogError("[Counter_DEBUG] Lỗi: Không tìm thấy PlayerMovement trong Scene!");
        if (pistolGun == null) Debug.LogWarning("[Counter_DEBUG] Cảnh báo: Không tìm thấy PistolGun trong Scene.");
        if (rifle == null) Debug.LogWarning("[Counter_DEBUG] Cảnh báo: Không tìm thấy Rifle trong Scene.");
        if (shotgun == null) Debug.LogWarning("[Counter_DEBUG] Cảnh báo: Không tìm thấy Shotgun trong Scene.");
    }

    void Update()
    {
        // Chỉ đếm ngược timer nếu nó lớn hơn 0
        if (powerUpTimer > 0)
        {
            powerUpTimer -= Time.deltaTime; // Giảm thời gian theo thời gian thực
            // Bỏ comment dòng dưới để thấy timer đếm ngược trong Console
            Debug.Log($"[Counter_DEBUG] Power-Up Timer còn lại: {powerUpTimer:F2}s");

            if (powerUpTimer <= 0)
            {
                Debug.Log("[Counter_DEBUG] Power-Up Timer đã hết. Gọi DisablePowerUps().");
                DisablePowerUps(); // Tắt power-up khi timer về 0
            }
        }
    }

    // Phương thức kích hoạt Power-Up
    public void ActivatePowerUp(string powerUpType)
    {
        // Đặt lại timer cho power-up mới
        powerUpTimer = POWER_UP_DURATION;
        Debug.Log($"[Counter_DEBUG] Phương thức ActivatePowerUp được gọi cho: {powerUpType}. Timer đặt lại: {POWER_UP_DURATION}s.");

        // Đảm bảo tắt tất cả các power-up hiện tại trước khi kích hoạt cái mới
        // Điều này đảm bảo chỉ có 1 power-up cùng loại hoạt động hoặc làm mới thời gian
        DisablePowerUps();

        switch (powerUpType)
        {
            case "Invincibility":
                isInvincible = true;
                Debug.Log("[Counter_DEBUG] Bất Tử đã KÍCH HOẠT!");
                break;
            case "InfiniteAmmo":
                hasInfiniteAmmo = true;
                if (pistolGun != null) pistolGun.SetInfiniteAmmo(true);
                if (rifle != null) rifle.SetInfiniteAmmo(true);
                if (shotgun != null) shotgun.SetInfiniteAmmo(true);
                Debug.Log("[Counter_DEBUG] Vô Hạn Đạn đã KÍCH HOẠT!");
                break;
            case "SpeedBoost":
                hasSpeedBoost = true;
                if (playerMovement != null) playerMovement.SetSpeedBoost(true);
                Debug.Log("[Counter_DEBUG] Tăng Tốc đã KÍCH HOẠT!");
                break;
            case "None":
                // Xử lý trường hợp không có power-up nào
                Debug.Log("[Counter_DEBUG] Kích hoạt Power-Up loại 'None'. Không làm gì.");
                break;
            default:
                Debug.LogWarning($"[Counter_DEBUG] Loại Power-Up không xác định: {powerUpType}");
                break;
        }
    }

    // Phương thức vô hiệu hóa tất cả các Power-Up đang hoạt động
    private void DisablePowerUps()
    {
        Debug.Log("[Counter_DEBUG] Bắt đầu Vô Hiệu Hóa Power-Ups.");

        if (isInvincible)
        {
            isInvincible = false;
            Debug.Log("[Counter_DEBUG] Bất Tử đã VÔ HIỆU HÓA!");
        }
        if (hasInfiniteAmmo)
        {
            hasInfiniteAmmo = false;
            if (pistolGun != null) pistolGun.SetInfiniteAmmo(false);
            if (rifle != null) rifle.SetInfiniteAmmo(false);
            if (shotgun != null) shotgun.SetInfiniteAmmo(false);
            Debug.Log("[Counter_DEBUG] Vô Hạn Đạn đã VÔ HIỆU HÓA!");
        }
        if (hasSpeedBoost)
        {
            hasSpeedBoost = false;
            if (playerMovement != null) playerMovement.SetSpeedBoost(false);
            Debug.Log("[Counter_DEBUG] Tăng Tốc đã VÔ HIỆU HÓA!");
        }
        powerUpTimer = 0f; // Đặt lại timer về 0 để dừng đếm ngược
        Debug.Log("[Counter_DEBUG] Đã Vô Hiệu Hóa Power-Ups xong.");
    }
}
