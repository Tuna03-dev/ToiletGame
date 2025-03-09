using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 3f; // Tốc độ chạy ngang
    public float climbForce = 3f; // Lực giúp nhân vật leo lên khi gặp vật cản
    public float detectionDistance = 0.2f; // Khoảng cách phát hiện vật cản phía trước
    private bool isGrounded;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public GameObject fartEffect; // Hiệu ứng fart (gán trong Inspector)
    public GameObject speedEffect; // Hiệu ứng khi tăng tốc (gán trong Inspector)

    public float speedBoostDuration = 1f; // Thời gian tăng tốc sau khi va chạm cuộn giấy
    private bool isSpeedBoosted = false; // Kiểm tra trạng thái tăng tốc

    private float originalSpeed; // Lưu lại tốc độ ban đầu

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Lấy SpriteRenderer
        rb.gravityScale = 1; // Trọng lực mặc định
        originalSpeed = speed; // Lưu tốc độ ban đầu

        // Ẩn hiệu ứng fart ban đầu
        if (fartEffect != null)
        {
            fartEffect.SetActive(false);
        }

        // Ẩn hiệu ứng tăng tốc ban đầu
        if (speedEffect != null)
        {
            speedEffect.SetActive(false);
        }
    }

    void Update()
    {
        // Di chuyển ngang liên tục
        rb.velocity = new Vector2(speed, rb.velocity.y);

        // Kiểm tra nếu nhấn phím Up/Down để đổi trọng lực
        if (Input.GetKeyDown(KeyCode.UpArrow) && rb.gravityScale > 0)
        {
            FlipGravity();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && rb.gravityScale < 0)
        {
            FlipGravity();
        }

        // Nếu đang tăng tốc, tự động giảm tốc sau thời gian boost
        if (isSpeedBoosted)
        {
            Invoke("ResetSpeed", speedBoostDuration);
        }
    }

    void FlipGravity()
    {
        rb.gravityScale *= -1;
        spriteRenderer.flipY = !spriteRenderer.flipY;

        // Hiện hiệu ứng fart khi đổi trọng lực
        if (fartEffect != null)
        {
            fartEffect.SetActive(true);
            Invoke("HideFartEffect", 0.5f); // Ẩn sau 0.5 giây
        }
    }

    void HideFartEffect()
    {
        if (fartEffect != null)
        {
            fartEffect.SetActive(false);
        }
    }

    // Khi nhân vật đi qua cuộn giấy (Trigger)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Paper") && !isSpeedBoosted)
        {
            Debug.Log("Player đi qua cuộn giấy!");
            ActivateSpeedBoost();

            // Ẩn hoặc phá hủy cuộn giấy
            Destroy(other.gameObject);
        }
    }

    // Hàm tăng tốc
    void ActivateSpeedBoost()
    {
        isSpeedBoosted = true;
        speed *= 5; // Tăng tốc độ gấp đôi

        // Hiển thị hiệu ứng tăng tốc
        if (speedEffect != null)
        {
            speedEffect.SetActive(true);
        }

        // Ẩn hiệu ứng sau thời gian tăng tốc
        Invoke("HideSpeedEffect", speedBoostDuration);
    }

    // Hàm reset tốc độ sau khi tăng tốc
    void ResetSpeed()
    {
        speed = originalSpeed;
        isSpeedBoosted = false;
    }

    // Hàm ẩn hiệu ứng tăng tốc
    void HideSpeedEffect()
    {
        if (speedEffect != null)
        {
            speedEffect.SetActive(false);
        }
    }
}
