using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player; // Nhân vật
    public float scrollSpeed = 2f; // Tốc độ cuộn màn hình

    void Update()
    {
        // Camera di chuyển theo hướng X với tốc độ cố định
        transform.position += new Vector3(scrollSpeed * Time.deltaTime, 0, 0);

        // Nếu nhân vật bị tụt khỏi màn hình, game over
        if (player != null && player.position.x < transform.position.x - 10f) // -10f là khoảng cách an toàn
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Game Over! Nhân vật bị rớt khỏi màn hình.");
        player.gameObject.SetActive(false); // Ẩn nhân vật
        Invoke("RestartGame", 2f); // Restart game sau 2s
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
