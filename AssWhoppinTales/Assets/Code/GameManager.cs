using UnityEngine;
using UnityEngine.SceneManagement; // Cần thiết để tải lại màn hình

public class GameManager : MonoBehaviour
{
    // Tạo Singleton pattern để có thể dễ dàng truy cập GameManager từ bất kỳ đâu
    public static GameManager Instance { get; private set; }

    // Biến để lưu vị trí và xoay ban đầu của người chơi
    // Chúng ta sẽ tìm người chơi bằng Tag "Player" ở Start()
    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;

    private bool isGameOver = false; // Theo dõi trạng thái game over

    void Awake()
    {
        // Đảm bảo chỉ có một instance của GameManager tồn tại
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Hủy nếu đã có instance khác
        }
        else
        {
            Instance = this; // Gán instance hiện tại
            // Tùy chọn: Nếu GameManager cần tồn tại qua nhiều màn hình, hãy bỏ comment dòng dưới
            // DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // Tìm GameObject của người chơi và lưu vị trí, xoay ban đầu của nó
        // Đảm bảo GameObject của người chơi có Tag là "Player" trong Unity Editor
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            initialPlayerPosition = playerObj.transform.position;
            initialPlayerRotation = playerObj.transform.rotation;
            Debug.Log("[GameManager] Đã lưu vị trí ban đầu của người chơi.");
        }
        else
        {
            Debug.LogError("[GameManager] LỖI: Không tìm thấy GameObject của người chơi với tag 'Player' ở Start! Đảm bảo người chơi có tag 'Player' trong Unity Editor.");
        }

        // Đảm bảo Time.timeScale được đặt lại về 1 khi màn hình bắt đầu
        Time.timeScale = 1f;
        isGameOver = false; // Reset trạng thái game over khi màn hình tải
    }

    void Update()
    {
        // Nếu game đang ở trạng thái Game Over, lắng nghe phím 'R' để khởi động lại
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel(); // Gọi phương thức khởi động lại màn hình
            }
        }
    }

    // Phương thức công khai này sẽ được gọi bởi các script kẻ địch khi người chơi bị hạ gục
    public void PlayerDied()
    {
        // Chỉ xử lý nếu game chưa ở trạng thái Game Over
        if (!isGameOver)
        {
            isGameOver = true; // Đặt trạng thái game over
            Time.timeScale = 0f; // Dừng toàn bộ thời gian trong game
            Debug.Log("[GameManager] GAME OVER! Người chơi đã bị hạ gục. Nhấn 'R' để khởi động lại.");
            // Ở đây bạn có thể thêm mã để hiển thị UI "Game Over" (ví dụ: một Canvas với Text "Game Over - Press R to Restart")
        }
    }

    // Phương thức để tải lại màn hình hiện tại, đưa mọi thứ về trạng thái ban đầu
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Đảm bảo game tiếp tục chạy trước khi tải lại màn hình
        // Tải lại màn hình hiện tại theo Build Index (đảm bảo màn hình của bạn đã được thêm vào Build Settings)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        isGameOver = false; // Reset trạng thái game over sau khi khởi động lại
        Debug.Log("[GameManager] Đã khởi động lại màn hình.");
    }
}
