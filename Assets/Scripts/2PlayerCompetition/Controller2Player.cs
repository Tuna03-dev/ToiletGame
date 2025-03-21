﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controller2Player : MonoBehaviour
{
    public float gravityScale = 3.0f;
    public float moveSpeed = 0.5f;
    public KeyCode UpKey;
    public KeyCode DownKey;
    private Rigidbody2D rb;
    private bool isGameOver = false;
    [HideInInspector]
    public bool isGameStarted = false;
    private bool isMoving = true;
    //public Text countdownText; // UI hiển thị đếm ngược
    public Transform groundCheck;
    public Transform frontCheck;
    public LayerMask groundLayer;
    public GameObject speedEffect;
    public GameObject fartEffect;
    public GameObject bombEffect;
    public GameObject pushBackEffect;
    public GameObject shieldEffect;
    public Controller2Player opponent; // Đối thủ
    public float pushBackDuration = 1f; // Thời gian bị đẩy lùi
    private float defaultSpeed;
    private bool hasShield = false;
    public Text winText;
    public GameObject winPopUp;
    public GameObject pauseButton;

    // 🎵 Âm thanh
    private AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip pickupSound;
    public AudioClip speedUpSound;
    public AudioClip victorySound;
    // Nhan vat
    public SpriteRenderer spriteCharacter;
    public Animator animator;
    public bool isPlayer1;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;
        defaultSpeed = moveSpeed;
        //Time.timeScale = 0; // Dừng game ngay từ đầu
        //StartCoroutine(CountdownAndStart()); // Đếm ngược rồi bắt đầu game
        // Ẩn hiệu ứng  ban đầu
        if (fartEffect != null) fartEffect.SetActive(false);
        if (speedEffect != null) speedEffect.SetActive(false);
        if (bombEffect != null) bombEffect.SetActive(false);
        if (pushBackEffect != null) pushBackEffect.SetActive(false);
        if (shieldEffect != null) shieldEffect.SetActive(false);
        if (winPopUp != null) winPopUp.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    

    void Update()
    {
        if (!isGameStarted || isGameOver) return;

        //Kiểm tra nếu nhân vật va vào tile gồ ghề phía trước
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
                ReverseGravity(-Mathf.Abs(gravityScale), 180, 180);
            }
            else if (Input.GetKeyDown(DownKey))
            {
                ReverseGravity(Mathf.Abs(gravityScale), 0, 0);
            }
        }

        // Kiểm tra game over
        CheckGameOver();
    }

    public void ReverseGravity(float newGravity, float y, float rotation)
    {
        rb.gravityScale = newGravity;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        transform.rotation = Quaternion.Euler(0, y, rotation);
        if (fartEffect != null)
        {
            fartEffect.SetActive(true);
            StartCoroutine(HideEffectCoroutine(fartEffect, 0.5f)); // Ẩn sau 0.5 giây
        }
        PlaySound(jumpSound);
    }

    
    public void SwapGravity()
    {
        if(rb.transform.rotation.z == 180)
        {
            ReverseGravity(Mathf.Abs(gravityScale), 0, 0);
        }
        ReverseGravity(-Mathf.Abs(gravityScale), 180, 180);
    }

    // Hàm đẩy lùi
    public void PushBack(float pushForce)
    {
        
        StopCoroutine("RecoverSpeed"); // Dừng hồi phục nếu đang có
        moveSpeed = -pushForce;  // Đẩy lùi về phía sau
        StartCoroutine(RecoverSpeed()); // Hồi phục sau thời gian ngắn
    }

    public void Push(float pushForce)
    {
        StopCoroutine("RecoverSpeed"); // Dừng di chuyển trong thời gian đẩy
        moveSpeed = pushForce*5f; 

        StartCoroutine(RecoverSpeed()); // Khôi phục tốc độ di chuyển
    }

    bool IsOnGround()
    {
        float rayLength = 0.2f; // Khoảng cách kiểm tra
        Vector2 originCenter = groundCheck.position;
        Vector2 originLeft = originCenter + new Vector2(-0.2f, 0);

        // Bắn 2 tia xuống
        bool center = Physics2D.Raycast(originCenter, Vector2.down, rayLength, groundLayer);
        bool left = Physics2D.Raycast(originLeft, Vector2.down, 0.2f, groundLayer);

        // Kiểm tra nếu ít nhất một trong các tia chạm đất
        return center || left;
    }

    bool IsObstacleAhead()
    {
        return Physics2D.Raycast(frontCheck.position, Vector2.right, 0.5f, groundLayer);
    }

    void CheckGameOver()
    {
        float cameraX = Camera.main.transform.position.x;
        if (transform.position.x < cameraX - 13f) GameOver();

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
        if (opponent != null)
        {
            PlaySound(victorySound);
            opponent.Win();
            pauseButton.SetActive(false);
        }
    }

    public void Win()
    {
        if (winText != null)
        {
            winPopUp.SetActive(true);
            winText.text = gameObject.name + " Wins!";
        }
        
        Time.timeScale = 0f; // Dừng game
        StartCoroutine(NewRound());
        
    }

    IEnumerator NewRound()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (gameObject.name == "Player1")
            GameManager2PlayerCom.Instance.PlayerWin(1);
        else
            GameManager2PlayerCom.Instance.PlayerWin(2);
    }

    IEnumerator HideEffectCoroutine(GameObject effect, float delay)
    {
        if (effect != null)
        {
            yield return new WaitForSeconds(delay);
            effect.SetActive(false);
        }
    }

    // Khi chạm vào item
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("reverse"))
        {
            PlaySound(pickupSound);
            ApplyEffectToOpponent(() =>
            {
                opponent?.SwapGravity(); // Đảo trọng lực đối thủ
            });
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("pushBack"))
        {
            PlaySound(pickupSound);
            ApplyEffectToOpponent(() =>
            {
                opponent?.pushBackEffect.SetActive(true);
                StartCoroutine(HideEffectCoroutine(opponent?.pushBackEffect, 0.3f));
                opponent?.PushBack(13f); // Đẩy lùi đối thủ 5 đơn vị
            });
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("bomb"))
        {
            PlaySound(pickupSound);
            ApplyEffectToOpponent(() =>
            {
                opponent?.bombEffect.SetActive(true);
                StartCoroutine(HideEffectCoroutine(opponent?.bombEffect, 0.5f));
                StartCoroutine(ZeroGravityEffect(opponent, 1f)); // Không trọng lực đối thủ trong 3 giây
            });
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Paper"))
        {
            PlaySound(speedUpSound);
            speedEffect.SetActive(true);
            StartCoroutine(HideEffectCoroutine(speedEffect, 0.3f));
            StartCoroutine(SpeedBoostEffect(1f));
        }
        else if (collision.CompareTag("shield"))
        {
            hasShield = true;
            shieldEffect.SetActive(true);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("secret"))
        {
            PlaySound(pickupSound);
            ApplyRandomEffectToOpponent();
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Trap"))
        {
            if (hasShield)
            {
                hasShield = false;
                shieldEffect?.SetActive(false);
            }
            else
            {
                GameOver();
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Xác định hướng đẩy
            Vector2 pushDirection = (transform.position.x > collision.transform.position.x) ? Vector2.right : Vector2.left;
            // Đẩy cả hai người chơi
            Push(pushDirection.x);
            opponent.Push(-pushDirection.x);
        }
    }

    //  Kiểm tra khiên trước khi áp dụng hiệu ứng
    private void ApplyEffectToOpponent(System.Action effect)
    {
        if (opponent != null && !opponent.hasShield)
        {
            effect.Invoke();
        }
        else if (opponent != null && opponent.hasShield)
        {
            opponent.hasShield = false; // Mất khiên nếu bị tấn công
            opponent.shieldEffect.SetActive(false);
        }
    }

    // Không trọng lực trong X giây
    private IEnumerator ZeroGravityEffect(Controller2Player target, float duration)
    {
        if (target == null) yield break;
        Quaternion originalRotation = target.transform.rotation;
        float originalGravity = target.rb.gravityScale; // Lưu trọng lực gốc
        target.rb.gravityScale = 0; // Mất trọng lực
        float rotationSpeed = 360f; // Tốc độ xoay (độ/giây)

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            target.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime); // Xoay tròn
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.rb.gravityScale = originalGravity; // Khôi phục trọng lực
        target.transform.rotation = originalRotation; // Đưa nhân vật về góc quay ban đầu
    }

    private IEnumerator RecoverSpeed()
    {
        float elapsedTime = 0;
        float startSpeed = moveSpeed;

        while (elapsedTime < pushBackDuration)
        {
            moveSpeed = Mathf.Lerp(startSpeed, defaultSpeed, elapsedTime / pushBackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        moveSpeed = defaultSpeed; // Đảm bảo tốc độ trở lại bình thường
    }

    // ⚡ Tăng tốc trong X giây
    private IEnumerator SpeedBoostEffect(float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= 5.5f;
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y); // Cập nhật tốc độ di chuyển
        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed; // Trả tốc độ về ban đầu
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    //  Hiệu ứng ngẫu nhiên từ Secret Item
    private void ApplyRandomEffectToOpponent()
    {
        if (opponent == null || opponent.hasShield)
        {
            opponent.hasShield = false;
            opponent.shieldEffect.SetActive(false);// Nếu có khiên thì chỉ mất khiên
            return;
        }

        int randomEffect = Random.Range(0, 4);

        switch (randomEffect)
        {
            case 0:
                opponent.SwapGravity();
                break;
            case 1:
                opponent?.pushBackEffect.SetActive(true);
                StartCoroutine(HideEffectCoroutine(opponent?.pushBackEffect, 0.3f));
                opponent.PushBack(13f);
                break;
            case 2:
                opponent?.bombEffect.SetActive(true);
                StartCoroutine(HideEffectCoroutine(opponent?.bombEffect, 0.3f));
                StartCoroutine(ZeroGravityEffect(opponent, 1.5f));
                break;
            case 3:
                hasShield = true;
                shieldEffect.SetActive(true);
                break;
        }
    }

    // 📌 Hàm phát âm thanh
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

}
