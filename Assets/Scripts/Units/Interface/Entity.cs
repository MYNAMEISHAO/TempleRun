using UnityEngine;

public class Entity : MonoBehaviour
{
    public static Entity Instance;
    public static bool isGameStart = true;
    public static bool isPlaying = true;
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

    public void StopAnimation()
    {
        isPlaying = false;
    }

    public void ResumeAnimation()
    {
        isPlaying = true;
    }
}
