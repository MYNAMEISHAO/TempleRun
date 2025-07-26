using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GroundPool : MonoBehaviour 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int MAX_UNIT = 5;

    List<Unit> units = new List<Unit>();
    List<GameObject> GroundSpawned = new List<GameObject>();
    private GameObject[] Ground;
    public Transform SpawnPoint;


    private void Start()
    {
        //Nạp các prefab Ground vào List
        Ground = Resources.LoadAll<GameObject>("Prefabs/Ground");
        foreach (GameObject go in Ground)
        {
            units.Add(go.GetComponent<GroundController>().unit);
        }
        Debug.Log("list size is" + units.Count);
        
        //Lấy danh sách các Ground đã có trong hierachy
        for(int i = 1; i < transform.childCount; i++)
        {
            GroundSpawned.Add(transform.GetChild(i).gameObject);
        }

        SpawnPoint = transform.GetChild(0).transform;

        
    }
    public int PoolSize()
    {
        return units.Count;
    }

    //Lấy ngãu nhiên 1 ground tùy theo tỉ lệ
    private Unit getRandomUnits()
    {
        int totalWeight = 0;
        foreach (Unit u in units)
        {
            totalWeight += u.percentage;
        }

        //Cộng tổng các trọng số của ground lại nếu lớn hơn cái số chọn random thì chọn ground đó
        int randomNumb = Random.Range(0, totalWeight);
        int theNumb = 0;
        foreach (Unit g in units)
        {
            theNumb += g.percentage;
            if(randomNumb <= theNumb)
            {
                return g;
                Debug.Log("Ground nay la " + g.prefab.name);
            }
        }
        
        return units[0];
    }

    //Thuật toán sinh ra Ground
    public void Spawn(Vector3 position)
    {
        Unit unit = getRandomUnits();
        GameObject prefab = unit.prefab;
        GameObject GroundNeedSpawn = FindGroundInHierachy(prefab);
        //Nếu như trong hierachy đã có sẵn và đang inactive thì active lại và đặt lại vị trí
        if (GroundNeedSpawn != null)
        {
            if (!GroundNeedSpawn.activeInHierarchy)
            {
                GroundNeedSpawn.SetActive(true);
            }
            GroundNeedSpawn.transform.position = position; 
        }
        //Nếu không thì sinh ra cái mới và thêm vào danh sách các ground có trong hierachy
        else
        {
            GameObject obj = Instantiate(prefab, position, Quaternion.identity,transform);

            //Ta cần phải gán lại pool và spawnpoint cho ground mới sinh vì spawnpoint và pool chỉ có trong hierachy mà cái mới thì chưa được gán
            GroundController gc = obj.GetComponent<GroundController>();
            gc.pool = this;
            gc.SpawnPoint = SpawnPoint;
            GroundSpawned.Add(obj);
        }
       
    }
    //Đơn giản là inactive để tái sử dụng
    public void Despawn(GameObject go)
    {
        go.SetActive(false);
    }

    public GameObject FindGroundInHierachy(GameObject prefab)
    {
        foreach (GameObject g in GroundSpawned)
        {
            if (g.name.Equals(prefab.name))
            {
                return g;
            }
        }
        return null;
    }

  

}
