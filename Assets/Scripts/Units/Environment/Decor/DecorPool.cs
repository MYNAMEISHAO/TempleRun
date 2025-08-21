using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DecorPool : MonoBehaviour
{
    private GroundPool groundPool; // Lấy GroundPool để lấy thông tin về ground

    private float groundLength;
    private float groundHeight;
    GameObject[] DecorPrefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        GroundPool.OnGroundSpawned += HandleGroundSpawned; // Đăng ký sự kiện khi ground được spawn


    }
    void Start()
    {

        groundPool = GameObject.Find("GroundPool").GetComponent<GroundPool>(); // Lấy GroundPool từ scene
        groundLength = groundPool.GroundLength;
        groundHeight = groundPool.GroundHeight;

        Debug.Log("Ground Length: " + groundLength);
        Debug.Log("Ground Height: " + groundHeight);
        DecorPrefabs = Resources.LoadAll<GameObject>("Prefabs/Decor");
        

    }

    void OnDestroy()
    {
        GroundPool.OnGroundSpawned -= HandleGroundSpawned;
    }

    // Update is called once per frame

    public void HandleGroundSpawned(Vector3 gPos)
    {
        List<GameObject> rd = RandomDecor(); // Lấy một prefab ngẫu nhiên từ mảng DecorPrefabs
        if (rd.Count == 0) return; // Nếu không có decor nào, thoát
        else
        {
            List<int> decorPos = RandomDecorPos(rd.Count); // Lấy vị trí ngẫu nhiên để spawn decor
            for (int i = 0; i < rd.Count; i++)
            {
                float decorHeight = rd[i].GetComponent<SpriteRenderer>().bounds.size.y;

                Vector3 spawnPos = new Vector3(gPos.x + groundLength * decorPos[i] / 20f - groundLength / 2,
                    gPos.y + groundHeight / 2 + decorHeight / 2, gPos.z);
                Spawn(spawnPos, rd[i]);
            }
            
        }
            
    }

    List<GameObject> RandomDecor()
    {
        List<GameObject> decorList = new List<GameObject>();
        foreach(GameObject decor in DecorPrefabs)
        {
            int chance = Random.Range(0, 2);
            if(chance == 1) // 50% xác suất để thêm decor vào danh sách
            {
                decorList.Add(decor);
            }
        }
        return decorList;
    }

    List<int> RandomDecorPos(int n)
    {
        List<int> decorPosList = new List<int>();
        for(int i = 0; i < n; i++)
        {
            decorPosList.Add(Random.Range(1, 20));
        }
        return decorPosList;
    }

    void Spawn(Vector3 pos, GameObject prefab)
    {
        GameObject reused = FindInactiveMatching(transform, prefab);
        GameObject obj;
        if (reused != null)
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

}
