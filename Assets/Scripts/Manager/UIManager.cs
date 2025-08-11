using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
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
            //homePanel.SetActive(false);
            playingPanel.SetActive(true);
            pausedPanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }
        else if (state == GameState.Home)
        {
            //homePanel.SetActive(true);
            playingPanel.SetActive(false);
            pausedPanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }
        else if (state == GameState.Paused)
        {
            //homePanel.SetActive(false);
            playingPanel.SetActive(false);
            pausedPanel.SetActive(true);
            gameOverPanel.SetActive(false);
        }
        else if (state == GameState.GameOver)
        {
            //homePanel.SetActive(false);
            playingPanel.SetActive(false);
            pausedPanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
