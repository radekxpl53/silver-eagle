using UnityEngine;
using UnityEngine.InputSystem; // Wymagane dla Keyboard.current

public class InventoryToggle : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject inventoryPanel; // Przeciągnij tutaj Panel ekwipunku

    private bool isOpen = false;

    void Start()
    {
        // Na starcie upewnij się, że ekwipunek jest zamknięty
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            isOpen = false;
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Sprawdzamy klawisz I
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen; // Odwracamy stan (true -> false, false -> true)
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            // Opcjonalnie: Odśwież dane przy otwarciu
            PlayerInventory inv = Object.FindFirstObjectByType<PlayerInventory>();
            if (inv != null) inv.RefreshUI();
            
            // Opcjonalnie: Odblokuj kursor myszy
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Opcjonalnie: Zablokuj kursor z powrotem (jeśli masz grę FPP/TPP)
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
    }
}