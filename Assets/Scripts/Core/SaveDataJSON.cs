using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataJSON : MonoBehaviour
{
    public static SaveDataJSON Instance { get; private set; }

    public static string SavePath => Application.persistentDataPath + "/SaveData.json";

    private EconomyManager economyManager;
    private ShipStats shipStats;
    private PlayerInventory inventory;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        economyManager = EconomyManager.Instance;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            shipStats = player.GetComponent<ShipStats>();
            inventory = player.GetComponent<PlayerInventory>();
        }
    }

    public static bool HasSaveFile() => File.Exists(SavePath);

    public void SaveData()
    {
        try
        {
            var data = BuildSaveData();
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log("[Save] Zapisano grę.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Save] Błąd zapisu: {ex.Message}");
        }
    }

    public bool LoadData()
    {
        if (!HasSaveFile())
        {
            Debug.LogWarning("[Save] Brak pliku zapisu.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<GameSaveData>(json);
            if (data == null || data.saveVersion < 1)
            {
                Debug.LogWarning("[Save] Nieprawidłowa wersja zapisu.");
                return false;
            }
            ApplySaveData(data);
            Debug.Log("[Save] Wczytano grę.");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Save] Błąd wczytywania: {ex.Message}");
            return false;
        }
    }

    private GameSaveData BuildSaveData()
    {
        ResolveRefs();

        var data = new GameSaveData { saveVersion = 1 };

        if (economyManager != null)
        {
            data.credits = economyManager.Credits;
            data.debt = economyManager.Debt;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosition = player.transform.position;
            if (shipStats != null)
            {
                data.hp = shipStats.CurrentHP;
                data.energy = shipStats.CurrentEnergy;
                data.cargo = shipStats.CurrentCargo;
                data.purchasedUpgrades = shipStats.GetUnlockedUpgradesList();
            }
        }

        if (ChunkManager.Instance != null)
        {
            Vector2Int sector = ChunkManager.Instance.CurrentPlayerSector;
            data.sectorGridX = sector.x;
            data.sectorGridY = sector.y;

            data.sectors.Clear();
            foreach (var kv in ChunkManager.Instance.allSectorData)
            {
                data.sectors.Add(new SectorDataEntry
                {
                    gridX = kv.Key.x,
                    gridY = kv.Key.y,
                    json = JsonUtility.ToJson(kv.Value)
                });
            }
        }

        return data;
    }

    private void ApplySaveData(GameSaveData data)
    {
        ResolveRefs();

        if (economyManager != null)
        {
            economyManager.SetCredits(data.credits);
            economyManager.SetDebt(data.debt);
        }

        if (ChunkManager.Instance != null && data.sectors != null)
        {
            foreach (var entry in data.sectors)
            {
                var key = new Vector2Int(entry.gridX, entry.gridY);
                var sectorData = JsonUtility.FromJson<SectorData>(entry.json);
                if (sectorData != null)
                    ChunkManager.Instance.allSectorData[key] = sectorData;
            }
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = data.playerPosition;
            if (shipStats != null)
            {
                shipStats.SetHP(data.hp);
                shipStats.SetEnergy(data.energy);
                shipStats.SetCargo(data.cargo);
                if (data.purchasedUpgrades != null)
                    shipStats.LoadUpgrades(data.purchasedUpgrades);
            }
        }

        PlayerData.Instance.SetPlayerData(
            data.hp,
            Mathf.RoundToInt(data.credits),
            data.energy,
            inventory != null ? inventory.myItems : new List<ResourceStack>(),
            data.playerPosition,
            PlayerData.Instance.speed,
            PlayerData.Instance.maneuverability,
            PlayerData.Instance.acceleration,
            data.cargo,
            PlayerData.Instance.durability,
            PlayerData.Instance.shield,
            PlayerData.Instance.militaryScanner,
            PlayerData.Instance.laserTemperature,
            PlayerData.Instance.drillDurability,
            PlayerData.Instance.asteroidReport,
            PlayerData.Instance.sectorInformation,
            PlayerData.Instance.fastTravel,
            PlayerData.Instance.repairDrones,
            PlayerData.Instance.repairKits);
    }

    private void ResolveRefs()
    {
        if (economyManager == null) economyManager = EconomyManager.Instance;
        if (shipStats == null || inventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                if (shipStats == null) shipStats = player.GetComponent<ShipStats>();
                if (inventory == null) inventory = player.GetComponent<PlayerInventory>();
            }
        }
    }
}
