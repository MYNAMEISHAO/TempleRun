using UnityEngine;

public class ObstacleController : Entity
{
    [SerializeField] GameSpeedConfig GameSpeedConfig;
    Camera Cam;
    float obstacleLength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cam = Camera.main;
        obstacleLength = transform.GetComponent<SpriteRenderer>().bounds.size.x;
        GameSpeedConfig = GameObject.Find("GameSpeed").GetComponent<GameSpeedConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            float posX = transform.position.x - Time.deltaTime * GameSpeedConfig.speedOverTime.Evaluate(GameSpeedConfig.totalTime);
            transform.position = new Vector3(posX, transform.position.y, transform.position.z);

            if (CheckOutOfCam())
            {
                transform.gameObject.SetActive(false);
            }
        }
        
    }

    bool CheckOutOfCam()
    {
        if(Cam.transform.position.x - Cam.orthographicSize * Cam.aspect > transform.position.x + obstacleLength/2)
        {
            return true;
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().isDead = true;
        }
    }
}
