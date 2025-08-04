using UnityEngine;

public class MapChunkCoinSpawner : MonoBehaviour
{
    public enum SpawnPattern { Line, Square, StaggeredRows }

    [Header("Cài đặt chính")]
    [Range(0, 1)]
    public float spawnChance = 0.75f; // 75% cơ hội để sinh ra xu trên mảnh map này
    public GameObject coinPrefab;
    private Transform CoinContainer;
    public LayerMask groundLayer;

    [Header("Cài đặt sắp xếp")]
    public float coinSpacing = 0.7f;
    public float spawnHeightOffset = 0.5f;
    public int lineCount = 7;
    public int squareSide = 4;
    public int staggeredRows = 3;
    public int staggeredCols = 6;

    [Header("Cài đặt chống trùng lặp")]
    public int totalSpawnPoints = 10; // Tổng số vị trí có thể sinh ra

    private SpriteRenderer chunkSpriteRenderer;

    void Awake()
    {
        // Lấy SpriteRenderer để biết được kích thước của mảnh map
        chunkSpriteRenderer = GetComponent<SpriteRenderer>();
        if (chunkSpriteRenderer == null)
        {
            // Nếu map của bạn dùng Tilemap, bạn sẽ cần lấy TilemapRenderer
            Debug.LogWarning("Không tìm thấy SpriteRenderer trên mảnh map này.");
        }
        
    }

    void Start()
    {
        // 1. Quyết định xem có sinh xu trên mảnh này không
        if (Random.value > spawnChance)
        {
            return; // Không làm gì cả
        }

        if (chunkSpriteRenderer == null) return;

        // --- PHẦN LOGIC ĐƯỢC SỬA LẠI HOÀN CHỈNH ---
        Bounds chunkBounds = chunkSpriteRenderer.bounds;
        float randomX;

        // Tìm người chơi trong màn chơi bằng Tag
        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            // Nếu không tìm thấy player, sinh ngẫu nhiên trên cả platform
            randomX = Random.Range(chunkBounds.min.x, chunkBounds.max.x);
        }
        else
        {
            // Vị trí tối thiểu để sinh xu là vị trí của player CỘNG THÊM một khoảng an toàn
            float minSpawnXBasedOnPlayer = playerTransform.position.x + 2f; // Ví dụ: cách player 2 đơn vị

            // Vị trí bắt đầu sinh xu cuối cùng phải lớn hơn cả mép platform và vị trí tính theo player
            float finalSpawnMinX = Mathf.Max(chunkBounds.min.x, minSpawnXBasedOnPlayer);

            // Nếu vị trí bắt đầu đã vượt qua cả platform thì không có chỗ để sinh xu
            if (finalSpawnMinX >= chunkBounds.max.x)
            {
                return; // Không sinh xu trên platform này
            }

            // Sinh xu trong khoảng từ vị trí bắt đầu đến cuối platform
            randomX = Random.Range(finalSpawnMinX, chunkBounds.max.x);
        }
        // ----------------------------------------------------

        // 3. Dò tìm mặt đất tại vị trí đó
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(randomX, chunkBounds.max.y + 5f), Vector2.down, 20f, groundLayer);

        if (hit.collider != null)
        {
            // 4. Nếu tìm thấy đất, sinh ra một cụm xu tại đó
            Vector2 spawnPosition = hit.point + new Vector2(0, spawnHeightOffset);
            SpawnPattern randomPattern = (SpawnPattern)Random.Range(0, System.Enum.GetValues(typeof(SpawnPattern)).Length);
            Spawn(randomPattern, spawnPosition);
        }
    }

    // Các hàm Spawn và vẽ pattern (tương tự script cũ)
    void Spawn(SpawnPattern pattern, Vector2 position)
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
        {
            Instantiate(coinPrefab, startPos + new Vector2(i * coinSpacing, 0), Quaternion.identity, transform);
        }
    }
    void SpawnSquare(Vector2 startPos, int sideLength)
    {
        for (int y = 0; y < sideLength; y++)
        {
            for (int x = 0; x < sideLength; x++)
            {
                Instantiate(coinPrefab, startPos + new Vector2(x * coinSpacing, y * coinSpacing), Quaternion.identity, transform);
            }
        }
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
}
