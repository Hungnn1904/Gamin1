using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    [SerializeField] private string itemName; // Weapon name
    [SerializeField] private Sprite weaponSprite; // Weapon image
    [SerializeField] private Image weaponSlotUI; // UI Image for inventory slot

    [SerializeField] private SpriteRenderer playerWeaponSprite; // 👈 New: SpriteRenderer for the weapon on Player

    [Header("Weapon Type (Only check one)")]
    [SerializeField] private bool isPistol;
    [SerializeField] private bool isRifle;
    [SerializeField] private bool isShotgun;

    private static Weapons currentWeapon; // Store current weapon

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (weaponSlotUI == null || playerWeaponSprite == null)
            {
                Debug.LogError("[Weapons] weaponSlotUI or playerWeaponSprite is not assigned!");
                return;
            }

            // Remove old weapon
            if (currentWeapon != null)
            {
                Destroy(currentWeapon.gameObject);
                Debug.Log($"[Weapons] {currentWeapon.itemName} removed.");
            }

            // Update UI slot
            weaponSlotUI.sprite = weaponSprite;
            weaponSlotUI.enabled = true;

            // Update Player's weapon sprite 👇
            playerWeaponSprite.sprite = weaponSprite;
            playerWeaponSprite.enabled = true; // Ensure it's visible

            // Adjust size & position if needed
            UpdateWeaponUI(weaponSlotUI.rectTransform);

            // Assign new weapon
            currentWeapon = this;

            Debug.Log($"[Weapons] {itemName} picked up and updated on player.");

            // Destroy weapon in game world
            Destroy(gameObject);
        }
    }

    private void UpdateWeaponUI(RectTransform uiTransform)
    {
        if (isPistol)
        {
            uiTransform.anchoredPosition = new Vector2(-90, 0);
            uiTransform.sizeDelta = new Vector2(100, 100);
        }
        else if (isRifle)
        {
            uiTransform.anchoredPosition = new Vector2(-50, 0);
            uiTransform.sizeDelta = new Vector2(250, 100);
        }
        else if (isShotgun)
        {
            uiTransform.anchoredPosition = new Vector2(-30, 0);
            uiTransform.sizeDelta = new Vector2(300, 100);
        }
    }
}
