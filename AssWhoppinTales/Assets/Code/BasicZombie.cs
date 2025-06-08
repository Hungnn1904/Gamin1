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
    private PowerUpManager powerUpManager;

    public Vector2 MoveDirection => moveDirection;
    public Transform Player => player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        powerUpManager = PowerUpManager.Instance;
        ChooseRandomDirection();
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
            if (powerUpManager != null && !powerUpManager.IsInvincible)
            {
                Debug.Log("[Zombie] Collided with Player! Game Over!");
                Time.timeScale = 0f;
            }
            else
            {
                Debug.Log("[Zombie] Player is invincible, no game over!");
            }
        }
        else if (currentState == State.Patrol || currentState == State.Chase)
        {
            BounceAway();
        }
    }

    public void Die()
    {
        Debug.Log($"[Zombie] {gameObject.name} destroyed!");
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