using UnityEngine;
[System.Serializable]
public class SpeedConfig
{
    [Header("Speed Settings")]
    public float baseSpeed = 5f;
    public float maxSpeed = 20f;
    public AnimationCurve speedOverDistance; // Giá trị từ 0 (0%) đến 1 (100%) quãng đường
    public float maxDistance = 1000f;
}
