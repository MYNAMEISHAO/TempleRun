using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject playButton;
    public GameObject volumeSettingsPanel; // Panel chứa slider và overlay
    public GameObject closeSettingsOverlay; // Nút vô hình để đóng panel
    public GameObject exitButton; // Nút thoát game

    void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    // Tự động ẩn/hiện panel này dựa trên trạng thái game
    void HandleGameStateChanged(GameState state)
    {
        gameObject.SetActive(state == GameState.Home);
    }

    // Hàm này được gọi bởi nút PLAY
    public void OnStartButtonClicked()
    {
        SoundManager.instance.PlayGameMusic();
        GameManager.instance.UpdateGameState(GameState.Playing);
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    // Hàm này được gọi bởi nút SoundSettingsButton
    public void ToggleSettingsPanel()
    {
        if (volumeSettingsPanel != null && volumeSettingsPanel.activeSelf)
        {
            CloseSettings();
        }
        else
        {
            OpenSettings();
        }
    }

    // Hàm này được gọi bởi "màn che" CloseSettingsOverlay
    public void CloseSettings()
    {
        if(exitButton != null) exitButton.SetActive(true);
        if (playButton != null) playButton.SetActive(true);
        if (volumeSettingsPanel != null) volumeSettingsPanel.SetActive(false);
    }

    private void OpenSettings()
    {
        if (exitButton != null) exitButton.SetActive(false);
        if (playButton != null) playButton.SetActive(false);
        if (volumeSettingsPanel != null) volumeSettingsPanel.SetActive(true);
    }
}