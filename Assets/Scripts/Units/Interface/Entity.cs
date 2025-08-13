using UnityEngine;

public class Entity : MonoBehaviour
{
    public static Entity Instance;
    public static bool canMove = false;
    public static bool canAnimate = true;
    private void Awake()
    {
        Instance = this;
    }
    
    public void MoveAndAnimate()
    {
        canAnimate = true;
        canMove = true;
    }

    public void AnimateAndNoMove()
    {
        canAnimate = true;
        canMove = false;
    }

    public void NoMoveNoAnimate()
    {
        canAnimate = false;
        canMove = false;
    }
}
