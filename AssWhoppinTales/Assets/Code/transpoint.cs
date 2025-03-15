using Unity.Cinemachine;
using UnityEngine;

public class Transpoint : MonoBehaviour
{
    // Nếu gán mapBoundary từ Inspector thì dùng trực tiếp,
    // nếu không, sẽ tự động lấy từ GameObject.
    [SerializeField] private PolygonCollider2D mapBoundary;
    [SerializeField] private CinemachineConfiner confiner;
    [SerializeField] private Direction direction;
    public enum Direction { Up, Down, Left, Right }

    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Cập nhật bounding shape cho CinemachineConfiner2D.
           confiner.m_BoundingShape2D = mapBoundary;
            // Cập nhật vị trí người chơi.
            UpdatePlayerPosition(other.gameObject);
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;
        switch (direction)
        {
            case Direction.Up:
                newPos.y += 2;
                break;
            case Direction.Down:
                newPos.y -= 2;
                break;
            case Direction.Left:
                newPos.x -= 2;
                break;
            case Direction.Right:
                newPos.x += 2;
                break;
        }
        player.transform.position = newPos;
    }
}
