// Nguồn file: Code/BasicZombie.cs
using UnityEngine;

public class BasicZombie : MonoBehaviour
{
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float chaseDuration = 10f;

    // === DÒNG MỚI: Thêm biến điểm số ===
    public int scoreValue = 10;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float chaseTimer = 0f;

    public enum State { Patrol, Chase }
    public State currentState = State.Patrol;

    private Transform player;
    private Counter counter;

    public Vector2 MoveDirection => moveDirection;
    public Transform Player => player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        counter = Counter.Instance;
        ChooseRandomDirection();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[BasicZombie] LỖI: Không tìm thấy GameObject của người chơi với tag 'Player' ở Start!");
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
                Debug.Log("[BasicZombie] Va chạm với người chơi! Game Over!");
                GameManager.Instance.PlayerDied();
            }
            else
            {
                Debug.Log("[BasicZombie] Người chơi bất tử, không Game Over!");
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
        Debug.Log($"[Zombie] {gameObject.name} đã bị phá hủy!");

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
    }

    public void StopChasing()
    {
        player = null;
        currentState = State.Patrol;
        ChooseRandomDirection();
    }
}