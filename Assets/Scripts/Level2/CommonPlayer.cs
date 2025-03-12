using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CommonPlayer : MonoBehaviour
{
    public float countdownTime = 3.0f;
    public float gravityScale = 3.0f;
    public float moveSpeed = 5f;
    public KeyCode UpKey;
    public KeyCode DownKey;
    private Rigidbody2D rb;
    private bool isGameOver = false;
    private bool isGameStarted = false;
    private bool isMoving = true;
    public Transform groundCheck;
    public Transform frontCheck;
    public LayerMask groundLayer;
    public GameObject laser;
    public GameObject fartEffect;
    public GameObject speedEffect;
    public float speedBoostDuration = 1f;
    private bool isSpeedBoosted = false;
    private CapsuleCollider2D capsuleCollider2D;
    private float originalSpeed;
    public AudioClip fartSound;
    public AudioClip paperSound;
    public AudioClip victorySound;
    private AudioSource audioSource;
    private Animator animator;
    public AudioClip trapSound;
    public GameObject gameOverCanvas;
    public GameObject gameNextCanvas;
    public GameObject gameWinCanvas;
    public GameObject sparkleEffect;
    public GameObject pauseButton;
    private bool isVictory;
    void Start()
    {
        isVictory = false;
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        if (gameNextCanvas != null)
        {
            gameNextCanvas.SetActive(false);
        }
        if (gameWinCanvas != null)
        {
            gameWinCanvas.SetActive(false);
        }
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;
        originalSpeed = moveSpeed;
        audioSource = GetComponent<AudioSource>();

        if (sparkleEffect != null)
        {
            sparkleEffect.SetActive(false);
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

        StartCoroutine(CountdownAndStart()); // Đếm ngược rồi bắt đầu game
    }

    IEnumerator CountdownAndStart()
    {
        // Đếm ngược mà không sử dụng Time.timeScale, sử dụng WaitForSecondsRealtime
        
        while (countdownTime > 0)
        {
            countdownTime -= Time.unscaledDeltaTime;  // Dùng Time.unscaledDeltaTime thay vì Time.deltaTime
            yield return null;  // Tiếp tục lặp cho đến khi đếm ngược xong
        }

        // Sau khi đếm xong
        isGameStarted = true;
        laser.SetActive(false);
        if (animator != null)
        {
            Debug.Log("Run");
            animator.SetBool("run", true);
        }
    }



    void Update()
    {
        if (!isGameStarted || isGameOver || Time.timeScale == 0) return;

        // Kiểm tra nếu nhân vật va vào tile gồ ghề phía trước
        if (IsObstacleAhead())
        {
            isMoving = false;
           
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
        float radius = 0.3f; // Bán kính hình tròn, bạn có thể điều chỉnh theo nhu cầu
        RaycastHit2D hit = Physics2D.CircleCast(frontCheck.position, radius, Vector2.right, 0.2f, groundLayer);

        return hit.collider != null;
    }

   public bool CheckVictory()
    {
        return isVictory;
    }

    public void GameOver()
    {
        isGameOver = true;
        rb.velocity = Vector2.zero;

        // Dừng nhân vật và tắt hiệu ứng
        animator.SetBool("run", false);
        if (fartEffect != null) fartEffect.SetActive(false);
        if (speedEffect != null) speedEffect.SetActive(false);

        // Hiển thị Canvas GameOver
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            pauseButton.SetActive(false);
        }

        
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
        if (other.gameObject.CompareTag("Teleport"))
        {
            Debug.Log("Player va vào Teleport!");
            
            gameObject.SetActive(false);

            if (sparkleEffect != null)
            {
                sparkleEffect.transform.position = transform.position; // Đặt vị trí của hiệu ứng tại vị trí của player
                sparkleEffect.SetActive(true);
                AudioSource sparkleEffectAudio = sparkleEffect.GetComponent<AudioSource>();
                if (sparkleEffectAudio != null)
                {
                    sparkleEffectAudio.Play(); 
                }
            }

            Invoke("ShowGameNextCanvas", 1f);
            isVictory = true;

        }



    }
    void ShowGameNextCanvas()
    {
        
       

        if (gameNextCanvas != null)
        {
            gameNextCanvas.SetActive(true);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bird"))
        {
            Debug.Log("Player va vào bird!");

            originalSpeed = moveSpeed;
            moveSpeed = -3;

            Invoke("RestoreMoveSpeed", 0.3f);
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

            
            GameOver();
        }
        if (collision.gameObject.CompareTag("Toilet"))
        {
            if (gameWinCanvas != null)
            {
                gameWinCanvas.SetActive(true);
                pauseButton.SetActive(false);
                isVictory = true;

            }


        }

    }


    void StopPlayer()
    {
        moveSpeed = 0;
        rb.velocity = Vector2.zero;
        
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
