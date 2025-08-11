using UnityEngine;
using TMPro; // Thư viện để làm việc với TextMeshPro

public class ScoreManager : MonoBehaviour
{
    // Singleton pattern: Giúp các script khác có thể truy cập dễ dàng
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText; 
    private int score = 0;

    void Awake()
    {
        // Thiết lập Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Cập nhật UI lúc bắt đầu game
        scoreText.text = score.ToString();
    }

    // Hàm để các script khác gọi vào để cộng điểm
    public void AddPoints(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }
}