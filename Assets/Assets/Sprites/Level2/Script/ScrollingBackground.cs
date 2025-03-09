using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public float scrollSpeed = 10f; // Tốc độ cuộn của map

    void Update()
    {
        // Dịch chuyển nền từ phải sang trái
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        //// Nếu object ra khỏi màn hình, hủy nó để tối ưu hiệu suất
        //if (transform.position.x < -15f) // -15 là giới hạn màn hình bên trái
        //{
        //    Destroy(gameObject);
        //}
    }
}
