using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private static PowerUpManager instance;
    public static PowerUpManager Instance => instance;

    private bool isInvincible;
    private bool hasInfiniteAmmo;
    private bool hasSpeedBoost;
    private float powerUpTimer;
    private const float POWER_UP_DURATION = 30f;

    private PlayerMovement playerMovement;
    private PistolGun pistolGun;
    private Rifle rifle;
    private Shotgun shotgun;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        playerMovement = FindObjectOfType<PlayerMovement>();
        pistolGun = FindObjectOfType<PistolGun>();
        rifle = FindObjectOfType<Rifle>();
        shotgun = FindObjectOfType<Shotgun>();
    }

    void Update()
    {
        if (powerUpTimer > 0)
        {
            powerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0)
            {
                DisablePowerUps();
            }
        }
    }

    public void ActivatePowerUp(string powerUpType)
    {
        powerUpTimer = POWER_UP_DURATION;

        switch (powerUpType)
        {
            case "Invincibility":
                isInvincible = true;
                Debug.Log("[PowerUpManager] Invincibility activated for 30 seconds!");
                break;
            case "InfiniteAmmo":
                hasInfiniteAmmo = true;
                if (pistolGun != null) pistolGun.SetInfiniteAmmo(true);
                if (rifle != null) rifle.SetInfiniteAmmo(true);
                if (shotgun != null) shotgun.SetInfiniteAmmo(true);
                Debug.Log("[PowerUpManager] Infinite Ammo activated for 30 seconds!");
                break;
            case "SpeedBoost":
                hasSpeedBoost = true;
                if (playerMovement != null) playerMovement.SetSpeedBoost(true);
                Debug.Log("[PowerUpManager] Speed Boost activated for 30 seconds!");
                break;
        }
    }

    private void DisablePowerUps()
    {
        if (isInvincible)
        {
            isInvincible = false;
            Debug.Log("[PowerUpManager] Invincibility deactivated!");
        }
        if (hasInfiniteAmmo)
        {
            hasInfiniteAmmo = false;
            if (pistolGun != null) pistolGun.SetInfiniteAmmo(false);
            if (rifle != null) rifle.SetInfiniteAmmo(false);
            if (shotgun != null) shotgun.SetInfiniteAmmo(false);
            Debug.Log("[PowerUpManager] Infinite Ammo deactivated!");
        }
        if (hasSpeedBoost)
        {
            hasSpeedBoost = false;
            if (playerMovement != null) playerMovement.SetSpeedBoost(false);
            Debug.Log("[PowerUpManager] Speed Boost deactivated!");
        }
    }

    public bool IsInvincible => isInvincible;
}