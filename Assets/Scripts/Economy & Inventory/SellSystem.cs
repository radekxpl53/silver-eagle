using UnityEngine;
using UnityEngine.InputSystem;

public class SellSystem : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private ShipStats shipStats;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private bool firstSell = false;
    void Start()
    {
        economyManager = EconomyManager.Instance;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<PlayerInventory>();
        shipStats = player.GetComponent<ShipStats>();
        playerInteract = player.GetComponent<PlayerInteract>();
        if (endScreen != null) endScreen.SetActive(false);
    }

    void Update()
    {
        // wciśnij C aby sprzedać
        if (Keyboard.current.cKey.wasPressedThisFrame && playerInteract.canSell && shipStats.CurrentCargo > 0f)
        {
            if (!firstSell)
            {
                ShowEndScreen();
                firstSell = true;
            }

            foreach (var item in inventory.myItems)
            {
                int credits = item.amount * item.definition.basePrice;
                economyManager.AddCredits(credits);
            }

            inventory.myItems.Clear();
            shipStats.SetCargo(0);
            inventory.RefreshUI();

        }
    }

    private void ShowEndScreen()
    {
        if (endScreen != null)
        {
            endScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
    public void CloseEndScreen()
    {
        if (endScreen != null)
        {
            endScreen.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}


