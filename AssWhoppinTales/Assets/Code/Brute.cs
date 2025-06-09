// Nguồn file: Code/Brute.cs
using UnityEngine;

public class Brute : MonoBehaviour
{
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float chaseDuration = 10f;
    public float viewAngle = 45f;
    public float viewDistance = 10f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    // === DÒNG MỚI: Thêm biến điểm số ===
    public int scoreValue = 30;

    private int health = 3;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float chaseTimer = 0f;
    private Transform player;
    private Collider2D selfCollider;
    private Counter counter;

    public enum State { Patrol, Chase }
    public State currentState = State.Patrol;

    public Vector2 MoveDirection => moveDirection;
    public Transform Player => player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<Collider2D>();
        counter = Counter.Instance;
        
        if (counter == null)
        {
            Debug.LogError("[Brute] LỖI: Counter.Instance is null.");
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[Brute] LỖI: Không tìm thấy người chơi!");

        ChooseRandomDirection();
    }

    void Update()
    {
        if (player == null) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.red);

        if (distanceToPlayer <= viewDistance)
        {
            Vector2 facingDirection = transform.right;
            float angleBetween = Vector2.Angle(facingDirection, directionToPlayer);
            
            if (angleBetween <= viewAngle * 0.5f)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer, distanceToPlayer, playerMask | obstacleMask);
                bool playerDetected = false;
                float nearestObstacleDistance = Mathf.Infinity;

                foreach (var hit in hits)
                {
                    if (hit.collider == selfCollider) continue;
                    if (((1 << hit.collider.gameObject.layer) & obstacleMask) != 0)
                    {
                        if (hit.distance < nearestObstacleDistance)
                            nearestObstacleDistance = hit.distance;
                    }
                    else if (((1 << hit.collider.gameObject.layer) & playerMask) != 0)
                    {
                        if (hit.distance < nearestObstacleDistance)
                        {
                            playerDetected = true;
                            break;
                        }
                    }
                }
                
                if (playerDetected)
                {
                    StartChasing(player);
                    return;
                }
            }
        }
        
        if (currentState == State.Chase)
        {
            StopChasing();
        }
    }

    void FixedUpdate()
    {
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
        if (collision.gameObject.CompareTag("Player"))
        {
            if (counter != null && !counter.IsInvincible)
            {
                GameManager.Instance.PlayerDied();
            }
        }
        else if (currentState == State.Patrol || currentState == State.Chase)
        {
            BounceAway();
        }
    }

    // === HÀM ĐÃ SỬA: Thêm logic báo cáo cho GameManager ===
    public void Die()
    {
        health--;
        if (health <= 0)
        {
            Debug.Log($"[Brute] {gameObject.name} đã bị phá hủy!");
            
            // Gọi GameManager để cộng điểm và báo cáo kẻ địch bị diệt
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreValue);
                GameManager.Instance.EnemyDefeated();
            }

            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"[Brute] {gameObject.name} bị trúng đạn! Máu còn lại: {health}");
        }
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

    void StartChasing(Transform playerTransform)
    {
        player = playerTransform;
        currentState = State.Chase;
        chaseTimer = chaseDuration;
    }

    void StopChasing()
    {
        player = null;
        currentState = State.Patrol;
        ChooseRandomDirection();
    }

    void OnDrawGizmos()
    {
        if (player == null) return;
        Vector2 facingDirection = transform.right;
        float halfAngle = viewAngle * 0.5f;
        Vector2 leftRay = Quaternion.Euler(0, 0, -halfAngle) * facingDirection;
        Vector2 rightRay = Quaternion.Euler(0, 0, halfAngle) * facingDirection;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftRay * viewDistance);
        Gizmos.DrawRay(transform.position, rightRay * viewDistance);
    }
}