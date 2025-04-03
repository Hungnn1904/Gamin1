using UnityEngine;

public class Rifle : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public float fireRate = 0.167f; // Fast fire rate (3x Pistol)
    private float nextFireTime = 0f;

    void Awake()
    {
        this.enabled = false; // Disable script at start
    }

    void Update()
    {
        if (this.enabled)
        {
            UpdateGunDirection();

            if (Input.GetButton("Fire1"))
            {
                TryShoot();
            }
        }
    }

    void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void UpdateGunDirection()
    {
        Vector2 direction = GetMouseDirection();
        transform.right = direction;
    }

    Vector2 GetMouseDirection()
    {
        Vector2 gunPos = transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePos - gunPos;
    }

    public void Shoot()
    {
        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.GetComponent<Bullets>().SetDirection(GetMouseDirection());
        bullet.SetActive(true);
    }
}
