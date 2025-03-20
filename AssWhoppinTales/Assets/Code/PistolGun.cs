using UnityEngine;

public class PistolGun : MonoBehaviour
{
    public Vector2 direction;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float fireRate = 0.5f; // Time between shots (0.5s = 2 shots per second)
    private float nextFireTime = 0f;

    void Update()
    {
        Vector2 gunPos = transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePos - gunPos;
        transform.right = direction; // Make gun face the mouse

        if (Input.GetButtonDown("Fire1")) // Fires once per click
        {
            TryShoot();
        }
    }

    void TryShoot()
    {
        if (Time.time >= nextFireTime) // Check if enough time has passed
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // Set next allowed fire time
        }
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * 10f;
    }
}
