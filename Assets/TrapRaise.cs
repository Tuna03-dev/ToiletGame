using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapRaise : MonoBehaviour
{
    public Transform player;  // Gán player vào trong Unity Editor
    public float triggerDistance = 3f; // Khoảng cách để thùng xuất hiện
    public float moveSpeed = 2f;  // Tốc độ di chuyển
    public float moveDistance = 2f; // Khoảng cách di chuyển ngang
    public bool isMovingRight = true; // Chọn hướng (true: sang phải, false: sang trái)

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

        // Xác định hướng di chuyển (1: phải, -1: trái)
        float direction = isMovingRight ? 1f : -1f;
        Vector3 targetPos = startPos + new Vector3(moveDistance * direction, 0, 0);

        while ((isMovingRight && transform.position.x < targetPos.x) ||
               (!isMovingRight && transform.position.x > targetPos.x))
        {
            transform.position += Vector3.right * moveSpeed * direction * Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // Đảm bảo đúng vị trí
    }
}
