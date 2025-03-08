using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public float scrollSpeed = 3f; // Tốc độ cuộn màn hình
    public Transform player; // Đối tượng nhân vật
    public float killZoneOffset = 2f; // Khoảng cách giữa cạnh trái màn hình và vùng giết nhân vật

    private Camera mainCamera;
    private float killZoneX;
    private bool isScrolling = true; // Biến kiểm soát việc cuộn camera

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        // Kiểm tra nếu đang cuộn camera
        if (isScrolling)
        {
            // Di chuyển camera theo tốc độ cuộn
            transform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);
        }

        // Kiểm tra xem người chơi có vượt ra khỏi vùng giết nhân vật không
        if (player != null)
        {
            CheckPlayerPosition();
        }
    }

    void CheckPlayerPosition()
    {
        // Nếu nhân vật đi ra khỏi vùng giết nhân vật (bên trái màn hình)
        if (player.position.x < killZoneX)
        {
            Debug.Log("Player would be killed here!");
        }
    }

    // Phương thức dừng cuộn camera
    public void StopScrolling()
    {
        isScrolling = false;
    }
}
