using UnityEngine;

public class Brute : MonoBehaviour
{
    // Thuộc tính di chuyển và rượt đuổi
    public float patrolSpeed = 3f; // Tốc độ tuần tra
    public float chaseSpeed = 6f;  // Tốc độ rượt đuổi
    public float chaseDuration = 10f; // Thời gian rượt đuổi

    // Thuộc tính nhận biết người chơi
    public float viewAngle = 45f; // Góc nhìn
    public float viewDistance = 10f; // Khoảng cách nhìn
    public LayerMask obstacleMask; // Layer chướng ngại vật
    public LayerMask playerMask;   // Layer người chơi

    private int health = 3; // Brute cần 3 hit để chết
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float chaseTimer = 0f;
    private Transform player;
    private Collider2D selfCollider;
    private Counter counter; // Đã đổi từ PowerUpManager sang Counter

    public enum State { Patrol, Chase } // Các trạng thái của kẻ địch
    public State currentState = State.Patrol;

    // Getter công khai cho trạng thái nội bộ (hữu ích để debug hoặc các script bên ngoài)
    public Vector2 MoveDirection => moveDirection;
    public Transform Player => player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<Collider2D>();

        // Cố gắng lấy thể hiện của Counter
        counter = Counter.Instance; // Đã đổi từ PowerUpManager.Instance sang Counter.Instance
        if (counter == null)
        {
            Debug.LogError("[Brute] LỖI: Counter.Instance is null. Đảm bảo Counter được thiết lập đúng cách là một Singleton.");
        }

        // Tìm người chơi theo tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[Brute] LỖI: Không tìm thấy người chơi! Đảm bảo người chơi có tag 'Player'.");

        ChooseRandomDirection(); // Bắt đầu tuần tra theo hướng ngẫu nhiên
    }

    void Update()
    {
        if (player == null) return; // Không làm gì nếu không tìm thấy người chơi

        // Tính toán hướng và khoảng cách tới người chơi
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Hiển thị tia dò người chơi trong editor để debug
        Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.red);

        // Kiểm tra xem người chơi có trong khoảng cách nhìn không
        if (distanceToPlayer <= viewDistance)
        {
            // Xác định hướng nhìn của Brute (giả sử nó quay mặt sang phải theo mặc định)
            Vector2 facingDirection = transform.right;
            float angleBetween = Vector2.Angle(facingDirection, directionToPlayer);

            // Kiểm tra xem người chơi có trong góc nhìn không
            if (angleBetween <= viewAngle * 0.5f)
            {
                // Thực hiện Raycast để kiểm tra chướng ngại vật hoặc người chơi
                // Kết hợp layer của người chơi và chướng ngại vật để raycast hiệu quả
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer, distanceToPlayer, playerMask | obstacleMask);

                bool playerDetected = false;
                float nearestObstacleDistance = Mathf.Infinity;

                // Lặp qua tất cả các hit để tìm chướng ngại vật hoặc người chơi gần nhất
                foreach (var hit in hits)
                {
                    if (hit.collider == selfCollider) continue; // Bỏ qua collider của chính nó

                    // Kiểm tra nếu đối tượng bị hit là chướng ngại vật
                    if (((1 << hit.collider.gameObject.layer) & obstacleMask) != 0)
                    {
                        if (hit.distance < nearestObstacleDistance)
                            nearestObstacleDistance = hit.distance;
                    }
                    // Kiểm tra nếu đối tượng bị hit là người chơi
                    else if (((1 << hit.collider.gameObject.layer) & playerMask) != 0)
                    {
                        // Nếu người chơi bị hit và gần hơn bất kỳ chướng ngại vật nào đã tìm thấy
                        if (hit.distance < nearestObstacleDistance)
                        {
                            playerDetected = true;
                            break; // Tìm thấy người chơi, không cần kiểm tra thêm
                        }
                    }
                }

                // Nếu người chơi được phát hiện và không bị chặn bởi chướng ngại vật, bắt đầu rượt đuổi
                if (playerDetected)
                {
                    Debug.Log("[Brute] Phát hiện người chơi! Bắt đầu rượt đuổi!");
                    StartChasing(player);
                    return; // Thoát Update loop vì trạng thái đã thay đổi
                }
            }
        }

        // Nếu đang rượt đuổi nhưng người chơi không còn được nhìn thấy (ra khỏi tầm nhìn hoặc bị chặn), dừng rượt đuổi
        if (currentState == State.Chase)
        {
            Debug.Log("[Brute] Mất dấu người chơi. Dừng rượt đuổi.");
            StopChasing();
        }
    }

    void FixedUpdate()
    {
        // Xử lý di chuyển dựa trên trạng thái hiện tại
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
                    rb.linearVelocity = Vector2.zero; // Dừng lại nếu người chơi bị mất trong quá trình rượt đuổi
                }
                chaseTimer -= Time.fixedDeltaTime; // Đếm ngược thời gian rượt đuổi
                if (chaseTimer <= 0)
                {
                    currentState = State.Patrol; // Quay lại trạng thái tuần tra sau khi hết thời gian rượt đuổi
                    ChooseRandomDirection(); // Chọn hướng tuần tra mới
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
                Debug.Log("[Brute] Va chạm với người chơi! Game Over!");
                GameManager.Instance.PlayerDied(); // Gọi GameManager để xử lý Game Over
            }
            else
            {
                Debug.Log("[Brute] Người chơi bất tử, không Game Over!");
            }
        }
        else if (currentState == State.Patrol || currentState == State.Chase)
        {
            // Khi va chạm với vật thể khác (trong trạng thái tuần tra hoặc rượt đuổi), bật ra
            BounceAway();
        }
    }

    // Giảm máu của Brute và hủy nó nếu máu về không
    public void Die()
    {
        health--; // Giảm máu
        if (health <= 0)
        {
            Debug.Log($"[Brute] {gameObject.name} đã bị phá hủy!");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"[Brute] {gameObject.name} bị trúng đạn! Máu còn lại: {health}");
        }
    }

    // Đặt hướng tuần tra ngẫu nhiên mới
    void ChooseRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        float radians = angle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }

    // Thay đổi hướng 100 độ (hữu ích để bật ra khỏi tường/chướng ngại vật)
    void BounceAway()
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        float newAngle = angle + 100f; // Bật ra khoảng 100 độ so với hướng hiện tại
        float radians = newAngle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }

    // Bắt đầu trạng thái rượt đuổi
    void StartChasing(Transform playerTransform)
    {
        player = playerTransform;
        currentState = State.Chase;
        chaseTimer = chaseDuration;
    }

    // Kết thúc trạng thái rượt đuổi và quay lại tuần tra
    void StopChasing()
    {
        player = null; // Xóa tham chiếu người chơi
        currentState = State.Patrol;
        ChooseRandomDirection(); // Tiếp tục tuần tra theo hướng mới
    }

    // Vẽ Gizmos trong editor để trực quan hóa góc nhìn và khoảng cách
    void OnDrawGizmos()
    {
        if (player == null) return; // Chỉ vẽ nếu người chơi tồn tại

        Vector2 facingDirection = transform.right; // Giả sử Brute quay mặt sang phải
        float halfAngle = viewAngle * 0.5f;

        // Tính toán hai tia cho hình nón tầm nhìn
        Vector2 leftRay = Quaternion.Euler(0, 0, -halfAngle) * facingDirection;
        Vector2 rightRay = Quaternion.Euler(0, 0, halfAngle) * facingDirection;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftRay * viewDistance);
        Gizmos.DrawRay(transform.position, rightRay * viewDistance);
    }
}
