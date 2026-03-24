using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DecorController : Entity
{
    public GameSpeedConfig speed;
    private float bgLength;
    private Camera Cam;
    public float parallexEffect;                 //Hệ số quyết định tốc độ của nền

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cam = Camera.main;
        speed = GameObject.Find("GameSpeed").GetComponent<GameSpeedConfig>();
        bgLength = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            float totalTime = speed.totalTime;
            float speedFactor = speed.speedOverTime.Evaluate(totalTime) * parallexEffect;

            if (speedFactor >= speed.maxSpeed * parallexEffect)
            {
                speedFactor = speed.maxSpeed * parallexEffect;
            }

            if (transform.position.x + bgLength / 2 < Cam.transform.position.x - Cam.orthographicSize * Cam.aspect)
            {
                gameObject.SetActive(false);
            }

            transform.position = new Vector3(transform.position.x - speedFactor * Time.deltaTime, transform.position.y, transform.position.z);

        }
    }
}
