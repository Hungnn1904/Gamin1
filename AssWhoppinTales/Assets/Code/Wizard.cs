using UnityEngine;

public class Wizard : MonoBehaviour
{
    // Thuộc tính di chuyển và rượt đuổi
    public float patrolSpeed = 3f;
    public float chaseSpeed = 6f; // Tốc độ rượt đuổi của Wizard
    public float teleportDistance = 3f; // Khoảng cách từ người chơi sau khi dịch chuyển
    public float chaseDuration = 10f; // Thời gian rượt đuổi
    public float bulletSpeed = 10f; // Tốc độ đạn có thể tùy chỉnh trong Inspector
    public GameObject bulletPrefab; // Prefab đạn để gán trong Inspector
    public float shootInterval = 1f; // Thời gian giữa các lần bắn
    public float detectionRadius = 10f; // Bán kính phát hiện hình tròn
    public LayerMask playerMask; // LayerMask để phát hiện người chơi
    public LayerMask obstacleMask; // LayerMask cho chướng ngại vật

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float chaseTimer = 0f;
    private float shootTimer = 0f;
    private Transform player;
    private Counter counter; // Đã đổi từ PowerUpManager sang Counter
    private bool hasTeleported = false; // Theo dõi xem đã dịch chuyển chưa (trong một chu kỳ chase)
    private Collider2D selfCollider; // Tham chiếu đến collider của wizard

    public enum State { Patrol, Chase }
    public State currentState = State.Patrol;

    public Vector2 MoveDirection => moveDirection;
    public Transform Player => player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<Collider2D>();
        counter = Counter.Instance; // Lấy instance của Counter
        ChooseRandomDirection(); // Bắt đầu tuần tra theo hướng ngẫu nhiên khi khởi tạo

        if (bulletPrefab == null)
        {
            Debug.LogError("[Wizard] LỖI: Bullet Prefab chưa được gán trong Inspector!");
        }

        // Tìm người chơi khi bắt đầu
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[Wizard] LỖI: Không tìm thấy người chơi! Đảm bảo người chơi có tag 'Player'.");
    }

    void FixedUpdate()
    {
        // Logic phát hiện người chơi sử dụng bán kính tròn
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Kiểm tra xem người chơi có trong bán kính phát hiện không
            if (distanceToPlayer <= detectionRadius)
            {
                // Tính toán hướng tới người chơi
                Vector2 directionToPlayer = (player.position - transform.position).normalized;
                // Thực hiện Raycast để kiểm tra chướng ngại vật giữa Wizard và người chơi
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask | playerMask);

                // Nếu raycast trúng một collider và collider đó thuộc layer người chơi
                // và đó thực sự là người chơi (để tránh các collider khác trên cùng layer)
                if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & playerMask) != 0 && hit.collider.gameObject == player.gameObject)
                {
                    if (currentState != State.Chase)
                    {
                        StartChasing(player); // Kích hoạt trạng thái rượt đuổi
                        Debug.Log("[Wizard] Phát hiện người chơi trong phạm vi tròn! Bắt đầu rượt đuổi và dịch chuyển!");
                    }
                }
                else if (currentState == State.Chase)
                {
                    // Nếu đang ở trạng thái Chase nhưng mất tầm nhìn hoặc bị chặn
                    StopChasing();
                    Debug.Log("[Wizard] Người chơi ra khỏi tầm nhìn hoặc bị chặn. Dừng rượt đuổi.");
                }
            }
            else if (currentState == State.Chase)
            {
                // Nếu người chơi ra khỏi bán kính phát hiện
                StopChasing();
                Debug.Log("[Wizard] Người chơi ra khỏi bán kính phát hiện. Dừng rượt đuổi.");
            }
        }

        // Xử lý di chuyển và bắn đạn dựa trên trạng thái hiện tại
        switch (currentState)
        {
            case State.Patrol:
                rb.linearVelocity = moveDirection * patrolSpeed;
                break;

            case State.Chase:
                if (player != null)
                {
                    Vector2 dirToPlayer = (player.position - transform.position).normalized;
                    // Wizard sẽ đuổi theo người chơi với chaseSpeed
                    rb.linearVelocity = dirToPlayer * chaseSpeed;

                    // Logic bắn đạn
                    shootTimer -= Time.fixedDeltaTime;
                    if (shootTimer <= 0f)
                    {
                        ShootBullet();
                        shootTimer = shootInterval;
                    }
                }
                else
                {
                    rb.linearVelocity = Vector2.zero; // Dừng lại nếu người chơi bị mất trong quá trình rượt đuổi
                }
                chaseTimer -= Time.fixedDeltaTime; // Đếm ngược thời gian rượt đuổi
                if (chaseTimer <= 0)
                {
                    currentState = State.Patrol; // Quay lại trạng thái tuần tra sau khi hết thời gian rượt đuổi
                    ChooseRandomDirection(); // Chọn hướng tuần tra mới
                    Debug.Log("[Wizard] Thời gian rượt đuổi hết. Quay lại tuần tra.");
                }
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bỏ qua va chạm với đạn của chính wizard
        if (collision.gameObject.CompareTag("WizardBullet"))
        {
            Debug.Log($"[Wizard] Bỏ qua va chạm với đạn của chính nó: {collision.gameObject.name}");
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (counter != null && !counter.IsInvincible) // Đã đổi từ powerUpManager sang counter
            {
                Debug.Log("[Wizard] Va chạm với người chơi! Game Over!");
                GameManager.Instance.PlayerDied(); // Gọi GameManager để xử lý Game Over
            }
            else
            {
                Debug.Log("[Wizard] Người chơi bất tử, không Game Over!");
            }
        }
        else if (collision.gameObject.CompareTag("Gun")) // Giả sử "Gun" là tag của đạn người chơi
        {
            Debug.Log($"[Wizard] Bị trúng đạn người chơi! {gameObject.name} đã bị phá hủy!");
            Die(); // Gọi Die() khi bị trúng đạn
        }
        else if (currentState == State.Patrol) // Chỉ bật ra khi đang tuần tra
        {
            Debug.Log($"[Wizard] Bật ra khỏi: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
            BounceAway();
        }
    }

    public void Die()
    {
        Debug.Log($"[Wizard] {gameObject.name} đã bị phá hủy!");
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
        shootTimer = 0f; // Đặt lại timer bắn đạn để bắn ngay sau khi vào trạng thái Chase
        hasTeleported = false; // Đặt lại cờ dịch chuyển cho mỗi chu kỳ chase
        TeleportNearPlayer(); // Dịch chuyển đến gần người chơi khi bắt đầu rượt đuổi
    }

    public void StopChasing()
    {
        player = null;
        currentState = State.Patrol;
        ChooseRandomDirection();
        hasTeleported = false; // Đặt lại cờ dịch chuyển khi dừng rượt đuổi
    }

    void TeleportNearPlayer()
    {
        if (player == null) return;

        // Tính toán một vị trí ngẫu nhiên xung quanh người chơi ở khoảng cách teleportDistance
        float randomAngle = Random.Range(0f, 360f);
        Vector2 offset = new Vector2(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            Mathf.Sin(randomAngle * Mathf.Deg2Rad)
        ) * teleportDistance;

        Vector2 targetPosition = (Vector2)player.position + offset;

        // Đảm bảo wizard không dịch chuyển vào trong người chơi hoặc các vật thể khác quá gần
        float minDistance = 1f; // Khoảng cách tối thiểu để tránh chồng chéo với người chơi
        if (Vector2.Distance(targetPosition, player.position) < minDistance)
        {
            targetPosition = (Vector2)player.position + (offset.normalized * minDistance);
        }

        transform.position = targetPosition;
        hasTeleported = true; // Đánh dấu rằng wizard đã dịch chuyển
        // Không gọi ShootBullet() ở đây nữa vì nó sẽ được quản lý bởi shootTimer trong FixedUpdate
    }

    void ShootBullet()
    {
        if (player == null || bulletPrefab == null) return;

        // Sử dụng BulletPool để quản lý đạn của wizard
        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.tag = "WizardBullet"; // Gắn tag khác cho đạn của wizard để tránh va chạm với chính nó
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

        // Bỏ qua va chạm giữa wizard và đạn của chính nó
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), selfCollider, true);
    }

    void OnDrawGizmos()
    {
        // Vẽ bán kính phát hiện trong Scene view để debug
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
