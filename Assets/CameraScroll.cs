using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public float scrollSpeed = 3f; // Tốc độ cuộn màn hình
    public Transform player; // Đối tượng nhân vật
    public float killZoneOffset = 2f; // Khoảng cách giữa cạnh trái màn hình và vùng giết nhân vật

    private Camera mainCamera;
    private float killZoneX;

    void Start()
    {
        // Lấy component Camera
        mainCamera = GetComponent<Camera>();

        // Tính toán vị trí ban đầu của vùng giết nhân vật
        CalculateKillZone();
    }

    void Update()
    {
        // Di chuyển camera theo tốc độ cuộn
        transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);

        // Cập nhật vị trí vùng giết nhân vật
        CalculateKillZone();

        // Kiểm tra xem người chơi có vượt ra khỏi vùng giết nhân vật không
        if (player != null)
        {
            CheckPlayerPosition();
        }
    }

    void CalculateKillZone()
    {
        // Tính toán vị trí X của vùng giết nhân vật (cạnh trái của màn hình + offset)
        float cameraLeftEdge = transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;
        killZoneX = cameraLeftEdge + killZoneOffset;
    }

    void CheckPlayerPosition()
    {
        // Nếu nhân vật đi ra khỏi vùng giết nhân vật (bên trái màn hình)
        if (player.position.x < killZoneX)
        {
            // Tạm thời chỉ in thông báo trong console
            Debug.Log("Player would be killed here!");

            // Khi bạn đã tạo phương thức Die(), hãy gọi:
            // PlayerScript playerScript = player.GetComponent<PlayerScript>();
            // if (playerScript != null)
            // {
            //     playerScript.Die();
            // }
        }
    }

    // Hiển thị vùng giết nhân vật trong Editor
    void OnDrawGizmos()
    {
        if (Application.isPlaying && mainCamera != null)
        {
            Gizmos.color = Color.red;
            float cameraHeight = 2f * mainCamera.orthographicSize;
            Gizmos.DrawLine(
                new Vector3(killZoneX, transform.position.y - cameraHeight / 2, 0),
                new Vector3(killZoneX, transform.position.y + cameraHeight / 2, 0)
            );
        }
    }
}
