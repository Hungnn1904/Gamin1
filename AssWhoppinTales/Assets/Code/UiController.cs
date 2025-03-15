using UnityEngine;

public class UiController : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject hudCanvas; // Thêm HUD vào Inspector

    void Start()
    {
        menuCanvas.SetActive(false);
        hudCanvas.SetActive(true); // Đảm bảo HUD ban đầu hiển thị
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isMenuActive = !menuCanvas.activeSelf;
            menuCanvas.SetActive(isMenuActive);
            hudCanvas.SetActive(!isMenuActive); // Ẩn HUD khi menu hiển thị, hiện lại khi menu tắt
        }
    }
}
