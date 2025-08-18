using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstaclePool : MonoBehaviour
{
    private GroundPool groundPool; // Lấy GroundPool để lấy thông tin về ground

    Dictionary<int, List<float>> ObstaclePos;
    private float groundLength;
    private float groundHeight; 
    GameObject[] ObstaclePrefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        GroundPool.OnGroundSpawned += HandleGroundSpawned; // Đăng ký sự kiện khi ground được spawn
        

    }
    void Start()
    {
        SetUpObstaclePos();
        groundPool = GameObject.Find("GroundPool").GetComponent<GroundPool>(); // Lấy GroundPool từ scene
        groundLength = groundPool.GroundLength;
        groundHeight = groundPool.GroundHeight;
        ObstaclePrefabs = Resources.LoadAll<GameObject>("Prefabs/Obstacle");

    }

    void OnDestroy()
    {
        GroundPool.OnGroundSpawned -= HandleGroundSpawned;
    }

    // Update is called once per frame

    public void HandleGroundSpawned(Vector3 gPos)
    {
        int op = Random.Range(0, ObstaclePos.Count); // Chọn ngẫu nhiên một prefab
        if (ObstaclePos[op] != null)
        {
            foreach(float pos in ObstaclePos[op])
            {
          
                GameObject rgo = RandomObstacle(); // Lấy một prefab ngẫu nhiên từ mảng ObstaclePrefabs
                float obstacleHeight = rgo.GetComponent<SpriteRenderer>().bounds.size.y;
                Debug.Log("Obstacle Height: " + obstacleHeight);
                Debug.Log("Ground Length: " + groundLength);
                Debug.Log("Ground Height: " + groundHeight);
                Debug.Log("Ground Position: " + gPos.y + groundHeight/2 + obstacleHeight/2);
                Vector3 spawnPos = new Vector3(gPos.x + groundLength * pos - groundLength/2,
                    gPos.y + groundHeight/2 + obstacleHeight/2, gPos.z);
                Spawn(spawnPos, rgo);
            }
        }
    }

    GameObject RandomObstacle()
    {
        return ObstaclePrefabs[Random.Range(0, ObstaclePrefabs.Length)];
    }

    void Spawn(Vector3 pos, GameObject prefab)
    {
        GameObject reused = FindInactiveMatching(transform,prefab);
        GameObject obj;
        if(reused != null)
        {
            reused.SetActive(true); // Kích hoạt lại object
            reused.transform.position = pos; // Đặt lại vị trí
            obj = reused;
        }
        else
        {
            obj = Instantiate(prefab, pos, Quaternion.identity, transform);
        }
    }

    public GameObject FindInactiveMatching(Transform parent, GameObject go)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            if (!child.activeInHierarchy && child.name.Contains(go.name))
            {
                return child; // trả về object đang inactive phù hợp
            }
        }
        return null;
    }

    void SetUpObstaclePos()
    {
        ObstaclePos = new Dictionary<int, List<float>>();
        //Ground nào chọn cx đc
        ObstaclePos.Add(0, new List<float>() { });
        ObstaclePos.Add(1, new List<float>() { 10f / 19 });
        ObstaclePos.Add(2, new List<float>() { 6f / 19, 13f / 19 });
    }
}
