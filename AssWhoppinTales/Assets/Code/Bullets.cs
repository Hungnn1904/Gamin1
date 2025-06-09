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

        // Bỏ qua va chạm giữa các viên đạn với nhau
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
        if (collision.gameObject.CompareTag("Enemies")) // Nếu va chạm với kẻ địch (có tag "Enemies")
        {
            BasicZombie zombie = collision.gameObject.GetComponent<BasicZombie>(); // Thử lấy component BasicZombie
            if (zombie != null)
            {
                zombie.Die(); // Gọi phương thức Die() của BasicZombie
                Debug.Log($"[Bullets] Zombie {collision.gameObject.name} destroyed!"); // Ghi log Zombie bị phá hủy
            }

            Brute brute = collision.gameObject.GetComponent<Brute>(); // Thử lấy component Brute
            if (brute != null)
            {
                brute.Die(); // Gọi phương thức Die() của Brute
                Debug.Log($"[Bullets] Brute {collision.gameObject.name} hit!"); // Ghi log Brute bị trúng đạn
            }

            DeactivateBullet(); // Tắt viên đạn sau khi va chạm
        }
    }

    // Đưa viên đạn trở lại Bullet Pool
    void DeactivateBullet()
    {
        BulletPool.Instance.ReturnBullet(this.gameObject);
    }
}
