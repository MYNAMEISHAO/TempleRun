using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GroundPool : MonoBehaviour
{

    static int MAX_GROUND_PREFAB = 5;
    GameObject[] GroundPrefab = new GameObject[MAX_GROUND_PREFAB];

    Dictionary<int, List<int>> nextGround;

    LinkedList<GameObject> GroundQueue; //queue 2 chiều
    float GroundLength = 0;

    private void Start()
    {
        //Nạp các prefab Ground vào List
        setUpDictionaryAndDequeue();
        GroundLength = transform.GetChild(1).GetComponent<SpriteRenderer>().bounds.size.x;
        GroundPrefab = Resources.LoadAll<GameObject>("Prefabs/Ground");
        
    }

    public void Update()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            float PosX = transform.GetChild(i).position.x;
            
            if (PosX + GroundLength/2 < Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect && transform.GetChild(i).gameObject.activeInHierarchy)
            {
                Debug.Log("thu " + i + "vi tri x la " + PosX);
                int ID_Last_Ground = getIDLastGround();
                int ID_Next_Ground = getIDNextGround(ID_Last_Ground);
                GameObject nextGround = getNextGround(ID_Next_Ground);
                //Debug.Log("NextGround is " + nextGround.name);
                Despawn(transform.GetChild(i).gameObject);

                if (ID_Next_Ground == 1)
                {
                    Vector3 lastGroundPos = GroundQueue.Last.Value.transform.position;
                    Vector3 nextSpawnPos = lastGroundPos + new Vector3(GroundLength + 5, 0, 0);
                    Spawn(nextSpawnPos, nextGround);
                    //Spawn(nextSpawnPos, transform.GetChild(i).gameObject);
                    break;
                }
                else
                {
                    Vector3 lastGroundPos = GroundQueue.Last.Value.transform.position;
                    Vector3 nextSpawnPos = lastGroundPos + new Vector3(GroundLength, 0, 0);
                    Spawn(nextSpawnPos, nextGround);
                    //Spawn(nextSpawnPos, transform.GetChild(i).gameObject);
                    break;
                }
            }
        }
    }

    //Lấy ngãu nhiên 1 ground dựa trên ground cuối cùng xuất hiện
    private int getIDNextGround(int i)
    {
        if (i == 1 || i == 2)
        {
            int numb = Random.Range(2, 4);
            return numb;
        }
        return 1;
    }

    private GameObject getNextGround(int i)
    {
        if (i == 1) return GroundPrefab[2];
        else if (i == 2) return GroundPrefab[1];
        return GroundPrefab[0];
    }


    //Thuật toán sinh ra Ground
    public void Spawn(Vector3 position, GameObject go)
    {
        GameObject reused = FindInactiveMatching(transform, go);
        //Nếu như trong hierachy đã có sẵn và đang inactive thì active lại và đặt lại vị trí
        if (reused != null)
        {
            //Debug.Log("Active lai" + reused.name);
            reused.transform.position = position;
            reused.SetActive(true);
            GroundQueue.AddLast(reused);
        }

        //Nếu không thì sinh ra cái mới và thêm vào danh sách các ground có trong hierachy
        else
        {
            GameObject obj = Instantiate(go, position, Quaternion.identity, transform);
            //Debug.Log("Sinh cai moi" + obj.name);
            //Ta cần phải gán lại pool và spawnpoint cho ground mới sinh vì spawnpoint và pool chỉ có trong hierachy mà cái mới thì chưa được gán
            GroundController gc = obj.GetComponent<GroundController>();
            GroundQueue.AddLast(obj);
        }
    }

    public int getIDLastGround()
    {
        if (GroundQueue != null)
        {
            if (GroundQueue.Last.Value.name.Equals("StartPlatform")) return 1;
            else if (GroundQueue.Last.Value.name.Equals("MiddlePlatform")) return 2;
            return 3;
        }
        return 2;
    }

    //Đơn giản là inactive để tái sử dụng
    public void Despawn(GameObject go)
    {
        GroundQueue.RemoveFirst();
        go.SetActive(false);
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


    public void setUpDictionaryAndDequeue()
    {
        //Dictionary
        nextGround = new Dictionary<int, List<int>>();
        nextGround[1] = new List<int> { 2, 3 };
        nextGround[2] = new List<int> { 2, 3 };
        nextGround[3] = new List<int> { 1 };

        //Dequeue
        GroundQueue = new LinkedList<GameObject>();
        for (int i = 1; i < transform.childCount; i++)
        {
            GroundQueue.AddLast(transform.GetChild(i).gameObject);
        }

    }
}
