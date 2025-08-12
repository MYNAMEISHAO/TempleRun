using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateGameState(GameState.Playing);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch(newState)
        {
            case GameState.Home:
                // Logic for home state
                HandleHome();
                break;
            case GameState.Playing:
                // Logic for playing state
                HandlePlaying();
                break;
            case GameState.Paused:
                // Logic for paused state
                HandlePause();
                break;
            case GameState.GameOver:
                // Logic for game over state
                HandleGameOver();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
        Debug.Log($"Game state updated to: {newState}"); // Debug log for state change
    }

    private void HandleGameOver()
    {
        
    }

    private void HandlePause()
    {
        
    }

    private void HandlePlaying()
    {
        
    }

    private void HandleHome()
    {
        
    }
}

public enum GameState
{
    Home,
    Playing,
    Paused,
    GameOver
}
