using System;
using System.Collections;
using UnityEngine;

public class Interaction : MonoBehaviour
{

    [SerializeField] private InventoryItem interactObjectType;
    [SerializeField] private GameObject interactionBox;
    [SerializeField] private int Quantity;

    public InventoryItem InteractObjectType => interactObjectType;

    private bool isPlayerInRange;

    private void OnTriggerEnter(Collider other)
    {
        
            isPlayerInRange = true;
            interactionBox.SetActive(true);
            WaitForInput();
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactionBox.SetActive(false);
            
        }
    }


    private void WaitForInput()
    {
        while (isPlayerInRange)
        {

            if (Input.GetKeyDown(KeyCode.E))
            {
                
                Inventory.Instance.AddItem(interactObjectType, Quantity);
                Destroy(gameObject);
                
            }
            
        }
    }
}
