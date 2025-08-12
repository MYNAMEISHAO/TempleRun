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
    public float edgeMargin = 1.5f;
    public LayerMask groundLayer;
    public int totalPoints = 20;
    [Range(2, 10)] public int slotCount = 3;
    public bool isStartChunk = false;

    [Header("Cài đặt chiều cao spawn")]
    public float spawnHeightOffsetMin = 0.5f;
    public float spawnHeightOffsetMax = 1.5f;

    [Header("Cài đặt vùng đệm")]
    public bool useBufferSlots = true;
    [Range(1, 3)] public int bufferSlotCount = 1;

    private SpriteRenderer chunkSpriteRenderer;
    private bool hasSpawned = false;

    void Awake()
    {
        chunkSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        hasSpawned = false;
    }

    void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing && !hasSpawned)
        {
            StartSpawning();
            hasSpawned = true;
        }

        if (state == GameState.Home || state == GameState.GameOver)
        {
            ClearAllSpawnedObjects();
            hasSpawned = false;
        }
    }

    private void StartSpawning()
    {
        ClearAllSpawnedObjects();

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

                List<Vector2> coinPositions = GetCoinPositions(randomPattern, coinStartPosition);
                HashSet<int> slotsOccupiedByCoins = new HashSet<int>();
                foreach (var pos in coinPositions)
                    slotsOccupiedByCoins.Add(GetSlotIndex(pos.x, chunkSpriteRenderer.bounds));

                foreach (int slot in slotsOccupiedByCoins)
                    usedSlots.Add(slot);

                if (useBufferSlots)
                {
                    foreach (int slot in slotsOccupiedByCoins)
                    {
                        for (int i = 1; i <= bufferSlotCount; i++)
                        {
                            if (slot - i >= 0) usedSlots.Add(slot - i);
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

                foreach (var point in potentialSpawnPoints.ToList())
                {
                    int slot = GetSlotIndex(point.x, chunkSpriteRenderer.bounds);
                    if (!usedSlots.Contains(slot))
                    {
                        spawnPosition = point;
                        itemSlot = slot;
                        potentialSpawnPoints.Remove(point);
                        found = true;
                        break;
                    }
                }

                if (!found) break;

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

    private void ClearAllSpawnedObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    private List<Vector2> GenerateSpawnPoints()
    {
        List<Vector2> points = new List<Vector2>();
        if (chunkSpriteRenderer == null) return points;

        Bounds chunkBounds = chunkSpriteRenderer.bounds;
        float minScanX = chunkBounds.min.x;

        if (isStartChunk)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                minScanX = Mathf.Max(chunkBounds.min.x, player.transform.position.x + 2f);
        }

        float startX = minScanX + edgeMargin;
        float endX = chunkBounds.max.x - edgeMargin;
        if (startX >= endX) return points;

        for (int i = 0; i < totalPoints; i++)
        {
            float t = (totalPoints > 1) ? (float)i / (totalPoints - 1) : 0.5f;
            float pointX = Mathf.Lerp(startX, endX, t);

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(pointX, chunkBounds.max.y + 5f), Vector2.down, 20f, groundLayer);
            if (hit.collider != null)
                points.Add(hit.point + new Vector2(0, Random.Range(spawnHeightOffsetMin, spawnHeightOffsetMax)));
        }
        return points;
    }

    // --- Các hàm spawn coin patterns ---
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
            Instantiate(coinPrefab, startPos + Vector2.right * coinSpacing * i, Quaternion.identity, transform);
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
            float offset = (y % 2 == 0) ? 0 : coinSpacing / 2;
            for (int x = 0; x < coinsPerRow; x++)
                Instantiate(coinPrefab, startPos + new Vector2(x * coinSpacing + offset, y * coinSpacing), Quaternion.identity, transform);
        }
    }

    List<Vector2> GetCoinPositions(SpawnPattern pattern, Vector2 startPos)
    {
        List<Vector2> positions = new List<Vector2>();
        switch (pattern)
        {
            case SpawnPattern.Line:
                for (int i = 0; i < lineCount; i++)
                    positions.Add(startPos + Vector2.right * coinSpacing * i);
                break;
            case SpawnPattern.Square:
                for (int y = 0; y < squareSide; y++)
                    for (int x = 0; x < squareSide; x++)
                        positions.Add(startPos + new Vector2(x * coinSpacing, y * coinSpacing));
                break;
            case SpawnPattern.StaggeredRows:
                for (int y = 0; y < staggeredRows; y++)
                {
                    float offset = (y % 2 == 0) ? 0 : coinSpacing / 2;
                    for (int x = 0; x < staggeredCols; x++)
                        positions.Add(startPos + new Vector2(x * coinSpacing + offset, y * coinSpacing));
                }
                break;
        }
        return positions;
    }

    int GetSlotIndex(float xPos, Bounds bounds)
    {
        float relativeX = xPos - bounds.min.x;
        float slotWidth = bounds.size.x / slotCount;
        return Mathf.Clamp(Mathf.FloorToInt(relativeX / slotWidth), 0, slotCount - 1);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (chunkSpriteRenderer != null)
            Gizmos.DrawWireCube(chunkSpriteRenderer.bounds.center, chunkSpriteRenderer.bounds.size);
    }
#endif
}
