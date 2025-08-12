using UnityEngine;

public class Entity : MonoBehaviour
{
    public static Entity Instance;
    public static bool isGameStart = true;

    private void Awake()
    {
        Instance = this;
    }
    public void StopMoving()
    {
        isGameStart = false;
    }
    public void StartMoving()
    {
        isGameStart = true;
    }
}
