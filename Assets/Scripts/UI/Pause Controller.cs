using UnityEngine;

public class PauseController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void BackHome()
    {

    }

    public void Resume()
    {
        GameManager.instance.UpdateGameState(GameState.Playing);
    }

    public void Sound()
    {

    }
}
