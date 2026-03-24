using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstaclePool : MonoBehaviour
{
    private GroundPool groundPool;

    public static Dictionary<int, List<float>> ObstacleLayouts;

    private float groundLength;
    private float groundHeight;
    GameObject[] ObstaclePrefabs;

    private void Awake()
    {
        SetUpObstacleLayouts();
        GroundPool.OnGroundSpawned += HandleGroundSpawned;
    }

    private void SetUpObstacleLayouts()
    {
        ObstacleLayouts = new Dictionary<int, List<float>>();
        ObstacleLayouts.Add(0, new List<float>() { });
        ObstacleLayouts.Add(1, new List<float>() { 10f / 19 });
        ObstacleLayouts.Add(2, new List<float>() { 6f / 19, 13f / 19 });
    }

    void Start()
    {
        groundPool = GameObject.Find("GroundPool").GetComponent<GroundPool>();
        groundLength = groundPool.GroundLength;
        groundHeight = groundPool.GroundHeight;
        ObstaclePrefabs = Resources.LoadAll<GameObject>("Prefabs/Obstacle");
    }

    void OnDestroy()
    {
        GroundPool.OnGroundSpawned -= HandleGroundSpawned;
    }

    public void HandleGroundSpawned(GameObject groundObject, int patternIndex)
    {
        Vector3 gPos = groundObject.transform.position;

        if (ObstacleLayouts.ContainsKey(patternIndex) && ObstacleLayouts[patternIndex] != null)
        {
            foreach (float pos in ObstacleLayouts[patternIndex])
            {
                GameObject rgo = RandomObstacle();
                float obstacleHeight = rgo.GetComponent<SpriteRenderer>().bounds.size.y;
                Vector3 spawnPos = new Vector3(gPos.x + groundLength * pos - groundLength / 2,
                    gPos.y + groundHeight / 2 + obstacleHeight / 2, gPos.z);
                Spawn(spawnPos, rgo);
            }
        }
    }

    GameObject RandomObstacle()
    {
        if (ObstaclePrefabs.Length == 0) return null;
        return ObstaclePrefabs[Random.Range(0, ObstaclePrefabs.Length)];
    }

    void Spawn(Vector3 pos, GameObject prefab)
    {
        if (prefab == null) return; 

        GameObject reused = FindInactiveMatching(transform, prefab);
        GameObject obj;
        if (reused != null)
        {
            reused.SetActive(true);
            reused.transform.position = pos;
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
                return child;
            }
        }
        return null;
    }

}