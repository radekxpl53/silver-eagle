using UnityEngine;
using UnityEngine.InputSystem; // Wymagane dla Keyboard.current

public class InventoryToggle : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject inventoryPanel; 

    private bool isOpen = false;

    void Start()
    {
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            isOpen = false;
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen; 
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            
            PlayerInventory inv = Object.FindFirstObjectByType<PlayerInventory>();
            if (inv != null) inv.RefreshUI();
            
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}