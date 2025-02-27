using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    public GameObject chunkPrefab; // Prefab của chunk
    public int chunkCount = 3; // Số lượng chunk tối đa trên màn hình
    public float chunkWidth = 32f; // Chiều rộng của mỗi chunk
    public float moveSpeed = 5f; // Tốc độ di chuyển của chunk
    private List<GameObject> chunks = new List<GameObject>(); // Danh sách chứa các chunk
    private float nextChunkX = 0; // Vị trí X của chunk tiếp theo

    void Start()
    {
        // Khởi tạo các chunk ban đầu
        for (int i = 0; i < chunkCount; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        // Di chuyển tất cả các chunk sang trái
        for (int i = chunks.Count - 1; i >= 0; i--)
        {
            GameObject chunk = chunks[i];
            chunk.transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            // Nếu chunk ra khỏi màn hình bên trái, thì xóa nó
            if (chunk.transform.position.x < -chunkWidth * 2) // Kiểm tra với khoảng cách an toàn
            {
                Destroy(chunk);
                chunks.RemoveAt(i);
            }
        }

        // Nếu số lượng chunk ít hơn giới hạn, spawn thêm chunk mới
        if (chunks.Count < chunkCount)
        {
            SpawnChunk();
        }
    }

    void SpawnChunk()
    {
        GameObject newChunk = Instantiate(chunkPrefab, new Vector3(nextChunkX, 0, 0), Quaternion.identity);
        chunks.Add(newChunk);
        nextChunkX += chunkWidth;
    }
}
