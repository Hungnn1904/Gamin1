using UnityEngine;

public class Bullets : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    public float speed = 30f; // Tốc độ viên đạn

    private float lifetime = 10f; // Thời gian tồn tại của viên đạn
    private float timeAlive = 0f; // Thời gian viên đạn đã tồn tại

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Đặt hướng di chuyển cho viên đạn
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        rb.WakeUp(); // Đảm bảo Rigidbody2D hoạt động
        rb.linearVelocity = direction * speed; // Đặt vận tốc cho viên đạn
        timeAlive = 0f; // Reset thời gian tồn tại

        // Bỏ qua va chạm giữa các viên đạn với nhau để tránh lỗi
        Bullets[] bullets = FindObjectsOfType<Bullets>();
        foreach (var bullet in bullets)
        {
            if (bullet != this)
            {
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>(), true);
            }
        }
    }

    void Update()
    {
        timeAlive += Time.deltaTime; // Tăng thời gian tồn tại
        if (timeAlive >= lifetime)
        {
            DeactivateBullet(); // Tắt viên đạn nếu hết thời gian tồn tại
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Chỉ xử lý va chạm với các đối tượng có Tag là "Enemies"
        if (collision.gameObject.CompareTag("Enemies"))
        {
            // Kiểm tra va chạm với BasicZombie (nếu vẫn còn trong game của bạn)
            BasicZombie zombie = collision.gameObject.GetComponent<BasicZombie>();
            if (zombie != null)
            {
                zombie.Die(); // Gọi phương thức Die() của BasicZombie
                Debug.Log($"[Bullets] Zombie {collision.gameObject.name} destroyed!");
                DeactivateBullet(); // Tắt viên đạn sau khi va chạm
                return; // Thoát khỏi hàm để không xử lý tiếp cho cùng một va chạm
            }

            // Kiểm tra va chạm với Brute (nếu vẫn còn trong game của bạn)
            Brute brute = collision.gameObject.GetComponent<Brute>();
            if (brute != null)
            {
                brute.Die(); // Gọi phương thức Die() của Brute
                Debug.Log($"[Bullets] Brute {collision.gameObject.name} hit!");
                DeactivateBullet(); // Tắt viên đạn sau khi va chạm
                return; // Thoát khỏi hàm
            }

            // === DÒNG MỚI: Xử lý va chạm với Core ===
            Core core = collision.gameObject.GetComponent<Core>();
            if (core != null)
            {
                core.Die(); // Gọi phương thức Die() của Core, Core sẽ tự phá hủy và kích hoạt cái chết của Cloud
                Debug.Log($"[Bullets] Core {collision.gameObject.name} destroyed!");
                DeactivateBullet(); // Tắt viên đạn sau khi va chạm với Core
                return; // Thoát khỏi hàm
            }

            // === DÒNG MỚI: Xử lý va chạm với Cloud ===
            Cloud cloud = collision.gameObject.GetComponent<Cloud>();
            if (cloud != null)
            {
                // Cloud bất tử với đạn, KHÔNG gọi Die() ở đây.
                Debug.Log($"[Bullets] Cloud {collision.gameObject.name} hit but is immune to bullets!");
                DeactivateBullet(); // Viên đạn vẫn bị tắt sau khi va chạm với Cloud
                return; // Thoát khỏi hàm
            }

            // Nếu va chạm với bất kỳ đối tượng nào khác có tag "Enemies" mà không phải là BasicZombie, Brute, Core, hoặc Cloud,
            // thì viên đạn vẫn bị tắt.
            DeactivateBullet();
        }
    }

    // Đưa viên đạn trở lại Bullet Pool
    void DeactivateBullet()
    {
        // Đảm bảo BulletPool.Instance tồn tại trước khi gọi ReturnBullet
        if (BulletPool.Instance != null)
        {
            BulletPool.Instance.ReturnBullet(this.gameObject);
        }
        else
        {
            // Nếu không có BulletPool, đơn giản là Destroy đối tượng
            Destroy(this.gameObject);
        }
    }
}
