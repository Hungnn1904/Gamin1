// Nguồn file: Code/Wizard.cs
using UnityEngine;

public class Wizard : MonoBehaviour
{
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float teleportDistance = 3f;
    public float chaseDuration = 10f;
    public float bulletSpeed = 10f;
    public GameObject bulletPrefab;
    public float shootInterval = 1f;
    public float detectionRadius = 10f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    // === DÒNG MỚI: Thêm biến điểm số ===
    public int scoreValue = 50;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float chaseTimer = 0f;
    private float shootTimer = 0f;
    private Transform player;
    private Counter counter;
    private bool hasTeleported = false;
    private Collider2D selfCollider;

    public enum State { Patrol, Chase }
    public State currentState = State.Patrol;

    public Vector2 MoveDirection => moveDirection;
    public Transform Player => player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<Collider2D>();
        counter = Counter.Instance;
        ChooseRandomDirection();

        if (bulletPrefab == null)
        {
            Debug.LogError("[Wizard] LỖI: Bullet Prefab chưa được gán trong Inspector!");
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[Wizard] LỖI: Không tìm thấy người chơi!");
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRadius)
            {
                Vector2 directionToPlayer = (player.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask | playerMask);
                
                if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & playerMask) != 0 && hit.collider.gameObject == player.gameObject)
                {
                    if (currentState != State.Chase)
                    {
                        StartChasing(player);
                    }
                }
                else if (currentState == State.Chase)
                {
                    StopChasing();
                }
            }
            else if (currentState == State.Chase)
            {
                StopChasing();
            }
        }

        switch (currentState)
        {
            case State.Patrol:
                rb.linearVelocity = moveDirection * patrolSpeed;
                break;

            case State.Chase:
                if (player != null)
                {
                    Vector2 dirToPlayer = (player.position - transform.position).normalized;
                    rb.linearVelocity = dirToPlayer * chaseSpeed;

                    shootTimer -= Time.fixedDeltaTime;
                    if (shootTimer <= 0f)
                    {
                        ShootBullet();
                        shootTimer = shootInterval;
                    }
                }
                else
                {
                    rb.linearVelocity = Vector2.zero;
                }
                chaseTimer -= Time.fixedDeltaTime;
                if (chaseTimer <= 0)
                {
                    currentState = State.Patrol;
                    ChooseRandomDirection();
                }
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("WizardBullet"))
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (counter != null && !counter.IsInvincible)
            {
                GameManager.Instance.PlayerDied();
            }
        }
        else if (collision.gameObject.CompareTag("Gun"))
        {
            Die();
        }
        else if (currentState == State.Patrol)
        {
            BounceAway();
        }
    }

    // === HÀM ĐÃ SỬA: Thêm logic báo cáo cho GameManager ===
    public void Die()
    {
        Debug.Log($"[Wizard] {gameObject.name} đã bị phá hủy!");

        // Gọi GameManager để cộng điểm và báo cáo kẻ địch bị diệt
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
            GameManager.Instance.EnemyDefeated();
        }

        Destroy(gameObject);
    }

    void ChooseRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        float radians = angle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }

    void BounceAway()
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        float newAngle = angle + 100f;
        float radians = newAngle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }

    public void StartChasing(Transform playerTransform)
    {
        player = playerTransform;
        currentState = State.Chase;
        chaseTimer = chaseDuration;
        shootTimer = 0f;
        hasTeleported = false;
        TeleportNearPlayer();
    }

    public void StopChasing()
    {
        player = null;
        currentState = State.Patrol;
        ChooseRandomDirection();
        hasTeleported = false;
    }

    void TeleportNearPlayer()
    {
        if (player == null) return;
        
        float randomAngle = Random.Range(0f, 360f);
        Vector2 offset = new Vector2(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            Mathf.Sin(randomAngle * Mathf.Deg2Rad)
        ) * teleportDistance;
        
        Vector2 targetPosition = (Vector2)player.position + offset;
        
        float minDistance = 1f;
        if (Vector2.Distance(targetPosition, player.position) < minDistance)
        {
            targetPosition = (Vector2)player.position + (offset.normalized * minDistance);
        }
        
        transform.position = targetPosition;
        hasTeleported = true;
    }

    void ShootBullet()
    {
        if (player == null || bulletPrefab == null) return;
        
        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.tag = "WizardBullet";
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            bulletRb.linearVelocity = directionToPlayer * bulletSpeed;
            bullet.SetActive(true);
        }
        else
        {
            Debug.LogError("[Wizard] LỖI: Prefab đạn thiếu Rigidbody2D!");
        }
        
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), selfCollider, true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}