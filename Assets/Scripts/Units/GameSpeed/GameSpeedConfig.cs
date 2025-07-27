using UnityEngine;
[System.Serializable]
public class GameSpeedConfig : MonoBehaviour
{
    [Header("Speed Settings")]
    public float baseSpeed = 5f;
    public float maxSpeed = 20f;
    public AnimationCurve speedOverTime; // Giá trị từ 0 (0%) đến 1 (100%) quãng đường
    public float totalTime = 0;
    private void Update()
    {
        totalTime += Time.deltaTime;
    }

}
