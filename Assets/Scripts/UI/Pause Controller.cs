using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void BackHome()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Resume()
    {
        GameManager.instance.UpdateGameState(GameState.Playing);
    }

    public void Sound()
    {
        Debug.Log("Sound button clicked");
    }
}
