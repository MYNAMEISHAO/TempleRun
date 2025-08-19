using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image soundButtonImage;
    [SerializeField] private Color soundOffColor = Color.gray;
    private Color soundButtonOriginalColor;

    private void Awake()
    {
        if (soundButtonImage != null)
        {
            soundButtonOriginalColor = soundButtonImage.color;
        }
    }

    void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        UpdateSoundButtonVisual();

        SoundManager.instance.PlayGameOverMusic();
    }

    void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        gameObject.SetActive(state == GameState.GameOver);
    }

    public void OnRestartButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Sound()
    {
        SoundManager.instance.ToggleSound();
        UpdateSoundButtonVisual();
    }

    private void UpdateSoundButtonVisual()
    {
        if (soundButtonImage != null && SoundManager.instance != null)
        {
            soundButtonImage.color = SoundManager.instance.IsMuted ? soundOffColor : soundButtonOriginalColor;
        }
    }
}