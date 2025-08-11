using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Cài đặt Dữ liệu")]
    public ItemDatabase itemDatabase;

    [Header("Cài đặt UI")]
    public GameObject inventoryPanelObject;
    public GameObject closeButtonObject;
    public Transform itemSlotContainer;
    public GameObject itemSlotPrefab;
    public Animator bagAnimator;

    public Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();
    private List<GameObject> itemSlotsUI = new List<GameObject>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        inventoryPanelObject.SetActive(false);
        if (closeButtonObject != null) closeButtonObject.SetActive(false);
        InitializeInventory();
    }

    void InitializeInventory()
    {
        if (itemDatabase == null)
        {
            Debug.LogError("Chưa gán ItemDatabase cho InventoryManager!");
            return;
        }

        foreach (var itemData in itemDatabase.allItems)
        {
            GameObject slotInstance = Instantiate(itemSlotPrefab, itemSlotContainer);

            Image itemIcon = slotInstance.transform.Find("ItemIcon")?.GetComponent<Image>();
            TextMeshProUGUI quantityText = slotInstance.GetComponentInChildren<TextMeshProUGUI>();

            if (itemIcon != null) itemIcon.sprite = itemData.icon;
            if (quantityText != null) quantityText.text = "0";

            itemSlotsUI.Add(slotInstance);
        }
    }

    public void ToggleInventoryPanel()
    {
        bool isActive = inventoryPanelObject.activeSelf;
        inventoryPanelObject.SetActive(!isActive);
        if (closeButtonObject != null) closeButtonObject.SetActive(!isActive);

        if (!isActive)
        {
            RefreshInventoryUI();
        }
    }

    public void CloseInventoryPanel()
    {
        inventoryPanelObject.SetActive(false);
        if (closeButtonObject != null) closeButtonObject.SetActive(false);
    }

    public void AddItem(ItemData item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item]++;
        }
        else
        {
            inventory.Add(item, 1);
        }

        if (bagAnimator != null)
        {
            bagAnimator.SetTrigger("Collect");
        }

        if (inventoryPanelObject.activeSelf)
        {
            RefreshInventoryUI();
        }
    }

    void RefreshInventoryUI()
    {
        if (itemDatabase == null || itemSlotsUI.Count != itemDatabase.allItems.Count) return;

        for (int i = 0; i < itemDatabase.allItems.Count; i++)
        {
            ItemData currentItemData = itemDatabase.allItems[i];
            GameObject currentSlotUI = itemSlotsUI[i];

            TextMeshProUGUI quantityText = currentSlotUI.GetComponentInChildren<TextMeshProUGUI>();
            if (quantityText == null) continue;

            if (inventory.ContainsKey(currentItemData))
            {
                quantityText.text = inventory[currentItemData].ToString();
            }
            else
            {
                quantityText.text = "0";
            }
        }
    }
}