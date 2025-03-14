using UnityEngine;
using UnityEngine.Cinemachine;

public class Transpoint : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D mapBoundary;
    [SerializeField] private Direction direction;

    private CinemachineConfiner2D confiner;

    public enum Direction { Up, Down, Left, Right }

    private void Awake()
    {
        // Tìm CinemachineConfiner2D trong scene
        confiner = FindObjectOfType<CinemachineConfiner2D>();
        if (confiner == null)
        {
            Debug.LogError("CinemachineConfiner2D không tìm thấy trong scene!");
        }

        // Lấy PolygonCollider2D từ GameObject hiện tại
        mapBoundary = GetComponent<PolygonCollider2D>();
        if (mapBoundary == null)
        {
            Debug.LogError("PolygonCollider2D không được gán trên GameObject!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (confiner != null && mapBoundary != null)
            {
                // Cập nhật bounding shape cho CinemachineConfiner2D
                confiner.m_BoundingShape2D = mapBoundary;
            }
            UpdatePlayerPosition(other.gameObject);
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;
        switch (direction)
        {
            case Direction.Up:
                newPos.y += 1;
                break;
            case Direction.Down:
                newPos.y -= 1;
                break;
            case Direction.Left:
                newPos.x -= 1;
                break;
            case Direction.Right:
                newPos.x += 1;
                break;
        }
        player.transform.position = newPos;
    }
}
