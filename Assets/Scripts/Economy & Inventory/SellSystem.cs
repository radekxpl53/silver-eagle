using UnityEngine;
using UnityEngine.InputSystem;

public class SellSystem : MonoBehaviour
{
    public static float LastSaleGross { get; private set; }
    public static float LastSaleTax { get; private set; }
    public static float LastSaleNet { get; private set; }
    public static float LastSaleTaxPercent { get; private set; }

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
        if (player == null) return;

        inventory = player.GetComponent<PlayerInventory>();
        shipStats = player.GetComponent<ShipStats>();
        playerInteract = player.GetComponent<PlayerInteract>();
        if (endScreen != null) endScreen.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current == null || playerInteract == null || shipStats == null || inventory == null || economyManager == null)
            return;

        if (Keyboard.current.cKey.wasPressedThisFrame && playerInteract.canSell && shipStats.CurrentCargo > 0f)
        {
            if (!firstSell)
            {
                ShowEndScreen();
                firstSell = true;
            }

            float taxPercent = GetCurrentSectorTaxPercent();
            float grossTotal = 0f;
            float totalCredits = 0f;

            foreach (var item in inventory.myItems)
            {
                if (item?.definition == null) continue;
                float gross = item.amount * item.definition.basePrice;
                grossTotal += gross;
                totalCredits += gross * (1f - taxPercent / 100f);
            }

            float taxPaid = grossTotal - totalCredits;
            LastSaleGross = grossTotal;
            LastSaleTax = taxPaid;
            LastSaleNet = totalCredits;
            LastSaleTaxPercent = taxPercent;

            economyManager.AddCredits(totalCredits);
            inventory.myItems.Clear();
            shipStats.SetCargo(0);
            inventory.RefreshUI();

            GameEvents.TriggerResourcesSold();
            GameEvents.TriggerCreditsChanged(economyManager.Credits);
            GameEvents.TriggerDebtChanged(economyManager.Debt);
        }
    }

    private float GetCurrentSectorTaxPercent()
    {
        if (ChunkManager.Instance == null) return 0f;
        SectorDefinition def = SectorRegistry.GetDefinition(ChunkManager.Instance.CurrentPlayerSector);
        return def != null ? def.shopTaxPercent : 0f;
    }

    private void ShowEndScreen()
    {
        if (endScreen != null)
        {
            endScreen.SetActive(true);
            GameManager.Instance.ChangeState(GameState.Menu);
        }
    }

    public void CloseEndScreen()
    {
        if (endScreen != null)
        {
            endScreen.SetActive(false);
            GameManager.Instance.ChangeState(GameState.Exploration);
        }
    }
}
