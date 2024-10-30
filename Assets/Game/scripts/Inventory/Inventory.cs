using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using UnityEngine.AI;
using Yarn.Unity;

public class Inventory : Singelton<Inventory>
{[Header("Config")]
    [SerializeField] private int inventorySize;
    [SerializeField] private InventoryItem[] inventoryItems;
    [SerializeField] private GameContent _gameContent;
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private string itemName;

    [Header("References")]
    [SerializeField] private NavMeshAgent playerAgent; // Reference to player's NavMeshAgent

    [Header("Testing")]
    public InventoryItem testItem;

    private readonly Queue<string> dialogueQueue = new Queue<string>();
    private bool isDialogueActive = false;

    public int InventorySize => inventorySize;
    public InventoryItem[] InventoryItems => inventoryItems;

    public readonly string INVENTORY_KEY_DATA = "MY_INVENTORY";

    public void Start()
    {
        inventoryItems = new InventoryItem[inventorySize];
        VerifyItemsForDraw();
        LoadInventory();
        dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            AddItem(testItem, 1);
        }
    }

    public void AddItem(InventoryItem item, int quantity)
    {
        if (item == null || quantity <= 0) return;
        List<int> itemIndexes = CheckItemStockIdexes(item.ID);

        if (item.IsStackable && itemIndexes.Count > 0)
        {
            foreach (int index in itemIndexes)
            {
                int maxStack = item.MaxStack;
                if (inventoryItems[index].Quantity < maxStack)
                {
                    inventoryItems[index].Quantity += quantity;
                    if (inventoryItems[index].Quantity > maxStack)
                    {
                        int dif = inventoryItems[index].Quantity - maxStack;
                        inventoryItems[index].Quantity = maxStack;
                        AddItem(item, dif);
                        AudioManager.Instance.PlaySfx(33);
                    }

                    InventoryUi.Instance.DrawItem(inventoryItems[index], index);
                    SaveInventory();

                    EnqueueDialogue(item.Name); // Queue dialogue

                    return;
                }
            }
        }

        int quantityToAdd = quantity > item.MaxStack ? item.MaxStack : quantity;
        AddItemFreeSlot(item, quantityToAdd);
        int remainingAmount = quantity - quantityToAdd;
        if (remainingAmount > 0)
        {
            AddItem(item, remainingAmount);
        }

        SaveInventory();

        EnqueueDialogue(item.Name); // Queue dialogue
    }

    private void EnqueueDialogue(string itemName)
    {
        dialogueQueue.Enqueue(itemName);
        if (!isDialogueActive)
        {
            StartNextDialogue();
        }
    }

    private void StartNextDialogue()
    {
        if (dialogueQueue.Count > 0)
        {
            isDialogueActive = true;
            playerAgent.isStopped = true; // Stop player movement
            itemName = dialogueQueue.Dequeue();
            dialogueRunner.VariableStorage.SetValue("$itemName", itemName);
            dialogueRunner.StartDialogue("addToInventory");
        }
    }

// Called from Yarn Spinner when the dialogue ends
    public void OnDialogueComplete()
    {
        isDialogueActive = false;
        playerAgent.isStopped = false; // Resume player movement
        StartNextDialogue();
    }
    

    public void UseItem(int index)
    {
        if (inventoryItems[index] == null) return;
        if (inventoryItems[index].UseItem())
        {
            DecreaseItemStack(index);
        }
        
        SaveInventory();
    }

    public void RemoveItem(int index)
    {
        if (inventoryItems[index] == null) return;
        inventoryItems[index].RemoveItem();
        inventoryItems[index] = null;
        InventoryUi.Instance.DrawItem(null, index);
        
        SaveInventory();
    }

    public void EquipItem(int index)
    {
        if (inventoryItems[index] == null) return;
        if (inventoryItems[index].itemType != ItemType.weapon) return;
        inventoryItems[index].EquipItem();
    }

    private void AddItemFreeSlot(InventoryItem item, int quantity)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventoryItems[i] != null) continue;
            inventoryItems[i] = item.CopyItem();
            inventoryItems[i].Quantity = quantity;
            InventoryUi.Instance.DrawItem(inventoryItems[i], i);
            return;
        }
    }

    private void DecreaseItemStack(int index)
    {
        inventoryItems[index].Quantity--;
        if (inventoryItems[index].Quantity <= 0)
        {
            inventoryItems[index] = null;
            InventoryUi.Instance.DrawItem(null, index);
        }
        else
        {
            InventoryUi.Instance.DrawItem(inventoryItems[index], index);
        }
    }

    public void ConsumeItem(string itemID)
    {

        List<int> indexes = CheckItemStockIdexes(itemID);
        if (indexes.Count > 0)
        {
            DecreaseItemStack(indexes[^1]);
        }

    }

    private List<int> CheckItemStockIdexes(string itemID)
    {
        List<int> itemIndexes = new List<int>();
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null) continue;
            if (inventoryItems[i].ID == itemID)
            {
                itemIndexes.Add(i);
            }
        }

        return itemIndexes;
    }

    public int GetItemCurrentStock(string itemID)
    {
        List<int> indexes = CheckItemStockIdexes(itemID);
        int currentStock = 0;
        foreach (var index in indexes)
        {

            if (inventoryItems[index].ID==itemID)
            {

                currentStock += inventoryItems[index].Quantity;

            }
            
        }

        return currentStock;
    }

    private void VerifyItemsForDraw()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventoryItems[i] == null)
            {
                InventoryUi.Instance.DrawItem(null, i);
            }
        }
    }

    private InventoryItem ItemExistInGameContent(string ItemId)
    {
        for (int i = 0; i < _gameContent.GameItems.Length; i++)
        {
            if (_gameContent.GameItems[i].ID == ItemId)
            {
                return _gameContent.GameItems[i];
            }

            
        }
        return null;
        
    }

    private void LoadInventory()
    {

        if (SaveGame.Exists(INVENTORY_KEY_DATA))
        {
            InventoryData loadData = SaveGame.Load<InventoryData>(INVENTORY_KEY_DATA);
            for (int i = 0; i < inventorySize; i++)
            {
                if (loadData.ItemContent[i] != null)
                {
                    InventoryItem itemFromContent = ItemExistInGameContent(loadData.ItemContent[i]);

                    if (itemFromContent != null)
                    {
                        inventoryItems[i] = itemFromContent.CopyItem();
                        inventoryItems[i].Quantity = loadData.ItemQuantity[i];
                        InventoryUi.Instance.DrawItem(inventoryItems[i], i);
                    }
                }
                else
                {
                    inventoryItems[i] = null;
                }

            }
        }
        
        
    }

    private void SaveInventory()
    {
        InventoryData saveData = new InventoryData();
        saveData.ItemContent = new string[inventorySize];
        saveData.ItemQuantity = new int[inventorySize];

        for (int i = 0; i < inventorySize; i++)
        {

            if (inventoryItems[i] == null)
            {

                saveData.ItemContent[i] = null;
                saveData.ItemQuantity[i] = 0;


            }
            else
            {
                saveData.ItemContent[i] = inventoryItems[i].ID;
                saveData.ItemQuantity[i] = inventoryItems[i].Quantity;


            }

        }
        
        SaveGame.Save(INVENTORY_KEY_DATA, saveData);
    }
}
