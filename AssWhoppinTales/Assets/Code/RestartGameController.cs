// Nguồn file: Code/RestartGameController.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGameController : MonoBehaviour
{
    // Bạn có thể tùy chọn phím để kích hoạt việc khởi động lại
    public KeyCode restartKey = KeyCode.R;

    void Update()
    {
        // Kiểm tra xem người chơi có nhấn phím đã định không
        // VÀ quan trọng hơn: chỉ cho phép khởi động lại nếu game đang ở trạng thái dừng (Time.timeScale == 0)
        // Điều này đảm bảo bạn không vô tình khởi động lại giữa trận đấu
        if (Input.GetKeyDown(restartKey) && Time.timeScale == 0f)
        {
            // Kiểm tra xem GameManager có tồn tại và đang hoạt động không
            if (GameManager.Instance != null)
            {
                Debug.Log("[RestartGameController] Phím R được nhấn khi game OVER, đang khởi động lại màn chơi.");
                GameManager.Instance.RestartLevel();
            }
            else
            {
                Debug.LogWarning("[RestartGameController] GameManager.Instance không tìm thấy. Không thể khởi động lại màn chơi.");
                // Fallback: Nếu GameManager không tồn tại, bạn vẫn có thể tự tải lại cảnh
                // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
