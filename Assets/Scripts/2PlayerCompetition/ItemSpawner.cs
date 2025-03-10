using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemsToSpawn;  // Danh sách Prefab Item
    public Transform spawnArea;        // Empty Object chứa PolygonCollider2D
    public int itemCount = 1;          // Số lượng item cần spawn

    private PolygonCollider2D polygonCollider;
    private Vector2 minBounds, maxBounds;

    void OnEnable()
    {
        if (spawnArea != null)
        {
            polygonCollider = spawnArea.GetComponent<PolygonCollider2D>();
            if (polygonCollider == null)
            {
                return;
            }

            SetupBounds();
            SpawnItems();
        }
    }

    void SetupBounds()
    {
        // Lấy giới hạn của PolygonCollider2D
        Bounds bounds = polygonCollider.bounds;
        minBounds = bounds.min;
        maxBounds = bounds.max;
    }

    void SpawnItems()
    {
        int spawnedCount = 0;
        int maxAttempts = itemCount * 5; // Giới hạn số lần thử
        int attempts = 0;

        while (spawnedCount < itemCount && attempts < maxAttempts)
        {
            Vector2 spawnPoint = GetValidSpawnPoint();
            if (spawnPoint != Vector2.zero)
            {
                SpawnItem(spawnPoint);
                spawnedCount++;
            }
            attempts++;
        }
    }

    Vector2 GetValidSpawnPoint()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            float x = Random.Range(minBounds.x, maxBounds.x);
            float y = Random.Range(minBounds.y, maxBounds.y);
            Vector2 point = new Vector2(x, y);

            if (polygonCollider.OverlapPoint(point))
            {
                return point;
            }
        }
        return Vector2.zero; // Không tìm thấy vị trí hợp lệ
    }

    void SpawnItem(Vector2 position)
    {
        GameObject itemPrefab = itemsToSpawn[Random.Range(0, itemsToSpawn.Length)];
        GameObject newItem = Instantiate(itemPrefab, position, Quaternion.identity);
        newItem.transform.SetParent(transform);
    }
}
