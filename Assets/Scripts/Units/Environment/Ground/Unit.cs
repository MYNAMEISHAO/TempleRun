using UnityEngine;
[System.Serializable]

public class Unit
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject prefab;
    public int percentage;

    public Unit(GameObject prefab)
    {
        this.prefab = prefab;
    }
}
