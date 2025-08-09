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
    public float spawnHeightOffset = 0.5f;
    public int totalPoints = 20;
    [Range(2, 4)] public int slotCount = 3;
    public bool isStartChunk = false;  // <- CỜ XÁC ĐỊNH MẢNH START

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

                // Đánh dấu các slot mà coin đã chiếm
                List<Vector2> coinPositions = GetCoinPositions(randomPattern, coinStartPosition);
                foreach (var pos in coinPositions)
                {
                    int slot = GetSlotIndex(pos.x, chunkSpriteRenderer.bounds);
                    usedSlots.Add(slot);
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

                foreach (var point in potentialSpawnPoints.ToList()) // copy để tránh lỗi khi remove
                {
                    int slot = GetSlotIndex(point.x, chunkSpriteRenderer.bounds);
                    if (!usedSlots.Contains(slot))
                    {
                        spawnPosition = point;
                        usedSlots.Add(slot);
                        potentialSpawnPoints.Remove(point);
                        found = true;
                        break;
                    }
                }

                if (!found) break;

                GameObject randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
                Instantiate(randomPrefab, spawnPosition, Quaternion.identity, transform);
            }
        }
    }

    void OnDisable()
    {
        // Xóa toàn bộ coin & item cũ khi chunk bị tắt
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // ==== CÁC HÀM HỖ TRỢ ====

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
        Bounds chunkBounds = chunkSpriteRenderer.bounds;
        float minScanX = chunkBounds.min.x;

        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;

        // Nếu là mảnh start → chỉ spawn bên phải player
        if (isStartChunk && playerTransform != null)
        {
            float playerRight = playerTransform.position.x + 2f;
            minScanX = Mathf.Max(chunkBounds.min.x, playerRight);
        }

        if (minScanX >= chunkBounds.max.x) return points;

        for (int i = 0; i < totalPoints; i++)
        {
            float t = (float)i / (totalPoints - 1);
            float pointX = Mathf.Lerp(minScanX, chunkBounds.max.x, t);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(pointX, chunkBounds.max.y + 5f), Vector2.down, 20f, groundLayer);
            if (hit.collider != null)
            {
                points.Add(hit.point + new Vector2(0, spawnHeightOffset));
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
        if (chunkSpriteRenderer == null) chunkSpriteRenderer = GetComponent<SpriteRenderer>();
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
