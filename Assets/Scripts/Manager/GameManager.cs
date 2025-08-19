using System;
using System.Collections; 
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

    private void Start()
    {
        UpdateGameState(GameState.Home);
    }

    public void UpdateGameState(GameState newState)
    {

        if (newState == GameState.GameOver)
        {
            StartCoroutine(GameOverDelayRoutine());
            return; 
        }

        
        state = newState;

        switch (newState)
        {
            case GameState.Home:
                HandleHome();
                break;
            case GameState.Playing:
                HandlePlaying();
                break;
            case GameState.Paused:
                HandlePause();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
        Debug.Log($"Game state updated to: {newState}");
    }

    private IEnumerator GameOverDelayRoutine()
    {
        HandleGameOver();

        yield return new WaitForSeconds(2f);

        state = GameState.GameOver;
        OnGameStateChanged?.Invoke(state);
        Debug.Log($"Game state updated to: {state} (sau khi trễ 2 giây)");
    }


    private void HandleGameOver()
    {

    }

    private void HandlePause()
    {
        Time.timeScale = 0f; 
    }

    private void HandlePlaying()
    {
        Time.timeScale = 1f;
    }

    private void HandleHome()
    {
        Time.timeScale = 1f; 
    }
}

public enum GameState
{
    Home,
    Playing,
    Paused,
    GameOver
}