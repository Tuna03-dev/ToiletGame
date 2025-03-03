using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiraBall : MonoBehaviour
{
    public float speed = 10f; // Tốc độ bay của viên đạn
    public float lifeTime = 5f; // Thời gian tồn tại trước khi bị hủy

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-speed, 0); // Đẩy viên đạn sang trái theo trục X
        Destroy(gameObject, lifeTime); // Hủy đạn sau lifeTime giây
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Nếu va chạm với kẻ địch
        {
            Destroy(gameObject); // Hủy viên đạn
            // Gọi hàm sát thương kẻ địch nếu có
        }
    }
}
