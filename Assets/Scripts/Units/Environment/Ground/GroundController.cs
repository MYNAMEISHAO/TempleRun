using System.Collections.Generic;
using NUnit.Framework;
using TreeEditor;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float parallexEffect;
    public Unit unit;
    public GameSpeedConfig speedConfig;
    public float groundLength = 0f;
    public float camLength;
    Camera cam;
    public Transform SpawnPoint;
    void Start()
    {
        parallexEffect = 1f;
        groundLength = GetComponent<SpriteRenderer>().bounds.size.x;              //chiều dài của 1 ground
        cam = Camera.main;                                                                  //Main camera
        camLength = cam.orthographicSize * 2f * cam.aspect;                                 //Chiều dài của cam
        speedConfig = GameObject.Find("GameSpeed").GetComponent<GameSpeedConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        float time = speedConfig.totalTime;
        float speedFactor = speedConfig.speedOverTime.Evaluate(time) * parallexEffect;
        if (speedFactor > speedConfig.maxSpeed * parallexEffect)
        {
            speedFactor = speedConfig.maxSpeed * parallexEffect;
        }

        transform.position = new Vector3(transform.position.x - speedFactor * Time.deltaTime, transform.position.y, transform.position.z);
        
    }

    
}
