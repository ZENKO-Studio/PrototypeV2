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

    private void InitInventory()
    {
        for (int i = 0; i < Inventory.Instance.InventorySize; i++)
        {
            InventorySlot slot = Instantiate(slotPrefab, container);
            slot.Index = i;
            slotList.Add(slot);
        }
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
        descriptionPanel.SetActive(true);
        itemIcon.sprite = Inventory.Instance.InventoryItems[index].Icon;
        itemNameTMP.text = Inventory.Instance.InventoryItems[index].Name;
        itemDescriptionTMP.text = Inventory.Instance.InventoryItems[index].Description;
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
