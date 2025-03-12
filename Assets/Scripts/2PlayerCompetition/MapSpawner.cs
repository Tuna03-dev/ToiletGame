using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public GameObject firstChunk; // Chunk đầu tiên đã có sẵn trên màn hình
    public List<GameObject> chunkPrefabs; // Danh sách Prefab gốc (được load trước)
    public float chunkWidth = 32f; // Chiều rộng mỗi chunk
    public int maxChunks = 3; // Số chunk tối đa trên màn hình
    public float moveSpeed = 4f; // Tốc độ di chuyển

    private Queue<GameObject> activeChunks = new Queue<GameObject>(); // Hàng đợi chunk đang hiển thị
    private List<GameObject> chunkPool = new List<GameObject>(); // Pool chứa các chunk có sẵn
    private GameObject lastChunk; // Chunk cuối cùng đã spawn
    [HideInInspector]
    public bool IsUpdateChunk = false;
    void Start()
    {
        // Tạo pool từ danh sách prefab ban đầu
        foreach (var prefab in chunkPrefabs)
        {
            GameObject chunk = Instantiate(prefab);
            chunk.SetActive(false);
            chunkPool.Add(chunk);
        }

        // Đưa chunk đầu tiên vào hàng đợi
        if (firstChunk != null)
        {
            activeChunks.Enqueue(firstChunk);
            lastChunk = firstChunk;
        }

        // Tạo các chunk còn lại từ pool
        for (int i = 0; i < maxChunks - 1; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        if (!IsUpdateChunk)
            return;

        MoveChunks();

        // Xử lý chunk bị ra khỏi màn hình
        if (activeChunks.Count > 0)
        {
            GameObject first = activeChunks.Peek();
            if (first.transform.position.x < -22)
            {
                ReuseChunk(); // Tái sử dụng chunk thay vì destroy
            }
        }
    }

    void MoveChunks()
    {
        foreach (GameObject chunk in activeChunks)
        {
            chunk.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
    }

    void ReuseChunk()
    {
        GameObject oldChunk = activeChunks.Dequeue();
        oldChunk.SetActive(false);
        chunkPool.Add(oldChunk); // Đưa chunk cũ về pool
        SpawnChunk(); // Lấy chunk mới từ pool
    }

    void SpawnChunk()
    {
        GameObject chunk;
        do
        {
            chunk = GetChunkFromPool();
        } while (lastChunk != null && chunk.name == lastChunk.name); // Đảm bảo không trùng chunk trước đó

        chunk.transform.position = new Vector3(lastChunk.transform.position.x + chunkWidth, 0, 0);
        chunk.SetActive(true);
        activeChunks.Enqueue(chunk);
        lastChunk = chunk;
    }

    GameObject GetChunkFromPool()
    {
        if (chunkPool.Count == 0) return null; // Phòng trường hợp lỗi

        int randomIndex = Random.Range(0, chunkPool.Count);
        GameObject chunk = chunkPool[randomIndex];
        chunkPool.RemoveAt(randomIndex);
        return chunk;
    }
}
