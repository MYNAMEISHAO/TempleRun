using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Cài đặt Dữ liệu")]
    public ItemDatabase itemDatabase;

    [Header("Cài đặt UI Chính")]
    public GameObject inventoryPanelObject;
    public GameObject closeButtonObject;
    public Transform itemSlotContainer;
    public GameObject itemSlotPrefab;
    public Animator bagAnimator;

    [Header("Quản lý UI Khác")]
    public List<GameObject> otherUIPanels = new List<GameObject>();

    public Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();
    private List<GameObject> itemSlotsUI = new List<GameObject>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Đảm bảo ban đầu túi đồ đóng
        inventoryPanelObject.SetActive(false);
        if (closeButtonObject != null) closeButtonObject.SetActive(false);

        InitializeInventory();
    }

    void InitializeInventory()
    {
        if (itemDatabase == null) return;

        foreach (var itemData in itemDatabase.allItems)
        {
            GameObject slotInstance = Instantiate(itemSlotPrefab, itemSlotContainer);
            // Thiết lập icon và text ban đầu...
            itemSlotsUI.Add(slotInstance);
        }
    }

    // Hàm chính để bật/tắt Inventory
    public void ToggleInventoryPanel()
    {
        bool isOpening = !inventoryPanelObject.activeSelf;

        inventoryPanelObject.SetActive(isOpening);
        if (closeButtonObject != null) closeButtonObject.SetActive(isOpening);

        // Xử lý các UI khác dựa trên trạng thái của Inventory
        SetOtherUIActive(!isOpening);

        if (isOpening)
        {
            RefreshInventoryUI();
        }
    }

    // Hàm đóng túi đồ (thường gắn vào nút Close)
    public void CloseInventoryPanel()
    {
        if (closeButtonObject != null) closeButtonObject.SetActive(false);
        SetOtherUIActive(true);
        inventoryPanelObject.SetActive(false);
    }

    // Hàm bổ trợ để bật/tắt danh sách UI
    private void SetOtherUIActive(bool state)
    {
        foreach (GameObject ui in otherUIPanels)
        {
            if (ui != null)
            {
                ui.SetActive(state);
            }
        }
    }

    // Các hàm AddItem và RefreshInventoryUI giữ nguyên như cũ của bạn...
    public void AddItem(ItemData item)
    {
        if (inventory.ContainsKey(item)) inventory[item]++;
        else inventory.Add(item, 1);

        if (bagAnimator != null) bagAnimator.SetTrigger("Collect");
        if (inventoryPanelObject.activeSelf) RefreshInventoryUI();
    }

    void RefreshInventoryUI()
    {
        if (itemDatabase == null || itemSlotsUI.Count != itemDatabase.allItems.Count) return;
        for (int i = 0; i < itemDatabase.allItems.Count; i++)
        {
            ItemData currentItemData = itemDatabase.allItems[i];
            TextMeshProUGUI quantityText = itemSlotsUI[i].GetComponentInChildren<TextMeshProUGUI>();
            if (quantityText != null)
            {
                quantityText.text = inventory.ContainsKey(currentItemData) ? inventory[currentItemData].ToString() : "0";
            }
        }
    }
}