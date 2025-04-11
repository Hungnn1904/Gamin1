using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            string path = Application.dataPath + "/Screenshots/screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("Đã chụp ảnh tại: " + path);
        }
    }
}
