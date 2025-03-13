using UnityEngine;

public class LimitFPS : MonoBehaviour
{
    public int targetFPS = 60; // Set your desired FPS limit

    void Start()
    {
        Application.targetFrameRate = targetFPS;
    }

    void OnDisable()
    {
        Application.targetFrameRate = -1; // Reset to default when leaving scene
    }
}
