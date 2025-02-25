using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 3f; // Tốc độ chạy ngang
    public float climbForce = 3f; // Lực giúp nhân vật leo lên khi gặp vật cản
    public float detectionDistance = 0.5f; // Khoảng cách phát hiện vật cản phía trước
    private bool isGrounded;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1; // Trọng lực mặc định
    }

    void Update()
    {
        // Di chuyển ngang liên tục
        rb.velocity = new Vector2(speed, rb.velocity.y);
        
        // Kiểm tra vật cản phía trước
        CheckObstacle();

        // Kiểm tra phím Up hoặc Down để đổi trọng lực
        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            FlipGravity();
        }
    }

    void FlipGravity()
    {
        
        rb.gravityScale *= -1;

        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
    }

    void CheckObstacle()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, detectionDistance);

        if (hit.collider != null || isGrounded)
        {
            Debug.Log("Jump");

            // Xác định hướng trọng lực
            float gravityDirection = Mathf.Sign(rb.gravityScale); // 1 nếu trọng lực hướng xuống, -1 nếu trọng lực hướng lên

            // Khi gặp vật cản, đẩy nhân vật theo hướng trọng lực
            rb.velocity = new Vector2(rb.velocity.x, climbForce * -gravityDirection);
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
