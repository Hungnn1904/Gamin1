using UnityEngine;
using TMPro;

public class Rifle : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public float fireRate = 0.167f;
    private float nextFireTime = 0f;

    [Header("Ammo")]
    public int maxAmmo = 15;
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

        if (Input.GetButton("Fire1"))
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
                    Debug.Log("[Rifle] Hết đạn, tắt súng.");
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
        GameObject bullet = BulletPool.Instance.GetBullet();
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.GetComponent<Bullets>().SetDirection(GetMouseDirection());
        bullet.SetActive(true);
    }

    public void SetInfiniteAmmo(bool enabled)
    {
        infiniteAmmo = enabled;
        UpdateAmmoUI();
    }
}