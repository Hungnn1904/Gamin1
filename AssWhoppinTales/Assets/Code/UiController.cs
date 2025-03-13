using UnityEngine;

public class UiController : MonoBehaviour
{
    public GameObject menuCanvas;

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)){
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }
}
