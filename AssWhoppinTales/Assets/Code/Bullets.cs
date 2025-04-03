using UnityEngine;

public class Bullets : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    private float speed = 10f;
    private float lifetime = 20f;
    private float timeAlive = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false); // Ensure bullet is inactive when not in use
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        rb.linearVelocity = direction * speed;
        timeAlive = 0f; // Reset lifetime when bullet is fired
    }

    void Update()
    {
        if (timeAlive < lifetime)
        {
            timeAlive += Time.deltaTime; // Tăng thời gian sống
        }
        else
        {
            DeactivateBullet(); // Nếu hết thời gian sống, vô hiệu hóa đạn
        }
    }

    void DeactivateBullet()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            DeactivateBullet();
        }
    }
}
