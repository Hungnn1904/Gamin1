using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public string itemName;
    public Sprite itemSprite;
    public bool isFull;

    [SerializeField] private Image itemImage;
    [SerializeField] private bool isHUDSlot = false; // Xác định slot là HUD hay Inventory

    private Vector2 inventoryItemSize = new Vector2(50, 60); // Kích cỡ trong Inventory
    private Vector2 hudItemSize = new Vector2(10, 12);       // Kích cỡ trong HUD

    void Start()
    {
        if (itemImage == null)
        {
            itemImage = GetComponentInChildren<Image>();
        }
    }

    public void AddItem(string itemName, Sprite itemSprite)
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        isFull = true;

        if (itemImage != null)
        {
            itemImage.sprite = itemSprite;
            itemImage.enabled = true;

            // Kiểm tra nếu là HUD slot thì set kích cỡ nhỏ hơn
            itemImage.rectTransform.sizeDelta = isHUDSlot ? hudItemSize : inventoryItemSize;
        }
        else
        {
            Debug.LogError("itemImage chưa được gán trong Inspector!");
        }
    }

    public void ClearSlot()
    {
        itemName = "";
        itemSprite = null;
        isFull = false;
        if (itemImage != null)
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }
    }
}