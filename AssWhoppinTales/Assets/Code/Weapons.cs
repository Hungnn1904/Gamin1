using UnityEngine;
using UnityEngine.UI;

public class Weapons : MonoBehaviour
{
    [SerializeField] private string itemName; // Tên vũ khí
    [SerializeField] private Sprite weaponSprite; // Hình ảnh vũ khí
    [SerializeField] private Image weaponSlotUI; // UI hiển thị vũ khí

    [SerializeField] private SpriteRenderer playerWeaponSprite; // SpriteRenderer trên nhân vật

    [Header("Weapon Script")]
    [SerializeField] private MonoBehaviour weaponScript; // Gán script vũ khí qua Inspector

    private static Weapons currentWeapon; // Lưu vũ khí hiện tại

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (weaponSlotUI == null || playerWeaponSprite == null || weaponScript == null)
            {
                Debug.LogError("[Weapons] weaponSlotUI, playerWeaponSprite hoặc weaponScript chưa được gán!");
                return;
            }

            // Vô hiệu hóa các script vũ khí cũ
            DisableAllWeapons(collision.gameObject);

            // Cập nhật UI
            weaponSlotUI.sprite = weaponSprite;
            weaponSlotUI.enabled = true;

            // Cập nhật vũ khí trên nhân vật
            playerWeaponSprite.sprite = weaponSprite;
            playerWeaponSprite.enabled = true;

            // Kích hoạt script vũ khí tương ứng
            ActivateWeapon(collision.gameObject);

            // Lưu vũ khí mới
            currentWeapon = this;

            Debug.Log($"[Weapons] {itemName} đã được nhặt.");

            // Xóa vũ khí trên bản đồ
            Destroy(gameObject);
        }
    }

    // Vô hiệu hóa tất cả các script vũ khí của nhân vật
    private void DisableAllWeapons(GameObject player)
    {
        var pistol = player.GetComponent<PistolGun>();
        var rifle = player.GetComponent<Rifle>();
        var shotgun = player.GetComponent<Shotgun>();

        if (pistol != null) pistol.enabled = false;
        if (rifle != null) rifle.enabled = false;
        if (shotgun != null) shotgun.enabled = false;
    }

    // Kích hoạt script vũ khí đã gán qua Inspector
    private void ActivateWeapon(GameObject player)
    {
        if (weaponScript != null)
        {
            weaponScript.enabled = true; // Kích hoạt vũ khí đã gán
            Debug.Log($"[Weapons] {weaponScript.GetType().Name} activated.");
        }
    }
}
