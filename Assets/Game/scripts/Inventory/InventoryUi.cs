using System;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class InventoryUi : Singelton<InventoryUi>
{
      [Header("Config")] 
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventorySlot slotPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private NavMeshAgent _agent;

    [Header("Description Panel")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameTMP;
    [SerializeField] private TextMeshProUGUI itemDescriptionTMP;
    

    public InventorySlot CurrentSlot { get; set; }
    
    private List<InventorySlot> slotList = new List<InventorySlot>();
    
    private void Start()
    {
       
        InitInventory();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenCloseInventory();
        }
    }

    private void InitInventory()
    {
        Debug.Log($"Inventory Size: {Inventory.Instance.InventorySize}");
        Debug.Log($"Inventory Items Count: {Inventory.Instance.InventoryItems.Length}");
    
        
        for (int i = 0; i < Inventory.Instance.InventorySize; i++)
        {
            InventorySlot slot = Instantiate(slotPrefab, container);
            slot.Index = i;
            slotList.Add(slot);
        }

        // Log to confirm the slot list was populated correctly
        Debug.Log($"Slot List Count after Init: {slotList.Count}");
    }

    public void UseItem()
    {
        if (CurrentSlot == null) return;
        Inventory.Instance.UseItem(CurrentSlot.Index);
    }

    public void RemoveItem()
    {
        if (CurrentSlot == null) return;
        Inventory.Instance.RemoveItem(CurrentSlot.Index);
    }

    public void EquipItem()
    {
        if (CurrentSlot == null) return;
        Inventory.Instance.EquipItem(CurrentSlot.Index);
    }

    public void DrawItem(InventoryItem item, int index)
    {
        if (index < 0 || index >= slotList.Count) return; // Check if index is in bounds
        InventorySlot slot = slotList[index];
        if (item == null)
        {
            slot.ShowSlotInformation(false);
            return;
        }
    
        slot.ShowSlotInformation(true);
        slot.UpdateSlot(item);
    }

    public void ShowItemDescription(int index)
    {
        if (Inventory.Instance.InventoryItems[index] == null) return;
        if (index < 0 || index >= Inventory.Instance.InventoryItems.Length || Inventory.Instance.InventoryItems[index] == null) 
        {
            descriptionPanel.SetActive(false); // Hide panel if invalid index or item
            return;
        }

        InventoryItem selectedItem = Inventory.Instance.InventoryItems[index];
        descriptionPanel.SetActive(true);
        itemIcon.sprite = selectedItem.Icon;
        itemNameTMP.text = selectedItem.Name;
        itemDescriptionTMP.text = selectedItem.Description;

    }
    
    public void OpenCloseInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        // Check if inventory is open or closed
        if (inventoryPanel.activeSelf)
        {
            // Inventory is open, disable player movement
            _agent.isStopped = true;
        }
        else
        {
            // Inventory is closed, re-enable player movement
            _agent.isStopped = false;
            descriptionPanel.SetActive(false);
            CurrentSlot = null;
        }
    }

    private void SlotSelectedCallback(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotList.Count) return; // Check bounds
        CurrentSlot = slotList[slotIndex];
        ShowItemDescription(slotIndex);
    }
    
    private void OnEnable()
    {
        InventorySlot.OnSlotSelectedEvent += SlotSelectedCallback;
    }

    private void OnDisable()
    {
        InventorySlot.OnSlotSelectedEvent -= SlotSelectedCallback;
    }
}
