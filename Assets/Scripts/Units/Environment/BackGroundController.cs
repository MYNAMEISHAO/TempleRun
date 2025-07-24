using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BackGroundManager : MonoBehaviour
{
    public GameSpeedConfig speed;
    private float Distance1,Distance2;
    private float totalTime;
    private Transform bg1, bg2;
    private float startPos1, startPos2;
    private float bgLength;
    private GameObject Cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpVariables();

        bg1 = transform.GetChild(0);
        bg2 = transform.GetChild(1);
        Cam = Camera.main.gameObject;

        startPos1 = bg1.transform.position.x;
        startPos2 = bg2.transform.position.x;

        bgLength = bg1.GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;

        float speedFactor = speed.speedOverTime.Evaluate(totalTime);

        if (speedFactor >= speed.maxSpeed) { 
            speedFactor = speed.maxSpeed;
        }
       // Debug.Log("Speed" + speedFactor);
        //tinh vi tri moi cua background bang cach tinh khoang cach so voi vi tri ban dau
        Distance1 += speedFactor * Time.deltaTime;
        Distance2 += speedFactor * Time.deltaTime;
        


        if(startPos1 - Distance1 < Cam.transform.position.x - bgLength)
        {
            startPos1 = startPos2 - Distance2 + bgLength;
            Distance1 = 0;
        }
        if(startPos2 - Distance2 < Cam.transform.position.x - bgLength)
        {
            startPos2 = startPos1 - Distance1 + bgLength;
            Distance2 = 0;
        }
        bg1.position = new Vector3(startPos1 - Distance1, transform.position.y, transform.position.z);
        bg2.position = new Vector3(startPos2 - Distance2, transform.position.y, transform.position.z);



    }
    void SetUpVariables()
    {
        Distance1 = 0f;
        Distance2 = 0f;
    }
}
