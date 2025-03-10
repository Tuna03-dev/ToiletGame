using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public float scrollSpeed = 3f; // Tốc độ cuộn màn hình
    public Transform player; // Đối tượng nhân vật
    public GameObject boundaryObject; // GameObject chứa vùng giới hạn

    private Collider2D boundaryCollider; // Collider của vùng giới hạn
    private Bounds boundaryBounds; // Giới hạn vùng di chuyển
    private bool isScrolling = true; // Kiểm soát cuộn camera

    void Start()
    {
        if (boundaryObject != null)
        {
            boundaryCollider = boundaryObject.GetComponent<Collider2D>(); // Lấy Collider2D từ GameObject
            if (boundaryCollider == null)
            {
                Debug.LogError("Boundary Object không có Collider2D!");
            }
            else
            {
                boundaryBounds = boundaryCollider.bounds; // Lưu giới hạn vùng
            }
        }
        else
        {
            Debug.LogError("Boundary Object chưa được gán!");
        }
    }

    void Update()
    {
        if (isScrolling)
        {
            // Di chuyển camera theo chiều ngang
            Vector3 nextPosition = transform.position + Vector3.right * scrollSpeed * Time.deltaTime;

            // Giới hạn camera trong phạm vi của boundary
            if (boundaryCollider != null)
            {
                float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
                float maxX = boundaryBounds.max.x - cameraHalfWidth;
                float minX = boundaryBounds.min.x + cameraHalfWidth;

                // Giới hạn vị trí camera
                nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
            }

            transform.position = nextPosition;
        }

        // Kiểm tra xem nhân vật có ra khỏi màn hình không
        if (player != null)
        {
            CheckPlayerOutOfScreen();
        }
    }

    void CheckPlayerOutOfScreen()
    {
        // Lấy biên của camera
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)); // Mép trái camera
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 0)); // Mép phải camera
        Vector3 bottomEdge = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)); // Mép dưới camera

        // Kiểm tra nếu player nằm ngoài màn hình
        if (player.position.x < leftEdge.x || player.position.y < bottomEdge.y || player.position.x > rightEdge.x)
        {
            Debug.Log("Player bị ra khỏi màn hình -> Chết!");
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        // Reset hoặc xử lý chết nhân vật
        Destroy(player.gameObject); // Xóa nhân vật (hoặc thay thế bằng reset)
    }

    public void StopScrolling()
    {
        isScrolling = false;
    }
}
