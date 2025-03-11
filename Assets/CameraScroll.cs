using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    public float scrollSpeed = 3f;
    public Transform player; 
    public GameObject boundaryObject;
    public GameObject dieEffectPrefab; // Prefab của hiệu ứng die
    private Collider2D boundaryCollider;
    private Bounds boundaryBounds;
    private bool isScrolling = true;
    private bool isEffectSpawned = false; // Kiểm tra nếu hiệu ứng đã được tạo
    private Vector3 playerLastPosition; // Vị trí cuối cùng của player trước khi ra khỏi màn hình

    void Start()
    {
        // Kiểm tra xem player có được gán trong Inspector chưa
        if (boundaryObject != null)
        {
            boundaryCollider = boundaryObject.GetComponent<Collider2D>();
            if (boundaryCollider == null)
            {
                Debug.LogError("Boundary Object không có Collider2D!");
            }
            else
            {
                boundaryBounds = boundaryCollider.bounds;
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
        // Lấy biên của camera (theo chiều ngang và dọc)
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)); // Mép trái camera
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 0)); // Mép phải camera
        Vector3 bottomEdge = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)); // Mép dưới camera
        Vector3 topEdge = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1, 0)); // Mép trên camera

        // Kiểm tra nếu player nằm ngoài màn hình
        if (player.position.x < leftEdge.x || player.position.x > rightEdge.x || player.position.y < bottomEdge.y || player.position.y > topEdge.y)
        {
            // Lưu lại vị trí của player trước khi ra khỏi màn hình
            playerLastPosition = player.position;
            Debug.Log("Player bị ra khỏi màn hình -> Chết!");
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        if (!isEffectSpawned && dieEffectPrefab != null)
        {
            // Instantiate hiệu ứng die tại vị trí cuối cùng của player
            GameObject dieEffect = Instantiate(dieEffectPrefab, playerLastPosition, Quaternion.identity);

            // Phát âm thanh của hiệu ứng die
            AudioSource dieEffectAudio = dieEffect.GetComponent<AudioSource>();
            if (dieEffectAudio != null)
            {
                dieEffectAudio.Play(); // Phát âm thanh khi nhân vật chết
            }

            isEffectSpawned = true; // Đánh dấu là hiệu ứng đã được tạo
        }

        // Dừng cuộn camera
        StopScrolling();

        // Làm nhân vật biến mất
        player.gameObject.SetActive(false); // Ẩn nhân vật (dùng player.gameObject thay vì player)

        // Gọi hàm Game Over trong CommonPlayer
        CommonPlayer playerScript = player.GetComponent<CommonPlayer>();
        if (playerScript != null)
        {
            playerScript.GameOver(); // Thực hiện game over
        }
    }




    public void StopScrolling()
    {
        isScrolling = false;
    }
}
