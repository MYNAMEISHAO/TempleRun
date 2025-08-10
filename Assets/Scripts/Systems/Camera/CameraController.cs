using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameObject player;
    float distance;
    float minY, maxY;
    void Start()
    {
        player = GameObject.Find("Player");
        distance = Mathf.Abs(player.transform.position.y - transform.position.y);
        minY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float lowestCam = player.transform.position.y - distance;
        if (lowestCam < minY)
        {
            lowestCam = minY;
        }
        transform.position = new Vector3(transform.position.x, lowestCam, transform.position.z);
    }
}
