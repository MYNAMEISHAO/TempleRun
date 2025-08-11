using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;
    public List<GameObject> list;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    public void TurnOff()
    {
        foreach (GameObject go in list)
        {
            if (go != null)
            {
                go.SetActive(false);
            }
        }
    }

    public void TurnOn()
    {
        foreach (GameObject go in list)
        {
            if (go != null)
            {
                go.SetActive(true);
            }
        }
    }

    public void Freeze()
    {
        Time.timeScale = 0f;
    }

    public void UnFreeze()
    {
        Time.timeScale = 1f;
    }

    public void StopMoving()
    {
        foreach (GameObject go in list)
        {
            if (go != null)
            {
                Rigidbody rb = go.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                else
                {
                    go.SetActive(false);
                }
            }
        }
        
    }
}
