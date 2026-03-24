using System;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using Random = UnityEngine.Random;

public class DecorPool : MonoBehaviour
{
    private GroundPool groundPool;

    private float groundLength;
    private float groundHeight;
    GameObject[] DecorPrefabs;

    private void Awake()
    {
        GroundPool.OnGroundSpawned += HandleGroundSpawned;
    }

    void Start()
    {
        groundPool = GameObject.Find("GroundPool").GetComponent<GroundPool>();
        groundLength = groundPool.GroundLength;
        groundHeight = groundPool.GroundHeight;
        DecorPrefabs = Resources.LoadAll<GameObject>("Prefabs/Decor");
    }

    void OnDestroy()
    {
        GroundPool.OnGroundSpawned -= HandleGroundSpawned;
    }

    public void HandleGroundSpawned(GameObject groundObject, int obstaclePatternIndex)
    {
        Vector3 gPos = groundObject.transform.position;

        List<GameObject> rd = RandomDecor();
        if (rd.Count == 0) return;

        List<int> decorPos = RandomDecorPos(rd.Count);

        for (int i = 0; i < rd.Count; i++)
        {
            float decorHeight = rd[i].GetComponent<SpriteRenderer>().bounds.size.y;
            Vector3 spawnPos = new Vector3(gPos.x + groundLength * decorPos[i] / 20f - groundLength / 2,
                gPos.y + groundHeight / 2 + decorHeight / 2, gPos.z);
            Spawn(spawnPos, rd[i]);
        }
    }

    List<GameObject> RandomDecor()
    {
        List<GameObject> decorList = new List<GameObject>();
        foreach (GameObject decor in DecorPrefabs)
        {
            if (Random.value < 0.5f) // 50% xác suất
            {
                decorList.Add(decor);
            }
        }
        return decorList;
    }

    List<int> RandomDecorPos(int n)
    {
        List<int> possiblePositions = Enumerable.Range(1, 19).ToList();

        var shuffledPositions = possiblePositions.OrderBy(pos => Random.value);

        return shuffledPositions.Take(n).ToList();
    }

    void Spawn(Vector3 pos, GameObject prefab)
    {
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