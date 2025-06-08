using UnityEngine;
using TMPro;

public class Shotgun : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public float fireRate = 1.0f;
    private float nextFireTime = 0f;

    public int pelletCount = 6;
    public float spreadAngle = 30f;

    [Header("Ammo")]
    public int maxAmmo = 2;
    private int currentAmmo;
    private bool infiniteAmmo;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    void Awake()
    {
        this.enabled = false;
    }

    void OnEnable()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void OnDisable()
    {
        ResetAmmoUI();
    }

    void Update()
    {
        if (!this.enabled) return;

        UpdateGunDirection();

        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }
    }

    void TryShoot()
    {
        if (Time.time >= nextFireTime && (infiniteAmmo || currentAmmo > 0))
        {
            Shoot();
            if (!infiniteAmmo)
            {
                currentAmmo--;
                UpdateAmmoUI();
                if (currentAmmo <= 0)
                {
                    this.enabled = false;
                    ResetAmmoUI();
                    Debug.Log("[Shotgun] Hết đạn, tắt súng.");
                }
            }
            nextFireTime = Time.time + fireRate;
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = infiniteAmmo ? "∞/∞" : $"{currentAmmo}/{maxAmmo}";
    }

    void ResetAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = "-/-";
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
        float baseAngle = Mathf.Atan2(GetMouseDirection().y, GetMouseDirection().x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - spreadAngle / 2;
        float angleStep = spreadAngle / (pelletCount - 1);

        for (int i = 0; i < pelletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 shotDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject bullet = BulletPool.Instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.GetComponent<Rigidbody2D>().linearVelocity = shotDirection * 8f;
            bullet.SetActive(true);
        }
    }

    public void SetInfiniteAmmo(bool enabled)
    {
        infiniteAmmo = enabled;
        UpdateAmmoUI();
    }
}