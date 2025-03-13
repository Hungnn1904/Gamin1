using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;

    private float X, Y;
    
    private Rigidbody2D bd;
    
    //handling animations
    private Animator animator;

    void Start()
    {
        bd = GetComponent<Rigidbody2D>();

        // Get the Animator component 
        animator = GetComponent<Animator>();
    }

    void Update()
{
    X = Input.GetAxisRaw("Horizontal");
    Y = Input.GetAxisRaw("Vertical");

    bool isWalking = (X != 0 || Y != 0);

    animator.SetBool("isWalking", isWalking);

    animator.SetFloat("InputX", X);
    animator.SetFloat("InputY", Y);

    // ✅ Store last direction *only if moving*
    if (isWalking) 
    {
        animator.SetFloat("LastKeyPressX", X);
        animator.SetFloat("LastKeyPressY", Y);
    }
}


    void FixedUpdate()
    {
        //so that you can use fast 1399 movement 
        Vector2 movement = new Vector2(X, Y).normalized * movementSpeed;

        bd.linearVelocity = movement; //  This could be `bd.velocity`, aint trying it tho
    }
}
