using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;
    //[SerializeField] private List<GameObject> list;
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
        Debug.Log($"Game state changed to: {state} in EntityManager");
        if (state == GameState.Playing)
        {
            UnFreeze();
        }
        else if (state == GameState.Home)
        {
            StopMoving();
        }
        else if (state == GameState.Paused)
        {
            Freeze();
        }
        else if (state == GameState.GameOver)
        {
            StopMoving();
        }
    }

    // Update is called once per frame
    

    public void Freeze()
    {
        Entity.Instance.NoMoveNoAnimate();
    }

    public void UnFreeze()
    {
        StartCoroutine(WaitToUnFreeze());
        IEnumerator WaitToUnFreeze()
        {
            yield return new WaitForSeconds(1f);
            Entity.Instance.MoveAndAnimate();
        }

    }

    public void StopMoving()
    {
            Entity.Instance.AnimateAndNoMove();
  
    }

}
