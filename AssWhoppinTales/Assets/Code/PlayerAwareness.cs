using UnityEngine;

public class PlayerAwareness : MonoBehaviour
{
    public float viewAngle = 45f;
    public float viewDistance = 10f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    private BasicZombie basicZombie;
    private Transform player;
    private Collider2D selfCollider;

    void Start()
    {
        basicZombie = GetComponent<BasicZombie>();
        selfCollider = GetComponent<Collider2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[ERROR] Player not found! Ensure Player has tag 'Player'.");
    }

    void Update()
    {
        if (player == null) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.red);

        if (distanceToPlayer <= viewDistance)
        {
            Vector2 facingDirection = transform.right; // Hướng zombie đang nhìn
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
                    Debug.Log("[INFO] Player spotted! Start chasing!");
                    basicZombie.StartChasing(player);
                    return;
                }
            }
        }

        if (basicZombie.currentState == BasicZombie.State.Chase)
        {
            Debug.Log("[INFO] Lost sight of player. Stop chasing.");
            basicZombie.StopChasing();
        }
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