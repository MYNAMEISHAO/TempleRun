using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemAndCoinSpawner : MonoBehaviour
{
    public enum SpawnPattern { Line, Square, StaggeredRows }

    [Header("Cài đặt sinh Coin")]
    public GameObject coinPrefab;
    [Range(0, 1)] public float coinSpawnChance = 1f;
    public float coinSpacing = 0.7f;
    public int lineCount = 7;
    public int squareSide = 4;
    public int staggeredRows = 3;
    public int staggeredCols = 6;

    [Header("Cài đặt sinh Item")]
    public List<GameObject> itemPrefabs;
    [Range(0, 1)] public float itemSpawnChance = 0.5f;
    public int maxItemAmount = 3;

    [Header("Cài đặt chung")]
    public LayerMask groundLayer;
    public int totalPoints = 20;
    [Range(2, 10)] public int slotCount = 3; // Tăng max range để linh hoạt hơn
    public bool isStartChunk = false;

    [Header("Cài đặt chiều cao spawn")]
    public float spawnHeightOffsetMin = 0.5f;
    public float spawnHeightOffsetMax = 1.5f;

    // MỚI: Thêm cài đặt cho vùng đệm để tùy chỉnh trong Inspector
    [Header("Cài đặt Vùng Đệm")]
    public bool useBufferSlots = true; // Bật/tắt tính năng vùng đệm
    [Range(1, 3)] public int bufferSlotCount = 1; // Số slot đệm mỗi bên (1 là đủ cho hầu hết trường hợp)

    private SpriteRenderer chunkSpriteRenderer;

    void Awake()
    {
        chunkSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (chunkSpriteRenderer == null) return;

        List<Vector2> potentialSpawnPoints = GenerateSpawnPoints();
        if (potentialSpawnPoints.Count == 0) return;

        potentialSpawnPoints = potentialSpawnPoints.OrderBy(p => Random.value).ToList();

        HashSet<int> usedSlots = new HashSet<int>();

        // Spawn Coin
        if (Random.value <= coinSpawnChance && coinPrefab != null)
        {
            if (potentialSpawnPoints.Count > 0)
            {
                Vector2 coinStartPosition = potentialSpawnPoints[0];
                potentialSpawnPoints.RemoveAt(0);

                SpawnPattern randomPattern = (SpawnPattern)Random.Range(0, System.Enum.GetValues(typeof(SpawnPattern)).Length);
                SpawnCoinPattern(randomPattern, coinStartPosition);

                // SỬA ĐỔI: Logic đánh dấu slot đã chiếm của coin
                List<Vector2> coinPositions = GetCoinPositions(randomPattern, coinStartPosition);
                HashSet<int> slotsOccupiedByCoins = new HashSet<int>();
                foreach (var pos in coinPositions)
                {
                    slotsOccupiedByCoins.Add(GetSlotIndex(pos.x, chunkSpriteRenderer.bounds));
                }

                // Thêm các slot đã chiếm vào danh sách tổng
                foreach (int slot in slotsOccupiedByCoins)
                {
                    usedSlots.Add(slot);
                }

                // MỚI: Thêm các slot lân cận (vùng đệm) nếu được bật
                if (useBufferSlots)
                {
                    foreach (int slot in slotsOccupiedByCoins)
                    {
                        for (int i = 1; i <= bufferSlotCount; i++)
                        {
                            // Thêm slot bên trái
                            if (slot - i >= 0) usedSlots.Add(slot - i);
                            // Thêm slot bên phải
                            if (slot + i < slotCount) usedSlots.Add(slot + i);
                        }
                    }
                }
            }
        }

        // Spawn Items
        if (Random.value <= itemSpawnChance && itemPrefabs.Count > 0)
        {
            int amountToSpawn = Random.Range(1, maxItemAmount + 1);

            for (int i = 0; i < amountToSpawn && potentialSpawnPoints.Count > 0; i++)
            {
                Vector2 spawnPosition = Vector2.zero;
                bool found = false;
                int itemSlot = -1;

                // Tìm một điểm spawn hợp lệ
                foreach (var point in potentialSpawnPoints.ToList())
                {
                    int slot = GetSlotIndex(point.x, chunkSpriteRenderer.bounds);
                    if (!usedSlots.Contains(slot))
                    {
                        spawnPosition = point;
                        itemSlot = slot; // Lưu lại slot của item
                        potentialSpawnPoints.Remove(point);
                        found = true;
                        break;
                    }
                }

                if (!found) break;

                // SỬA ĐỔI: Thêm slot của item và vùng đệm xung quanh nó vào usedSlots
                usedSlots.Add(itemSlot);
                if (useBufferSlots)
                {
                    for (int j = 1; j <= bufferSlotCount; j++)
                    {
                        if (itemSlot - j >= 0) usedSlots.Add(itemSlot - j);
                        if (itemSlot + j < slotCount) usedSlots.Add(itemSlot + j);
                    }
                }

                GameObject randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
                Instantiate(randomPrefab, spawnPosition, Quaternion.identity, transform);
            }
        }
    }

    void OnDisable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // ==== CÁC HÀM HỖ TRỢ (Không thay đổi) ====

    void SpawnCoinPattern(SpawnPattern pattern, Vector2 position)
    {
        switch (pattern)
        {
            case SpawnPattern.Line:
                SpawnLine(position, lineCount);
                break;
            case SpawnPattern.Square:
                SpawnSquare(position, squareSide);
                break;
            case SpawnPattern.StaggeredRows:
                SpawnStaggeredRows(position, staggeredRows, staggeredCols);
                break;
        }
    }

    void SpawnLine(Vector2 startPos, int count)
    {
        for (int i = 0; i < count; i++)
            Instantiate(coinPrefab, startPos + new Vector2(i * coinSpacing, 0), Quaternion.identity, transform);
    }

    void SpawnSquare(Vector2 startPos, int sideLength)
    {
        for (int y = 0; y < sideLength; y++)
            for (int x = 0; x < sideLength; x++)
                Instantiate(coinPrefab, startPos + new Vector2(x * coinSpacing, y * coinSpacing), Quaternion.identity, transform);
    }

    void SpawnStaggeredRows(Vector2 startPos, int numRows, int coinsPerRow)
    {
        for (int y = 0; y < numRows; y++)
        {
            float xOffset = (y % 2 == 0) ? 0 : coinSpacing / 2f;
            for (int x = 0; x < coinsPerRow; x++)
            {
                Instantiate(coinPrefab, startPos + new Vector2(x * coinSpacing + xOffset, y * coinSpacing), Quaternion.identity, transform);
            }
        }
    }

    List<Vector2> GenerateSpawnPoints()
    {
        List<Vector2> points = new List<Vector2>();
        if (chunkSpriteRenderer == null) return points; // Thêm kiểm tra null
        Bounds chunkBounds = chunkSpriteRenderer.bounds;
        float minScanX = chunkBounds.min.x;

        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;

        if (isStartChunk && playerTransform != null)
        {
            float playerRight = playerTransform.position.x + 2f;
            minScanX = Mathf.Max(chunkBounds.min.x, playerRight);
        }

        if (minScanX >= chunkBounds.max.x) return points;

        for (int i = 0; i < totalPoints; i++)
        {
            float t = (totalPoints > 1) ? (float)i / (totalPoints - 1) : 0.5f;
            float pointX = Mathf.Lerp(minScanX, chunkBounds.max.x, t);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(pointX, chunkBounds.max.y + 5f), Vector2.down, 20f, groundLayer);
            if (hit.collider != null)
            {
                // Dùng giá trị random chiều cao spawn
                points.Add(hit.point + new Vector2(0, Random.Range(spawnHeightOffsetMin, spawnHeightOffsetMax)));
            }
        }

        return points;
    }

    int GetSlotIndex(float xPos, Bounds bounds)
    {
        float normalized = Mathf.InverseLerp(bounds.min.x, bounds.max.x, xPos);
        return Mathf.Clamp(Mathf.FloorToInt(normalized * slotCount), 0, slotCount - 1);
    }

    List<Vector2> GetCoinPositions(SpawnPattern pattern, Vector2 startPos)
    {
        List<Vector2> positions = new List<Vector2>();
        switch (pattern)
        {
            case SpawnPattern.Line:
                for (int i = 0; i < lineCount; i++)
                    positions.Add(startPos + new Vector2(i * coinSpacing, 0));
                break;
            case SpawnPattern.Square:
                for (int y = 0; y < squareSide; y++)
                    for (int x = 0; x < squareSide; x++)
                        positions.Add(startPos + new Vector2(x * coinSpacing, y * coinSpacing));
                break;
            case SpawnPattern.StaggeredRows:
                for (int y = 0; y < staggeredRows; y++)
                {
                    float xOffset = (y % 2 == 0) ? 0 : coinSpacing / 2f;
                    for (int x = 0; x < staggeredCols; x++)
                        positions.Add(startPos + new Vector2(x * coinSpacing + xOffset, y * coinSpacing));
                }
                break;
        }
        return positions;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (chunkSpriteRenderer == null) chunkSpriteRenderer = GetComponentInChildren<SpriteRenderer>(); // Sửa lại GetComponent
        if (chunkSpriteRenderer == null) return;

        Bounds bounds = chunkSpriteRenderer.bounds;
        float widthPerSlot = (bounds.max.x - bounds.min.x) / slotCount;

        Gizmos.color = Color.cyan;
        for (int i = 1; i < slotCount; i++)
        {
            float x = bounds.min.x + i * widthPerSlot;
            Gizmos.DrawLine(new Vector3(x, bounds.min.y - 2f, 0), new Vector3(x, bounds.max.y + 2f, 0));
        }
    }
#endif
}
