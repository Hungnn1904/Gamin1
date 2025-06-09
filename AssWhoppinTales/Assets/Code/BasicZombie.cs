using UnityEngine;

public class BasicZombie : MonoBehaviour
{
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f;
    public float chaseDuration = 10f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float chaseTimer = 0f;

    public enum State { Patrol, Chase }
    public State currentState = State.Patrol;

    private Transform player;
    private Counter counter; // Đã đổi từ PowerUpManager sang Counter

    public Vector2 MoveDirection => moveDirection;
    public Transform Player => player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        counter = Counter.Instance; // Lấy instance của Counter
        ChooseRandomDirection();

        // Tìm người chơi khi bắt đầu
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[BasicZombie] LỖI: Không tìm thấy GameObject của người chơi với tag 'Player' ở Start! Đảm bảo người chơi có tag 'Player'.");
    }

    void FixedUpdate()
    {
        // Logic phát hiện người chơi có thể được thêm ở đây nếu BasicZombie cần phát hiện tầm nhìn
        // Hiện tại nó chưa có logic phát hiện tầm nhìn như Brute hay Assassin, chỉ có Chase khi được kích hoạt từ bên ngoài.

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
            // Kiểm tra Counter và trạng thái bất tử của người chơi
            if (counter != null && !counter.IsInvincible) // Đã đổi từ powerUpManager sang counter
            {
                Debug.Log("[BasicZombie] Va chạm với người chơi! Game Over!");
                GameManager.Instance.PlayerDied(); // Gọi GameManager để xử lý Game Over
            }
            else
            {
                Debug.Log("[BasicZombie] Người chơi bất tử, không Game Over!");
            }
        }
        else if (currentState == State.Patrol || currentState == State.Chase)
        {
            // Khi va chạm với vật thể khác (trong trạng thái tuần tra hoặc rượt đuổi), bật ra
            BounceAway();
        }
    }

    public void Die()
    {
        Debug.Log($"[Zombie] {gameObject.name} đã bị phá hủy!");
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

    // Các phương thức StartChasing và StopChasing (nếu được gọi từ các script khác)
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
