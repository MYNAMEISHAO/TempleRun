using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    private void HandleGameStateChanged(GameState state)
    {
        gameObject.SetActive(state == GameState.GameOver);
    }

    void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }


    public void OnRestartButtonClicked()
    {
        GameManager.instance.UpdateGameState(GameState.Playing);

        // Tải lại màn chơi
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuButtonClicked()
    {
        // Chỉ cần yêu cầu GameManager quay về trạng thái Home là đủ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}