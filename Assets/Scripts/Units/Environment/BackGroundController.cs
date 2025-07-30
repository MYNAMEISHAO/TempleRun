using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    public GameSpeedConfig speed;
    private float bgLength;
    private Camera Cam;
    public float parallexEffect;                 //Hệ số quyết định tốc độ của nền

    public LinkedList<GameObject> listBG = new LinkedList<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cam = Camera.main;
        speed = GameObject.Find("GameSpeed").GetComponent<GameSpeedConfig>();

        for(int i = 0;i < transform.childCount; i++)
        {
            listBG.AddLast(transform.GetChild(i).gameObject);
        }
        bgLength = listBG.First.Value.GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // Update is called once per frame
    void Update()
    {
        float totalTime = speed.totalTime;

        float speedFactor = speed.speedOverTime.Evaluate(totalTime) * parallexEffect;

        if (speedFactor >= speed.maxSpeed * parallexEffect)
        {
            speedFactor = speed.maxSpeed * parallexEffect;
        }
        //Debug.Log("Speed" + speedFactor);
        //tinh vi tri moi cua background bang cach tinh khoang cach so voi vi tri ban dau

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform current = transform.GetChild(i);
            if (transform.GetChild(i).position.x + bgLength/2 < Cam.transform.position.x - Cam.orthographicSize * Cam.aspect)
            {
                listBG.RemoveFirst();

                Transform lastBgPos = LastBGPosition();
                current.position = lastBgPos.position + new Vector3(bgLength,0,0);
                listBG.AddLast(current.gameObject);

            }

            current.position = new Vector3(current.position.x - speedFactor * Time.deltaTime,current.position.y,current.position.z);
        }
      
    }

    public Transform LastBGPosition()
    {
        return listBG.Last.Value.transform;
    }

}
