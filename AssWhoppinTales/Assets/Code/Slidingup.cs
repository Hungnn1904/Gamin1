using UnityEngine;

public class Slidingup : MonoBehaviour
{
    public float scrollspeed = 80f;
    private RectTransform rectTransform;
    void Start(){
        rectTransform = GetComponent<RectTransform>();

    }

    void Update()
    {
        rectTransform.anchoredPosition +=new Vector2(0, scrollspeed*Time.deltaTime);
    }
}
