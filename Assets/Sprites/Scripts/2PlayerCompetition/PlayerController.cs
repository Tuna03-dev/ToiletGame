using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float gravityScale = 3.0f;
    public float moveSpeed = 5f;
    public KeyCode UpKey;
    public KeyCode DownKey;
    private Rigidbody2D rb;
    private bool isGameOver = false;
    private bool isGameStarted = false;
    private bool isMoving = true;
    public Text countdownText;
    public Transform groundCheck;
    public Transform frontCheck;
    public LayerMask groundLayer;
    public GameObject fartEffect;
    public GameObject speedEffect;
    public GameObject fallingObjectPrefab; // Object rơi xuống
    public float spawnHeight = 5f;
    public float speedBoostDuration = 1f;
    private bool isSpeedBoosted = false;
    private CapsuleCollider2D capsuleCollider2D;
    private float originalSpeed;
    public AudioClip fartSound;
    public AudioClip paperSound;
    private AudioSource audioSource;
    private Animator animator;
    public AudioClip trapSound;

    void Start()
    {
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Lấy Animator component
        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;
        originalSpeed = moveSpeed;
        audioSource = GetComponent<AudioSource>(); 

        if (fartEffect != null)
        {
            fartEffect.SetActive(false);
        }
        if (fartEffect != null)
        {
            fartEffect.SetActive(false);
        }

        // Ẩn hiệu ứng tăng tốc ban đầu
        if (speedEffect != null)
        {
            speedEffect.SetActive(false);
        }
        Time.timeScale = 0;
        StartCoroutine(CountdownAndStart()); // Đếm ngược rồi bắt đầu game
    }

    IEnumerator CountdownAndStart()
    {
        countdownText.gameObject.SetActive(true); // Hiện UI đếm ngược
        for (int i = 1; i > 0; i--)
        {
            yield return new WaitForSecondsRealtime(1f); // Chờ 1 giây (không bị ảnh hưởng bởi Time.timeScale)
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.gameObject.SetActive(false); // Ẩn UI khi game bắt đầu
        Time.timeScale = 1; // Bắt đầu game
        isGameStarted = true;
    }

    void Update()
    {
        if (!isGameStarted || isGameOver) return;

        // Kiểm tra nếu nhân vật va vào tile gồ ghề phía trước
        if (IsObstacleAhead())
        {
            isMoving = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            isMoving = true;
        }

        if (isMoving)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

        if (IsOnGround())
        {
            if (Input.GetKeyDown(UpKey))
            {
                PlayFartEffect();
                ReverseGravity(-Mathf.Abs(gravityScale), 180, 180);
            }
            else if (Input.GetKeyDown(DownKey))
            {
                PlayFartEffect();
                ReverseGravity(Mathf.Abs(gravityScale), 0, 0);
            }
        }
        if (isSpeedBoosted)
        {
            Invoke("ResetSpeed", speedBoostDuration);
        }

        // Kiểm tra game over
        CheckGameOver();
    }
    void PlayFartEffect()
    {
        if (fartEffect != null)
        {
            fartEffect.SetActive(true);
            Invoke("HideFartEffect", 0.5f);
        }

        if (audioSource != null && fartSound != null)
        {
            audioSource.PlayOneShot(fartSound); // Phát âm thanh fart
        }
    }
    void ReverseGravity(float newGravity, float y, float rotation)
    {
        rb.gravityScale = newGravity;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        transform.rotation = Quaternion.Euler(0, y, rotation);

    }

    bool IsOnGround()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);
    }

    bool IsObstacleAhead()
    {
        return Physics2D.Raycast(frontCheck.position, Vector2.right, 0.2f, groundLayer);
    }

    void CheckGameOver()
    {
        float cameraX = Camera.main.transform.position.x;
        if (transform.position.x < cameraX - 17f) GameOver();

        float cameraY = Camera.main.transform.position.y;
        float screenHeight = Camera.main.orthographicSize;
        if (transform.position.y > cameraY + screenHeight + 3f || transform.position.y < cameraY - screenHeight - 2f)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        rb.velocity = Vector2.zero;
        Time.timeScale = 0;
        
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void HideFartEffect()
    {
        if (fartEffect != null)
        {
            fartEffect.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Paper") && !isSpeedBoosted)
        {
            Debug.Log("Player ăn Paper! Buff Speed!");
            ActivateSpeedBoost();

            
            if (audioSource != null && paperSound != null)
            {
                audioSource.PlayOneShot(paperSound);
            }

           
            Destroy(other.gameObject);
        }

        

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bird"))
        {
            Debug.Log("Player va vào bird!");

            originalSpeed = moveSpeed;
            moveSpeed = -3;

            Invoke("RestoreMoveSpeed", 0.5f);
        }

        if (collision.gameObject.CompareTag("Trap"))
        {
            Debug.Log("Player va vào trap!");
            if (audioSource != null && trapSound != null)
            {
                audioSource.PlayOneShot(trapSound); // Phát âm thanh va vào trap
            }
            animator.SetBool("die", true);
            moveSpeed = -5;

            // Gửi thông báo đến Camera để dừng cuộn
            CameraScroll cameraScript = Camera.main.GetComponent<CameraScroll>();
            if (cameraScript != null)
            {
                cameraScript.StopScrolling();
            }

            // **Reset gravity về hướng xuống dưới**
            rb.gravityScale = Mathf.Abs(gravityScale); // Đảm bảo gravity luôn dương
            transform.rotation = Quaternion.identity; // Reset xoay nhân vật về mặc định

            // Dừng nhân vật sau 1 giây
            Invoke("StopPlayer", 1f);

            // Chỉnh collider nằm ngang sau khi chết
            capsuleCollider2D.direction = CapsuleDirection2D.Horizontal;
            capsuleCollider2D.size = new Vector2(5.646687f, 2.417982f);
            capsuleCollider2D.offset = new Vector2(0.15F, 0.04399896f);

            // Gọi object rơi sau khi player đã dừng lại
            Invoke("SpawnFallingObject", 1.2f);
        }
    }

    void SpawnFallingObject()
    {
        if (fallingObjectPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + spawnHeight, transform.position.z);
            GameObject fallingObject = Instantiate(fallingObjectPrefab, spawnPosition, Quaternion.identity);

            // Thêm Rigidbody2D để rơi tự nhiên
            Rigidbody2D rb = fallingObject.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = fallingObject.AddComponent<Rigidbody2D>();
            }
            rb.gravityScale = 2; // Điều chỉnh tốc độ rơi theo ý bạn
        }
    }

    // Hàm để camera focus vào nhân vật theo trục X
    void FocusCameraOnPlayer()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.position = targetPosition; 
    }



    // Phương thức để dừng nhân vật lại
    void StopPlayer()
    {
        moveSpeed = 0;
        rb.velocity = Vector2.zero;
        FocusCameraOnPlayer();
    }




    // Thêm hàm này để khôi phục tốc độ di chuyển sau khi bị va chạm
    void RestoreMoveSpeed()
    {
        moveSpeed = originalSpeed;
    }

    void ActivateSpeedBoost()
    {
        isSpeedBoosted = true;
        moveSpeed *= 5; // Tăng tốc độ gấp đôi

        // Hiển thị hiệu ứng tăng tốc
        if (speedEffect != null)
        {
            speedEffect.SetActive(true);
        }

        // Ẩn hiệu ứng sau thời gian tăng tốc
        Invoke("HideSpeedEffect", speedBoostDuration);
    }

    void ResetSpeed()
    {
        moveSpeed = originalSpeed;
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
