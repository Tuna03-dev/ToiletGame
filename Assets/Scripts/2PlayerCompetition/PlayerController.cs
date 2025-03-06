﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float gravityScale = 3.0f;
    public float moveSpeed = 0.5f;
    public KeyCode UpKey;
    public KeyCode DownKey;
    private Rigidbody2D rb;
    private bool isGameOver = false;
    private bool isGameStarted = false;
    private bool isMoving = true;
    public Text countdownText; // UI hiển thị đếm ngược
    public Transform groundCheck;
    public Transform frontCheck;
    public LayerMask groundLayer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;
        Time.timeScale = 0; // Dừng game ngay từ đầu
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

        if (IsOnGround()) { 
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

    void ReverseGravity(float newGravity,float y, float rotation)
    {
        rb.gravityScale = newGravity;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        transform.rotation = Quaternion.Euler(0, y, rotation);
        
    }

    bool IsOnGround()
    {
        float rayLength = 0.2f; // Khoảng cách kiểm tra
        Vector2 originCenter = groundCheck.position;
        Vector2 originRight = originCenter + new Vector2(-2f,0); // Điểm bên phải

        // Bắn 3 tia xuống
        bool center = Physics2D.Raycast(originCenter, Vector2.down, rayLength, groundLayer);
        bool left = Physics2D.Raycast(originRight, Vector2.down, 0.5f, groundLayer);

        // Kiểm tra nếu ít nhất một trong các tia chạm đất
        return center  || left;
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
    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    isMoving = false;
    //}

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
}
