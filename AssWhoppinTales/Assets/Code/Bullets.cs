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
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        rb.WakeUp();
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemies"))
        {
            BasicZombie zombie = collision.gameObject.GetComponent<BasicZombie>();
            if (zombie != null)
            {
                zombie.Die();
                Debug.Log($"[Bullets] Zombie {collision.gameObject.name} destroyed!");
            }
            DeactivateBullet();
        }
    }

    void DeactivateBullet()
    {
        BulletPool.Instance.ReturnBullet(this.gameObject);
    }
}