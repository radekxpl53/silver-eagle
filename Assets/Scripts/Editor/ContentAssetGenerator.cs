using System.IO;
using UnityEditor;
using UnityEngine;

public static class ContentAssetGenerator
{
    [MenuItem("SilverEagle/Generate Upgrades And Missions")]
    public static void GenerateAll()
    {
        GenerateUpgrades();
        GenerateMissions();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ContentAssetGenerator] Upgrades + missions generated.");
    }

    public static void GenerateUpgrades()
    {
        string dir = "Assets/Resources/Upgrades";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var defaults = new[]
        {
            Def("engine_thrust", "Silnik Mk.I", "Większy ciąg", 500f, 0, UpgradeEffectType.EngineThrust, 0.15f),
            Def("cargo_capacity", "Ładownia Mk.I", "Więcej miejsca", 400f, 0, UpgradeEffectType.CargoCapacity, 0.25f),
            Def("max_hp", "Pancerz Mk.I", "Więcej HP", 600f, 1, UpgradeEffectType.MaxHP, 0.2f),
            Def("shield", "Tarcza Mk.I", "Osłona energetyczna", 800f, 1, UpgradeEffectType.Shield, 50f),
            Def("military_scanner", "Skaner Wojskowy", "Wykrywa wrogów", 700f, 2, UpgradeEffectType.MilitaryScanner, 1f),
            Def("laser_max_temp", "Chłodzenie Lasera", "Wyższa temp. lasera", 450f, 1, UpgradeEffectType.LaserMaxTemp, 200f),
            Def("drill_durability", "Wiertło Hardened", "Trwalsze wiertło", 350f, 0, UpgradeEffectType.DrillDurability, 0.3f),
            Def("asteroid_report", "Analizator Próbek", "Raport składu", 300f, 0, UpgradeEffectType.AsteroidReport, 1f),
            Def("sector_scan", "Skan Sektorowy", "Info o sektorze", 550f, 2, UpgradeEffectType.SectorScanInfo, 1f),
            Def("fast_travel", "Szybki Transfer", "Skok do bazy", 1200f, 3, UpgradeEffectType.FastTravel, 1f),
            Def("repair_drones", "Drony Naprawcze", "5 HP/s poza walką", 900f, 2, UpgradeEffectType.RepairDrones, 5f),
            Def("repair_kits", "Zestawy Naprawcze", "Consumable +20% HP", 250f, 1, UpgradeEffectType.RepairKits, 0.2f),
        };

        foreach (var entry in defaults)
        {
            string path = $"{dir}/{entry.id}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<UpgradeDefinition>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.upgradeId = entry.id;
            asset.displayName = entry.name;
            asset.description = entry.desc;
            asset.creditCost = entry.cost;
            asset.requiredSectorStage = entry.stage;
            asset.effectType = entry.type;
            asset.effectValue = entry.value;
            EditorUtility.SetDirty(asset);
        }
    }

    public static void GenerateMissions()
    {
        string dir = "Assets/Resources/Missions";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        ResourceDatabase db = AssetDatabase.LoadAssetAtPath<ResourceDatabase>("Assets/Scripts/GlobalResourceDB.asset");
        ResourceDefinition fallback = db != null && db.Resources.Count > 0 ? db.Resources[0] : null;

        CreateMission(dir, "mission_iron_delivery", "Dostawa żelaza", "Dostarcz surowiec do stacji Cermandii.", fallback, 50, 800f);
        CreateMission(dir, "mission_copper_scan", "Skan miedzi", "Zbierz próbki miedzi w Rubieżach.", fallback, 30, 600f);
        CreateMission(dir, "mission_rare_ore", "Rzadki rudnik", "Zdobądź rzadki surowiec dla frakcji.", fallback, 20, 1200f);
    }

    private static void CreateMission(string dir, string id, string name, string desc, ResourceDefinition target, int amount, float reward)
    {
        string path = $"{dir}/{id}.asset";
        var asset = AssetDatabase.LoadAssetAtPath<FactionMissionDefinition>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<FactionMissionDefinition>();
            AssetDatabase.CreateAsset(asset, path);
        }

        asset.missionId = id;
        asset.displayName = name;
        asset.description = desc;
        asset.targetResource = target;
        asset.requiredAmount = amount;
        asset.creditReward = reward;
        EditorUtility.SetDirty(asset);
    }

    private static (string id, string name, string desc, float cost, int stage, UpgradeEffectType type, float value) Def(
        string id, string name, string desc, float cost, int stage, UpgradeEffectType type, float value) =>
        (id, name, desc, cost, stage, type, value);
}
