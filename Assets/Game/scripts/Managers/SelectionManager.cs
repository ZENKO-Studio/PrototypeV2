using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{
  public static event Action OnNoSelectionEvent;

    [Header("Config")]
    [SerializeField] private LayerMask interactableMask;  // Rename for general use (not just enemies)

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;  // Assign camera on awake
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reassign the main camera whenever a new scene is loaded
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera is missing after scene load!");
        }
    }

    private void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;  // Try reassigning the camera
            if (mainCamera == null)
            {
                Debug.LogError("Main camera is missing!");
                return;
            }
        }

        HandleSelection();
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(1))  // Left-click
        {
            RaycastHit2D hit = Physics2D.Raycast(
                mainCamera.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero, Mathf.Infinity, interactableMask);

            if (hit.collider != null)
            {
                // Check if the hit object has an InventoryItem component
                InventoryItem item = hit.collider.GetComponent<InventoryItemComp>()?.Item;
                if (item != null)
                {
                    // Add the item to the inventory
                    Inventory.Instance.AddItem(item, 1);
                    Debug.Log($"Added {item.Name} to inventory");
                }
                else
                {
                    OnNoSelectionEvent?.Invoke();  // Trigger event if no inventory item selected
                }
            }
            else
            {
                OnNoSelectionEvent?.Invoke();
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
