using UnityEngine;

public class PlayerAwareness : MonoBehaviour
{
    public float viewAngle = 45f; // Góc nhìn 45 độ mỗi bên → hình nón 90 độ
    public float viewDistance = 10f;
    public LayerMask obstacleMask; // Layer của các vật cản như tường, cây cối,...
    public LayerMask playerMask;   // Layer của player

    private BasicZombie basicZombie;
    private Transform player;

    void Start()
    {
        basicZombie = GetComponent<BasicZombie>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= viewDistance)
        {
            Vector2 forward = Vector2.zero;

            if (basicZombie.currentState == BasicZombie.State.Patrol)
                forward = basicZombie.MoveDirection;
            else
                forward = (player.position - transform.position).normalized;

            float angleBetween = Vector2.Angle(forward, directionToPlayer);
            if (angleBetween <= viewAngle)
            {
                // Kiểm tra vật cản giữa enemy và player
                RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask);
                RaycastHit2D playerHit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, playerMask);

                // Nếu player được raycast trúng trước và không bị vật cản chặn
                if (playerHit.collider != null && (obstacleHit.collider == null || playerHit.distance < obstacleHit.distance))
                {
                    basicZombie.StartChasing(player);
                    return;
                }
            }
        }

        // Nếu không phát hiện player, gọi dừng đuổi
        if (basicZombie.currentState == BasicZombie.State.Chase)
        {
            basicZombie.StopChasing();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position;
        Vector3 forward = Vector3.right; // mặc định hướng mặt phải

        if (Application.isPlaying)
        {
            if (basicZombie != null)
            {
                if (basicZombie.currentState == BasicZombie.State.Patrol)
                    forward = basicZombie.MoveDirection;
                else if (basicZombie.currentState == BasicZombie.State.Chase && basicZombie.Player != null)
                    forward = (basicZombie.Player.position - transform.position).normalized;
            }
        }

        Gizmos.DrawRay(pos, Quaternion.Euler(0, 0, viewAngle) * forward * viewDistance);
        Gizmos.DrawRay(pos, Quaternion.Euler(0, 0, -viewAngle) * forward * viewDistance);
        Gizmos.DrawWireSphere(pos, viewDistance);
    }
}
