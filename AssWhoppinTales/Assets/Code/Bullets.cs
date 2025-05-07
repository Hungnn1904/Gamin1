using UnityEngine;

public class Bullets : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    private float speed = 10f;
    private float lifetime = 10f; // sống 10 giây
    private float timeAlive = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false); // Đảm bảo đạn tắt khi khởi tạo
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        rb.linearVelocity = direction * speed; // Sử dụng velocity thay vì linearVelocity
        timeAlive = 0f;

        // Lấy tất cả các viên đạn trong scene và tránh va chạm giữa chúng
        Bullets[] bullets = FindObjectsOfType<Bullets>();
        foreach (var bullet in bullets)
        {
            if (bullet != this) // Tránh va chạm với chính nó
            {
                // Tắt va chạm giữa viên đạn hiện tại và tất cả các viên đạn khác
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>(), true);
            }
        }
    }

    void Update()
    {
        timeAlive += Time.deltaTime;

        if (timeAlive >= lifetime)
        {
            DeactivateBullet(); // Sau 10 giây thì tắt đạn
        }
    }

    void DeactivateBullet()
    {
        BulletPool.Instance.ReturnBullet(this.gameObject);
    }

    // Bỏ hoàn toàn hàm OnTriggerEnter2D nếu không cần xử lý va chạm
}
