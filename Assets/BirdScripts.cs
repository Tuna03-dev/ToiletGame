using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScripts : MonoBehaviour
{
    public float activationOffset = 2f; // Camera đến gần bao nhiêu thì kích hoạt
    public float moveSpeed = 0f; // Tốc độ di chuyển của vật cản (0 = đứng yên)
    public float lifeTime = 5f; // Thời gian tồn tại trước khi bị hủy
    public GameObject destroyEffect; // Prefab hiệu ứng phá hủy

    private bool hasActivated = false;
    private Camera mainCamera;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D collider2D;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<Collider2D>();

        // Ẩn vật cản ban đầu
        spriteRenderer.enabled = false;
        collider2D.enabled = false;
        if (rb != null) rb.velocity = Vector2.zero;

        // Nếu destroyEffect đã được gán, ẩn nó đi
        if (destroyEffect != null)
        {
            destroyEffect.SetActive(false);
        }
    }

    void Update()
    {
        // Nếu Camera đi đến gần vật cản thì kích hoạt
        if (!hasActivated && mainCamera.transform.position.x >= transform.position.x - activationOffset)
        {
            ActivateObstacle();
        }
    }

    void ActivateObstacle()
    {
        hasActivated = true;
        spriteRenderer.enabled = true; // Hiện vật cản
        collider2D.enabled = true; // Bật va chạm

        if (rb != null)
        {
            rb.velocity = new Vector2(-moveSpeed, 0); // Bắt đầu di chuyển nếu có tốc độ
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Va chạm với Player!");
            DestroyWithEffect();
        }
    }

    void DestroyWithEffect()
    {
        // Ẩn vật cản
        spriteRenderer.enabled = false;
        collider2D.enabled = false;
        if (rb != null) rb.velocity = Vector2.zero;

        // Kích hoạt hiệu ứng phá hủy rồi tắt nó sau 0.5s
        if (destroyEffect != null)
        {
            destroyEffect.SetActive(true);
            Destroy(destroyEffect, 0.5f); // Hủy hiệu ứng sau 0.5s
        }

        // Hủy vật cản sau 0.5s
        Destroy(gameObject, 0.5f);
    }
}
