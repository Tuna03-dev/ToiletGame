using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isFlipped = false;
    public float gravityScale = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Nhấn Enter
        {
            isFlipped = !isFlipped;
            rb.gravityScale = isFlipped ? -gravityScale : gravityScale; // Đảo hướng trọng lực
            transform.Rotate(0f, 180f, 180f); // Quay ngược nhân vật
        }
    }
}
