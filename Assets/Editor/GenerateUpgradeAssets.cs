#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class GenerateUpgradeAssets
{
    [MenuItem("SilverEagle/Generate Upgrade & Mission Assets")]
    public static void Generate()
    {
        EnsureDir("Assets/Resources/Upgrades");
        EnsureDir("Assets/Resources/Missions");

        CreateUpgrade("engine_thrust", "Silnik Mk.I", "Większy ciąg", 500f, 0, UpgradeEffectType.EngineThrust, 0.15f);
        CreateUpgrade("cargo_capacity", "Ładownia Mk.I", "Więcej miejsca", 400f, 0, UpgradeEffectType.CargoCapacity, 0.25f);
        CreateUpgrade("max_hp", "Pancerz Mk.I", "Więcej HP", 600f, 1, UpgradeEffectType.MaxHP, 0.2f);
        CreateUpgrade("shield", "Tarcza Mk.I", "Osłona energetyczna", 800f, 1, UpgradeEffectType.Shield, 50f);
        CreateUpgrade("military_scanner", "Skaner Wojskowy", "Wykrywa wrogów", 700f, 2, UpgradeEffectType.MilitaryScanner, 1f);
        CreateUpgrade("laser_max_temp", "Chłodzenie Lasera", "Wyższa temp. lasera", 450f, 1, UpgradeEffectType.LaserMaxTemp, 200f);
        CreateUpgrade("drill_durability", "Wiertło Hardened", "Trwalsze wiertło", 350f, 0, UpgradeEffectType.DrillDurability, 0.3f);
        CreateUpgrade("asteroid_report", "Analizator Próbek", "Raport składu", 300f, 0, UpgradeEffectType.AsteroidReport, 1f);
        CreateUpgrade("sector_scan", "Skan Sektorowy", "Info o sektorze", 550f, 2, UpgradeEffectType.SectorScanInfo, 1f);
        CreateUpgrade("fast_travel", "Szybki Transfer", "Skok do bazy", 1200f, 3, UpgradeEffectType.FastTravel, 1f);
        CreateUpgrade("repair_drones", "Drony Naprawcze", "5 HP/s poza walką", 900f, 2, UpgradeEffectType.RepairDrones, 5f);
        CreateUpgrade("repair_kits", "Zestawy Naprawcze", "Consumable +20% HP", 250f, 1, UpgradeEffectType.RepairKits, 0.2f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Editor] Wygenerowano 12 upgrade assetów w Resources/Upgrades/");
    }

    private static void EnsureDir(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }

    private static void CreateUpgrade(string id, string name, string desc, float cost, int stage, UpgradeEffectType type, float value)
    {
        string path = $"Assets/Resources/Upgrades/{id}.asset";
        if (File.Exists(path)) return;

        var u = ScriptableObject.CreateInstance<UpgradeDefinition>();
        u.upgradeId = id;
        u.displayName = name;
        u.description = desc;
        u.creditCost = cost;
        u.requiredSectorStage = stage;
        u.effectType = type;
        u.effectValue = value;
        AssetDatabase.CreateAsset(u, path);
    }
}
#endif
