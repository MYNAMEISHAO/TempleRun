using UnityEngine;

public class PlayerConfig
{
    [Header("Player Settings")]
    public float maxHeight = 5f;
    public float gravity = 1.5f;
    public float jumpForce = 10f;
    public float coyoteTime = 0.2f;  //khoảng thời gian tối đa để người chơi bấm nhảy nếu không chạm đất
}
