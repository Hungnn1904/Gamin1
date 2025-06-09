// Nguồn file: Code/Cloud.cs
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float patrolSpeed = 3f; // Tốc độ di chuyển khi tuần tra
    public float chaseSpeed = 6f;  // Tốc độ di chuyển khi đuổi theo
    public float chaseDuration = 10f; // Thời gian đuổi theo người chơi

    // Cloud không có điểm số riêng vì chúng bất tử và chết theo Core
    // public int scoreValue = 0; // Không cần thiết

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float chaseTimer = 0f;

    public enum State { Patrol, Chase } // Trạng thái tuần tra hoặc đuổi theo
    public State currentState = State.Patrol;

    private Transform player;
    private Counter counter;

    // Các thuộc tính chỉ đọc để truy cập hướng di chuyển và người chơi
    public Vector2 MoveDirection => moveDirection;
    public Transform Player => player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        counter = Counter.Instance; // Lấy thể hiện của Counter
        ChooseRandomDirection(); // Chọn hướng di chuyển ngẫu nhiên ban đầu

        // Tìm đối tượng người chơi
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[Cloud] LỖI: Không tìm thấy GameObject của người chơi với tag 'Player' ở Start!");
    }

    void FixedUpdate()
    {
        // Cập nhật hành vi dựa trên trạng thái hiện tại
        switch (currentState)
        {
            case State.Patrol:
                rb.linearVelocity = moveDirection * patrolSpeed; // Di chuyển với tốc độ tuần tra
                break;

            case State.Chase:
                if (player != null)
                {
                    // Di chuyển về phía người chơi
                    Vector2 dirToPlayer = (player.position - transform.position).normalized;
                    rb.linearVelocity = dirToPlayer * chaseSpeed;
                }
                else
                {
                    rb.linearVelocity = Vector2.zero; // Dừng lại nếu không tìm thấy người chơi
                }
                chaseTimer -= Time.fixedDeltaTime; // Giảm thời gian đuổi theo
                if (chaseTimer <= 0)
                {
                    currentState = State.Patrol; // Chuyển về trạng thái tuần tra
                    ChooseRandomDirection(); // Chọn hướng ngẫu nhiên mới
                }
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Xử lý va chạm với người chơi
        if (collision.gameObject.CompareTag("Player"))
        {
            if (counter != null && !counter.IsInvincible)
            {
                Debug.Log("[Cloud] Va chạm với người chơi! Game Over!");
                GameManager.Instance.PlayerDied(); // Gọi GameManager khi người chơi bị va chạm
            }
            else
            {
                Debug.Log("[Cloud] Người chơi bất tử, không Game Over!");
            }
        }
        // Xử lý va chạm với các đối tượng khác để đổi hướng
        else if (currentState == State.Patrol || currentState == State.Chase)
        {
            BounceAway(); // Đổi hướng khi va chạm
        }
    }

    // Cloud bất tử, không bị chết bởi đạn trực tiếp.
    // Phương thức Die() này sẽ được gọi bởi Core khi Core chết.
    public void Die()
    {
        Debug.Log($"[Cloud] {gameObject.name} đã bị phá hủy vì Core đã chết!");
        // Cloud không cộng điểm khi bị tiêu diệt
        // GameManager.Instance.EnemyDefeated(); // Cloud bị diệt không tính vào tổng số kẻ địch cần diệt để qua màn (nếu có)
        Destroy(gameObject); // Phá hủy đối tượng Cloud
    }

    // Chọn một hướng di chuyển ngẫu nhiên
    void ChooseRandomDirection()
    {
        float angle = Random.Range(0f, 360f); // Góc ngẫu nhiên từ 0 đến 360 độ
        float radians = angle * Mathf.Deg2Rad; // Chuyển đổi sang radian
        moveDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized; // Tính toán hướng
    }

    // Đổi hướng khi va chạm (như bật ra)
    void BounceAway()
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg; // Góc hiện tại
        float newAngle = angle + 100f; // Thêm một góc để thay đổi hướng
        float radians = newAngle * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }

    // Bắt đầu đuổi theo người chơi
    public void StartChasing(Transform playerTransform)
    {
        player = playerTransform;
        currentState = State.Chase;
        chaseTimer = chaseDuration;
    }

    // Ngừng đuổi theo người chơi
    public void StopChasing()
    {
        player = null;
        currentState = State.Patrol;
        ChooseRandomDirection();
    }
}
