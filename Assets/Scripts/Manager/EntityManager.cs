using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;
    [SerializeField] private List<GameObject> list;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
        GameManager.OnGameStateChanged += HandleGameStateChange;

    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }

    private void HandleGameStateChange(GameState state)
    {
        if (state == GameState.Playing)
        {
            
        }
        else if (state == GameState.Home)
        {
            StopMoving(1f);
        }
        else if (state == GameState.Paused)
        {
            Freeze();
        }
        else if (state == GameState.GameOver)
        {
            StopMoving(1f);
        }
    }

    // Update is called once per frame
    

    public void Freeze()
    {
        Time.timeScale = 0f;
    }

    public void UnFreeze()
    {
        Time.timeScale = 1f;
    }

    public void StopMoving(float afterSec)
    {
        StartCoroutine(Wait());
        Entity.Instance.StopMoving();

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(afterSec);
        }
        
    }
}
