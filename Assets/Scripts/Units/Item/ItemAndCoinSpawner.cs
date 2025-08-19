using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemAndCoinSpawner : MonoBehaviour
{
    public enum SpawnPattern { Line, Square, StaggeredRows }

    [Header("Cài đặt sinh Coin")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField, Range(0, 1)] private float coinSpawnChance = 1f;
    [SerializeField] private float coinSpacing = 0.7f;
    [SerializeField] private int lineCount = 7;
    [SerializeField] private int squareSide = 4;
    [SerializeField] private int staggeredRows = 3;
    [SerializeField] private int staggeredCols = 6;

    [Header("Cài đặt sinh Item")]
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField, Range(0, 1)] private float itemSpawnChance = 0.5f;
    [SerializeField] private int maxItemAmount = 3;

    [Header("Cài đặt chung")]
    [SerializeField] private float edgeMargin = 1.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int totalPoints = 20;
    [SerializeField, Range(2, 10)] private int slotCount = 3;
    [SerializeField] private bool isStartChunk = false;

    [Header("Cài đặt chiều cao spawn")]
    [SerializeField] private float spawnHeightOffsetMin = 0.5f;
    [SerializeField] private float spawnHeightOffsetMax = 1.5f;

    [Header("Cài đặt Vùng Đệm")]
    [SerializeField] private bool useBufferSlots = true;
    [SerializeField, Range(1, 3)] private int bufferSlotCount = 1;

    private SpriteRenderer chunkSpriteRenderer;
    private bool hasSpawned = false; // Biến cờ mới

    void Awake()
    {
        chunkSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện thay đổi trạng thái game
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        hasSpawned = false; // Đặt lại cờ khi chunk được bật lại

        // Kiểm tra nếu game đã ở trạng thái Playing (để xử lý trường hợp script được bật sau khi game đã bắt đầu)
        if (GameManager.instance != null && GameManager.instance.state == GameState.Playing)
        {
            HandleGameStateChanged(GameState.Playing);
        }
    }

    void OnDisable()
    {
        // Hủy đăng ký sự kiện khi chunk bị tắt
        GameManager.OnGameStateChanged -= HandleGameStateChanged;

        // Xóa tất cả các vật phẩm đã spawn trên chunk này
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Hàm xử lý khi trạng thái game thay đổi
    private void HandleGameStateChanged(GameState state)
    {
        // Chỉ spawn nếu trạng thái là Playing và chưa spawn lần nào
        if (state == GameState.Playing && !hasSpawned)
        {
            SpawnObjects();
            hasSpawned = true; // Đặt cờ đã spawn
        }
    }

    private void SpawnObjects()
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

                List<Vector2> coinPositions = GetCoinPositions(randomPattern, coinStartPosition);
                HashSet<int> slotsOccupiedByCoins = new HashSet<int>();
                foreach (var pos in coinPositions)
                {
                    slotsOccupiedByCoins.Add(GetSlotIndex(pos.x, chunkSpriteRenderer.bounds));
                }

                foreach (int slot in slotsOccupiedByCoins)
                {
                    usedSlots.Add(slot);
                }

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
        if (chunkSpriteRenderer == null) return points;

        Bounds chunkBounds = chunkSpriteRenderer.bounds;
        float minScanX = chunkBounds.min.x;

        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;

        if (isStartChunk && playerTransform != null)
        {
            float playerRight = playerTransform.position.x + 2f;
            minScanX = Mathf.Max(chunkBounds.min.x, playerRight);
        }

        float startX = minScanX + edgeMargin;
        float endX = chunkBounds.max.x - edgeMargin;

        if (startX >= endX)
        {
            return points;
        }

        for (int i = 0; i < totalPoints; i++)
        {
            float t = (totalPoints > 1) ? (float)i / (totalPoints - 1) : 0.5f;
            float pointX = Mathf.Lerp(startX, endX, t);

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(pointX, chunkBounds.max.y + 5f), Vector2.down, 20f, groundLayer);
            if (hit.collider != null)
            {
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
        if (chunkSpriteRenderer == null) chunkSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
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