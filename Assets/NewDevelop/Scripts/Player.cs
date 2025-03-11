using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float gravityScale = 3.0f;
    public float moveSpeed = 0.5f;
    public KeyCode UpKey;
    public KeyCode DownKey;
    private Rigidbody2D rb;
    private bool isGameOver = false;

    [HideInInspector]
    public bool IsGameStarted = false;

    private bool isMoving = true;
     // UI hiển thị đếm ngược
    public Transform groundCheck;
    public Transform frontCheck;
    public LayerMask groundLayer;
    public GameObject fartEffect;
    public GameObject speedEffect;


    public SpriteRenderer spriteCharacter;
    public Animator animator;
    public float boostSpeed = 3;

    public AudioClip audioFart;

    public AudioClip audioTriggerTrapItem;

    public AudioClip audioTriggerRewardItem;

    public AudioSource audioSource;

    private float originalSpeed;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;


        if (fartEffect != null)
        {
            fartEffect.SetActive(false);
        }
        if (speedEffect != null)
        {
            speedEffect.SetActive(false);
        }
        originalSpeed = moveSpeed;
    }


    void Update()
    {
        if (!IsGameStarted || isGameOver) return;


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


        if (Input.GetKeyDown(UpKey))
        {
            ReverseGravity(-Mathf.Abs(gravityScale), 180, 180);
        }
        else if (Input.GetKeyDown(DownKey))
        {
            ReverseGravity(Mathf.Abs(gravityScale), 0, 0);
        }


        // Kiểm tra game over
        CheckGameOver();
    }

    void ReverseGravity(float newGravity, float y, float rotation)
    {
        rb.gravityScale = newGravity;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        transform.rotation = Quaternion.Euler(0, y, rotation);
        if (fartEffect != null)
        {
            fartEffect.SetActive(true);
            Invoke("HideFartEffect", 0.5f);
        }

        PlayAudio(audioFart);
    }




    void HideFartEffect()
    {
        if (fartEffect != null)
        {
            fartEffect.SetActive(false);
        }
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
        GameManager.Instance.RestartGame();
    }

    private bool isBoosting;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsGameStarted) 
            return;

        if (collision.CompareTag("Paper"))
        {
            if (!isBoosting)
            {
                Destroy(collision.gameObject);
                isBoosting = true;
                speedEffect.SetActive(true);
                moveSpeed *= boostSpeed;

                StartCoroutine(IEWaitBoost());

                
            }
        }
        if (collision.CompareTag("Trap"))
        {
            PlayAudio(audioTriggerTrapItem);

            GameManager.Instance.TriggerOstacle();
        }
        if (collision.CompareTag("Reward"))
        {
            Destroy(collision.gameObject);
            PlayAudio(audioTriggerRewardItem);

            GameManager.Instance.RewardItem();

            
        }
    }

    public void Stop()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is null in Player.Stop()");
            return;
        }
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
    }

    private IEnumerator IEWaitBoost()
    {
        yield return new WaitForSeconds(1.5f);

        moveSpeed = originalSpeed;
        isBoosting = false;
        speedEffect.SetActive(false);
    }

    public void PlayAudio(AudioClip audio)
    {
        audioSource.PlayOneShot(audio);
        audioSource.Play();
    }
}
