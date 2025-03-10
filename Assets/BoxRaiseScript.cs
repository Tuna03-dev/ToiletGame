using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRaiseScript : MonoBehaviour
{
    public Transform player;  // Gán player vào trong Unity Editor
    public float triggerDistance = 3f; // Khoảng cách để thùng xuất hiện
    public float riseSpeed = 2f;  // Tốc độ đi lên
    public float riseHeight = 2f; // Chiều cao đi lên
    private Vector3 startPos;
    private bool isRising = false;

    void Start()
    {
        startPos = transform.position; // Lưu vị trí ban đầu
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= triggerDistance && !isRising)
        {
            StartCoroutine(RiseUp());
        }
    }

    IEnumerator RiseUp()
    {
        isRising = true;
        Vector3 targetPos = startPos + new Vector3(0, riseHeight, 0);
        while (transform.position.y < targetPos.y)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos; // Đảm bảo đúng vị trí
    }
}
