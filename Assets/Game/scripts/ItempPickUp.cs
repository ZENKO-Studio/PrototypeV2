using System;
using UnityEngine;

public class ItempPickUp : MonoBehaviour
{

    [SerializeField] private InventoryItem _item;
    [SerializeField] private GameObject interactionBox;

    public int quantity;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player contact");
            interactionBox.SetActive(true);
            
                Inventory.Instance.AddItem(_item,quantity);
                Debug.Log("Item Picked Up");
                
                Destroy(gameObject);
           
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interactionBox.SetActive(false);
    }
}
