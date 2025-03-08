using System.Collections;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public Transform player;
    public float spawnDistance; // Khoảng cách so với nhân vật để spawn
    public float speed = 2f; // Vận tốc bay của chim
    public Animator animator; // Animator để điều khiển animation

    private bool isDestroyed = false;
    private Rigidbody2D rb;
    private bool isVisible = false;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        rb = GetComponent<Rigidbody2D>();
        PositionBird();
        gameObject.SetActive(false); // Ẩn chim khi chưa nằm trong camera
    }

    void Update()
    {
        if (!isDestroyed && isVisible)
        {
            rb.velocity = new Vector2(-speed, 0); // Bay về bên trái theo trục X
        }

        CheckVisibility();
    }

    void PositionBird()
    {
        float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
        spawnDistance = cameraWidth; // Đặt khoảng cách bằng độ rộng của main camera
        Vector2 spawnPosition = new Vector2(player.position.x + spawnDistance, player.position.y + Random.Range(-2f, 2f));
        transform.position = spawnPosition;
    }

    void CheckVisibility()
    {
        if (!isVisible && transform.position.x < Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect)
        {
            isVisible = true;
            gameObject.SetActive(true); // Hiện chim khi camera đi qua
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDestroyed && other.CompareTag("Player"))
        {
            isDestroyed = true;
            animator.SetBool("destroy", true);
            rb.velocity = Vector2.zero; // Dừng lại khi va chạm
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}