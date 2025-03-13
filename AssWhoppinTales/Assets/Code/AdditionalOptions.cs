using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdditionalOptions : MonoBehaviour
{
    public List<GameObject> subButtons; // Danh sách nút con
    private Button mainButton;          // Nút chính
    private bool isExpanded = false;    // Trạng thái hiện/ẩn

    void Start()
    {
        mainButton = GetComponent<Button>();

        if (mainButton != null)
        {
            mainButton.onClick.AddListener(Toggle);
        }

        // Ẩn tất cả các nút con lúc đầu
        SetSubButtonsActive(false);
    }

    private void Toggle()
    {
        isExpanded = !isExpanded;
        SetSubButtonsActive(isExpanded);
    }

    private void SetSubButtonsActive(bool state)
    {
        foreach (GameObject btn in subButtons)
        {
            if (btn != null) btn.SetActive(state);
        }
    }
}
