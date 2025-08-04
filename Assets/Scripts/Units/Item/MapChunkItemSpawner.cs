using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Cần thêm dòng này để xáo trộn danh sách

public class MapChunkItemSpawner : MonoBehaviour
{
    [Header("Các loại Item sẽ sinh ra")]
    public List<GameObject> itemPrefabs;

    [Header("Cài đặt chung")]
    [Range(0, 1)]
    public float spawnChance = 0.5f;
    public int maxAmount = 3;

    [Header("Cài đặt vị trí")]
    public LayerMask groundLayer;
    public float spawnHeightOffset = 0.5f;

    private SpriteRenderer chunkSpriteRenderer;

    void Awake()
    {
        chunkSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Kiểm tra các điều kiện ban đầu
        if (Random.value > spawnChance || itemPrefabs.Count == 0 || chunkSpriteRenderer == null)
        {
            return;
        }

        // 1. Tạo một danh sách tất cả các vị trí có thể sinh item
        List<Vector2> potentialSpawnPoints = new List<Vector2>();
        Bounds chunkBounds = chunkSpriteRenderer.bounds;

        // Giả sử có 10 vị trí trên mỗi platform
        int totalPoints = 10;
        for (int i = 0; i < totalPoints; i++)
        {
            float pointX = chunkBounds.min.x + (chunkBounds.size.x / (totalPoints - 1)) * i;
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(pointX, chunkBounds.max.y + 5f), Vector2.down, 20f, groundLayer);
            if (hit.collider != null)
            {
                potentialSpawnPoints.Add(hit.point + new Vector2(0, spawnHeightOffset));
            }
        }

        // 2. Xáo trộn ngẫu nhiên danh sách các vị trí này
        potentialSpawnPoints = potentialSpawnPoints.OrderBy(p => Random.value).ToList();

        // 3. Quyết định số lượng item sẽ sinh ra
        int amountToSpawn = Random.Range(1, maxAmount + 1);

        // 4. Lần lượt sinh item vào các vị trí khác nhau trong danh sách đã xáo trộn
        for (int i = 0; i < amountToSpawn && i < potentialSpawnPoints.Count; i++)
        {
            // Lấy ra vị trí thứ i (đã được xáo trộn)
            Vector2 spawnPosition = potentialSpawnPoints[i];

            // Chọn ngẫu nhiên một item
            GameObject randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

            // Sinh item và gắn nó trực tiếp vào platform này
            Instantiate(randomPrefab, spawnPosition, Quaternion.identity, transform);
        }
    }
}