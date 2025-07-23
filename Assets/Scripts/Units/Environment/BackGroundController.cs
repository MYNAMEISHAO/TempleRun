using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BackGroundManager : MonoBehaviour
{
    private float startPos, bgLength;
    private float totalDistance;
    public float speed;
    Transform backGroundPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpVariables();
        backGroundPos = transform.GetChild(0);                   //Lay background1
        startPos = backGroundPos.position.x;                     //Lay vi tri bat dau
        bgLength = backGroundPos.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaDistance = Time.deltaTime * speed;
        totalDistance += deltaDistance;
        float newPosX;
        if(totalDistance > bgLength - 1)
        {
            newPosX = startPos + bgLength + 1;
            totalDistance = 0;
        }
        else
        {
            newPosX = startPos - totalDistance;
        }
        backGroundPos.position = new Vector3(newPosX, transform.position.y, transform.position.z);

        
    }

    void SetUpVariables()
    {
        speed = 5f;
        totalDistance = 0f;
    }
}
