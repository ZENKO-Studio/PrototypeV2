using UnityEngine;


public enum ItemType
{
    Key,
    Note,
    JournalEntry,
    weapon,
    Ingredients
}

[CreateAssetMenu(menuName = "Items/Item")]
public class InventoryItem : ScriptableObject
{

    [Header("Config")]
    public string ID;
    public string Name;
    public Sprite Icon;
    [TextArea] public string Description;

    [Header("Info")] 
    public ItemType itemType;
    public bool IsConsumable;
    public bool IsStackable;
    public int MaxStack;

    [HideInInspector] public int Quantity;

    public InventoryItem CopyItem()
    {

        InventoryItem instance = Instantiate(this);
        return instance;


    }

    public virtual bool UseItem()
    {
        return true;
    }

    public virtual void EquipItem()
    {
    }

    public virtual void RemoveItem()
    {
    }

}
