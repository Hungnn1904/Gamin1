using UnityEngine;

public class Bullets : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    private Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Get the Gun object
        GameObject gunObject = GameObject.FindWithTag("Gun");

        if (gunObject != null)
        {
            PistolGun gun = gunObject.GetComponent<PistolGun>();
            if (gun != null)
            {
                direction = gun.direction.normalized;
            }
            else
            {
                Debug.LogError("[Bullets] Gun script not found on the object with tag 'Gun'!");
                return;
            }
        }
        else
        {
            Debug.LogError("[Bullets] No object found with tag 'Gun'!");
            return;
        }

        rb.linearVelocity = direction * speed; // Fixed: Use 'velocity', not 'linearVelocity'
    }

    void Update()
    {
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
