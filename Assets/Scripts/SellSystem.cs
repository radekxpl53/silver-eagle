using UnityEngine;
using UnityEngine.InputSystem;

public class SellSystem : MonoBehaviour
{
    private PlayerInventory inventory;
    private EconomyManager economyManager;
    private ShipStats shipStats;
    void Start()
    {
        economyManager = EconomyManager.Instance;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<PlayerInventory>();
        shipStats = player.GetComponent<ShipStats>();
    }

    void Update()
    {
        // wciśnij C aby sprzedać
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
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
}
