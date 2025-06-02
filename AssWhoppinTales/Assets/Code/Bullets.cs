using UnityEngine;

public class Bullets : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    public float speed = 30f;

    private float lifetime = 10f;
    private float timeAlive = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // KHÔNG tắt gameObject ở đây!
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        rb.WakeUp(); // Đảm bảo Rigidbody không bị sleep
        rb.linearVelocity = direction * speed;
        timeAlive = 0f;

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
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifetime)
        {
            DeactivateBullet();
        }
    }

    void DeactivateBullet()
    {
        BulletPool.Instance.ReturnBullet(this.gameObject);
    }
}
