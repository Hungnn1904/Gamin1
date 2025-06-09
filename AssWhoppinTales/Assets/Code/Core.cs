// Nguồn file: Code/Core.cs
using UnityEngine;

public class Core : MonoBehaviour
{
    public int scoreValue = 100; // Điểm số khi tiêu diệt Core

    void Start()
    {
        // Core đứng yên, không cần Rigidbody2D di chuyển tự động.
        // Tuy nhiên, nếu bạn muốn nó có va chạm vật lý với đạn, vẫn cần Rigidbody2D
        // và đặt Body Type là Kinematic hoặc Static để nó không bị đẩy bởi va chạm khác.
        // Ví dụ: rb = GetComponent<Rigidbody2D>(); if (rb != null) rb.bodyType = RigidbodyType2D.Static;
    }

    // Phương thức này sẽ được gọi bởi Bullets khi Core bị bắn trúng
    public void Die()
    {
        Debug.Log($"[Core] {gameObject.name} đã bị phá hủy! Toàn bộ Cloud sẽ chết theo!");

        // Gọi GameManager để cộng điểm khi Core bị diệt
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
            GameManager.Instance.EnemyDefeated(); // Core bị diệt tính là một kẻ địch bị diệt
        }

        // Tìm tất cả các đối tượng Cloud và gọi phương thức Die() của chúng
        Cloud[] clouds = FindObjectsOfType<Cloud>();
        foreach (Cloud cloud in clouds)
        {
            cloud.Die(); // Kích hoạt cái chết của Cloud
        }

        Destroy(gameObject); // Phá hủy đối tượng Core
    }

    // Core mặc định không va chạm với người chơi để gây Game Over.
    // Nếu bạn muốn Core có thể gây sát thương khi va chạm với người chơi, hãy thêm đoạn mã sau:
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Core có thể gây sát thương cho người chơi khi va chạm (nếu bạn muốn)
            Debug.Log("[Core] Va chạm với người chơi!");
            // GameManager.Instance.PlayerDied(); // Kích hoạt Game Over nếu người chơi bị va chạm với Core
        }
    }
    */
}
