using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    private float baseMovementSpeed;
    private bool hasSpeedBoost;

    private float X, Y;
    private Rigidbody2D bd;
    private Animator animator;

    void Start()
    {
        bd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        baseMovementSpeed = movementSpeed;
    }

    void Update()
    {
        X = Input.GetAxisRaw("Horizontal");
        Y = Input.GetAxisRaw("Vertical");

        bool isWalking = (X != 0 || Y != 0);

        animator.SetBool("isWalking", isWalking); // Fixed: Removed extra parenthesis
        animator.SetFloat("InputX", X);
        animator.SetFloat("InputY", Y);

        if (isWalking)
        {
            animator.SetFloat("LastKeyPressX", X);
            animator.SetFloat("LastKeyPressY", Y); // Fixed: Removed comma and quote
        }
    }

    void FixedUpdate()
    {
        Vector2 direction = new Vector2(X, Y).normalized * (hasSpeedBoost ? baseMovementSpeed * 2f : baseMovementSpeed);
        bd.linearVelocity = direction;
    }

    public void SetSpeedBoost(bool enabled)
    {
        hasSpeedBoost = enabled;
        Debug.Log($"[PlayerMovement] Speed Boost {(enabled ? "enabled" : "disabled")}: Speed = {(hasSpeedBoost ? baseMovementSpeed * 2f : baseMovementSpeed)}");
    }
}