using UnityEngine;

public class MenuButtonSwitching : MonoBehaviour
{
    public GameObject[] tabs; 
    public int currentTab = 0; 

    void Start()
    {
        UpdateTabs();
    }

    public void NextTab()
    {
        currentTab = (currentTab + 1) % tabs.Length;
        UpdateTabs();
    }

    public void PreviousTab()
    {
        currentTab = (currentTab - 1 + tabs.Length) % tabs.Length;
        UpdateTabs();
    }

    void UpdateTabs()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(i == currentTab);
        }
    }
}
