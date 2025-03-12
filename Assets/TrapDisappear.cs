using UnityEngine;

public class TrapDisappear : MonoBehaviour
{
    public Transform player;         // Nhân vật
    public float triggerDistance = 1f; // Khoảng cách để trap biến mất

    private SpriteRenderer trapSpriteRenderer; // Để kiểm soát sự hiển thị của trap

    private void Start()
    {
        // Kiểm tra xem player có được gán đúng hay không
        if (player == null)
        {
            Debug.LogError("Player object is not assigned in the TrapDisappear script.");
            return;
        }

        // Lấy SpriteRenderer của trap
        trapSpriteRenderer = GetComponent<SpriteRenderer>();

        // Hiện trap ban đầu
        trapSpriteRenderer.enabled = true;
    }

    private void Update()
    {
        // Tính khoảng cách giữa trap và nhân vật
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Debug để xem khoảng cách giữa nhân vật và trap
        Debug.Log("Khoảng cách đến trap: " + distance);

        // Nếu khoảng cách nhỏ hơn hoặc bằng triggerDistance, ẩn trap đi
        if (distance <= triggerDistance)
        {
            // Ẩn trap
            if (trapSpriteRenderer.enabled)
            {
                trapSpriteRenderer.enabled = false;
                Debug.Log("Trap biến mất!");
            }
        }
        else
        {
            // Nếu khoảng cách lớn hơn triggerDistance, hiển thị trap
            if (!trapSpriteRenderer.enabled)
            {
                trapSpriteRenderer.enabled = true;
                Debug.Log("Trap xuất hiện lại!");
            }
        }
    }
}
