using UnityEngine;
[System.Serializable]
public class GameSpeedConfig
{
    [Header("Speed Settings")]
    public float baseSpeed = 5f;
    public float maxSpeed = 15f;
    public AnimationCurve speedOverTime; // Giá trị từ 0 (0%) đến 1 (100%) quãng đường
    
}
