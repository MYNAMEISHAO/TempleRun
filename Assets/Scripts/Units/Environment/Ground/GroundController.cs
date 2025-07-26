using System.Collections.Generic;
using NUnit.Framework;
using TreeEditor;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    float totalTime = 0;
    public GameSpeedConfig speedConfig;
    public float parallexEffect;
    public Unit unit;
    public GroundPool pool;

    public float groundLength = 0f;
    public float camLength;
    Camera cam;
    public Transform SpawnPoint;
    void Start()
    {
        parallexEffect = 1f;
        groundLength = GetComponent<SpriteRenderer>().bounds.size.x;              //chiều dài của 1 ground
        cam = Camera.main;                                                                  //Main camera
        camLength = cam.orthographicSize * 2f * cam.aspect;                                //Chiều dài của cam

    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        float speedFactor = speedConfig.speedOverTime.Evaluate(totalTime) * parallexEffect;
        if (speedFactor > speedConfig.maxSpeed * parallexEffect)
        {
            speedFactor = speedConfig.maxSpeed * parallexEffect;
        }

        transform.position = new Vector3(transform.position.x - speedFactor * Time.deltaTime, transform.position.y, transform.position.z);
        //Debug.Log("Toc do cua platform la " + speedFactor);

        if (transform.position.x + groundLength <= cam.transform.position.x - camLength / 2)
        {
            if (pool != null)
            {
                pool.Spawn(SpawnPoint.position);
                pool.Despawn(this.gameObject);
            }
            else
            {
                Debug.LogWarning("GroundPool chưa được gán trong GroundController!");
            }
        }
    }

    
}
