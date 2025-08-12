using UnityEngine;
public class MainMenu : MonoBehaviour
{
    void OnEnable() { GameManager.OnGameStateChanged += HandleGameStateChanged; }
    void OnDisable() { GameManager.OnGameStateChanged -= HandleGameStateChanged; }

    // Hàm này tự động ẩn/hiện panel dựa trên trạng thái game
    void HandleGameStateChanged(GameState state)
    {
        gameObject.SetActive(state == GameState.Home);
    }

    // Hàm này cho nút PLAY gọi
    public void OnStartButtonClicked()
    {
        GameManager.instance.UpdateGameState(GameState.Playing);
    }
}