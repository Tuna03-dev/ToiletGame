using System.Collections;
using UnityEngine;

public class BoxRaise : MonoBehaviour
{
    public Transform player;  // Gán player vào trong Unity Editor
    public float triggerDistance = 3f; // Khoảng cách để thùng xuất hiện
    public float moveSpeed = 2f;  // Tốc độ di chuyển
    public float moveDistance = 2f; // Khoảng cách di chuyển
    public bool isRisingUp = true; // Chọn hướng di chuyển (true: lên, false: xuống)

    private Vector3 startPos;
    private bool isMoving = false;

    void Start()
    {
        startPos = transform.position; // Lưu vị trí ban đầu
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= triggerDistance && !isMoving)
        {
            StartCoroutine(MoveBox());
        }
    }

    IEnumerator MoveBox()
    {
        isMoving = true;

        // Xác định hướng di chuyển
        float direction = isRisingUp ? 1f : -1f;
        Vector3 targetPos = startPos + new Vector3(0, moveDistance * direction, 0);

        while ((isRisingUp && transform.position.y < targetPos.y) ||
               (!isRisingUp && transform.position.y > targetPos.y))
        {
            transform.position += Vector3.up * moveSpeed * direction * Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // Đảm bảo đúng vị trí
    }
}
