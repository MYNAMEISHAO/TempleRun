using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{









    //
    void pressPause()
    {
        // This method is called when the pause button is pressed
        // Implement the logic to pause the game here
        Debug.Log("Game Paused");
        GameManager.instance.UpdateGameState(GameState.Paused);
    }

}
