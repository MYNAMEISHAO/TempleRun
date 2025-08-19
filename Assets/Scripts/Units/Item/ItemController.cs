using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemData itemData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.instance?.AddItem(itemData);
            Destroy(gameObject);
        }
    }
}