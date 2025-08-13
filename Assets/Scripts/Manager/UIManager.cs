using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject homePanel;
    public GameObject playingPanel;
    public GameObject pausedPanel;
    public GameObject gameOverPanel;

    void Awake()
    {
        GameManager.OnGameStateChanged += HandleGameStateChange;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }
    private void HandleGameStateChange(GameState state)
    {
        if (state == GameState.Playing)
        {
            homePanel.SetActive(false);
            playingPanel.SetActive(true);
            pausedPanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }
        else if (state == GameState.Home)
        {
            homePanel.SetActive(true);
            playingPanel.SetActive(false);
            pausedPanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }
        else if (state == GameState.Paused)
        {
            homePanel.SetActive(false);
            pausedPanel.SetActive(true);
            gameOverPanel.SetActive(false);
        }
        else if (state == GameState.GameOver)
        {
            homePanel.SetActive(false);
            pausedPanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }

    }
}
