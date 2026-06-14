using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; }

    private EconomyManager economy;
    private ShipStats shipStats;
    private UpgradeDefinition[] catalog;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        catalog = Resources.LoadAll<UpgradeDefinition>("Upgrades");
        if (catalog == null || catalog.Length == 0)
            catalog = CreateDefaultCatalog();
    }

    void Start()
    {
        economy = EconomyManager.Instance;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            shipStats = player.GetComponent<ShipStats>();
    }

    public bool CanAfford(UpgradeDefinition upgrade)
    {
        if (upgrade == null || economy == null) return false;
        return economy.Credits >= upgrade.creditCost;
    }

    public bool IsAlreadyOwned(UpgradeDefinition upgrade)
    {
        if (upgrade == null || shipStats == null) return false;
        return shipStats.GetUnlockedUpgradesList().Contains(upgrade.upgradeId);
    }

    public bool TryPurchase(UpgradeDefinition upgrade)
    {
        if (upgrade == null || shipStats == null || economy == null) return false;
        if (IsAlreadyOwned(upgrade)) return false;

        int stage = ChunkManager.Instance != null
            ? ChunkManager.Instance.CurrentPlayerSector.x + ChunkManager.Instance.CurrentPlayerSector.y
            : 0;
        if (stage < upgrade.requiredSectorStage) return false;
        if (!economy.SpendCredits(upgrade.creditCost)) return false;

        ApplyEffect(upgrade);
        shipStats.UnlockUpgrade(upgrade.upgradeId);
        GameEvents.TriggerUpgradePurchased(upgrade.upgradeId);
        return true;
    }

    public void ApplyEffect(UpgradeDefinition upgrade)
    {
        if (upgrade == null || shipStats == null) return;

        switch (upgrade.effectType)
        {
            case UpgradeEffectType.EngineThrust:
                shipStats.MaxMainThrust *= 1f + upgrade.effectValue;
                PlayerData.Instance.speed += upgrade.effectValue;
                break;
            case UpgradeEffectType.CargoCapacity:
                shipStats.UpdateMaxCargo(1f + upgrade.effectValue);
                PlayerData.Instance.cargoHold = shipStats.GetMaxCargo();
                break;
            case UpgradeEffectType.MaxHP:
                shipStats.SetMaxHP(shipStats.GetMaxHP() * (1f + upgrade.effectValue));
                PlayerData.Instance.durability = shipStats.GetMaxHP();
                break;
            case UpgradeEffectType.Shield:
                shipStats.SetMaxShield(shipStats.GetMaxShield() + upgrade.effectValue);
                shipStats.RefillShield();
                PlayerData.Instance.shield = shipStats.GetMaxShield();
                break;
            case UpgradeEffectType.MilitaryScanner:
                PlayerData.Instance.militaryScanner += upgrade.effectValue;
                break;
            case UpgradeEffectType.LaserMaxTemp:
                PlayerData.Instance.laserTemperature += upgrade.effectValue;
                break;
            case UpgradeEffectType.DrillDurability:
                PlayerData.Instance.drillDurability += upgrade.effectValue;
                break;
            case UpgradeEffectType.AsteroidReport:
                PlayerData.Instance.asteroidReport = true;
                break;
            case UpgradeEffectType.SectorScanInfo:
                PlayerData.Instance.sectorInformation = true;
                break;
            case UpgradeEffectType.FastTravel:
                PlayerData.Instance.fastTravel = true;
                break;
            case UpgradeEffectType.RepairDrones:
                PlayerData.Instance.repairDrones = true;
                break;
            case UpgradeEffectType.RepairKits:
                PlayerData.Instance.repairKits = true;
                break;
        }
    }

    private static UpgradeDefinition[] CreateDefaultCatalog()
    {
        return new[]
        {
            MakeUpgrade("engine_thrust", "Silnik Mk.I", "Większy ciąg", 500f, 0, UpgradeEffectType.EngineThrust, 0.15f),
            MakeUpgrade("cargo_capacity", "Ładownia Mk.I", "Więcej miejsca", 400f, 0, UpgradeEffectType.CargoCapacity, 0.25f),
            MakeUpgrade("max_hp", "Pancerz Mk.I", "Więcej HP", 600f, 1, UpgradeEffectType.MaxHP, 0.2f),
            MakeUpgrade("shield", "Tarcza Mk.I", "Osłona energetyczna", 800f, 1, UpgradeEffectType.Shield, 50f),
            MakeUpgrade("military_scanner", "Skaner Wojskowy", "Wykrywa wrogów", 700f, 2, UpgradeEffectType.MilitaryScanner, 1f),
            MakeUpgrade("laser_max_temp", "Chłodzenie Lasera", "Wyższa temp. lasera", 450f, 1, UpgradeEffectType.LaserMaxTemp, 200f),
            MakeUpgrade("drill_durability", "Wiertło Hardened", "Trwalsze wiertło", 350f, 0, UpgradeEffectType.DrillDurability, 0.3f),
            MakeUpgrade("asteroid_report", "Analizator Próbek", "Raport składu", 300f, 0, UpgradeEffectType.AsteroidReport, 1f),
            MakeUpgrade("sector_scan", "Skan Sektorowy", "Info o sektorze", 550f, 2, UpgradeEffectType.SectorScanInfo, 1f),
            MakeUpgrade("fast_travel", "Szybki Transfer", "Skok do bazy", 1200f, 3, UpgradeEffectType.FastTravel, 1f),
            MakeUpgrade("repair_drones", "Drony Naprawcze", "5 HP/s poza walką", 900f, 2, UpgradeEffectType.RepairDrones, 5f),
            MakeUpgrade("repair_kits", "Zestawy Naprawcze", "Consumable +20% HP", 250f, 1, UpgradeEffectType.RepairKits, 0.2f),
        };
    }

    private static UpgradeDefinition MakeUpgrade(string id, string name, string desc, float cost, int stage, UpgradeEffectType type, float value)
    {
        var u = ScriptableObject.CreateInstance<UpgradeDefinition>();
        u.upgradeId = id;
        u.displayName = name;
        u.description = desc;
        u.creditCost = cost;
        u.requiredSectorStage = stage;
        u.effectType = type;
        u.effectValue = value;
        return u;
    }
}
