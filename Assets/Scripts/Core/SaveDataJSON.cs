using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveDataJSON : MonoBehaviour
{
    private PlayerData playerData;
    [SerializeField] private ShipStats shipStats;
    [SerializeField] PlayerInventory inventory;
    private EconomyManager economyManager;

    void Start()
    {
        playerData = PlayerData.Instance;
        economyManager = EconomyManager.Instance;
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public void GoToMainMenu()
    {
        SaveData();
        SceneManager.LoadScene("MainMenu"); // Zmień na nazwę swojej sceny
    }

    public void SaveData()
    {
        playerData.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        playerData.hp = shipStats.CurrentHP;
        playerData.energy = shipStats.CurrentEnergy;
        playerData.cargoHold = shipStats.CurrentCargo;
        playerData.credits = economyManager.Credits;

        playerData.unlockedUpgrades = new List<string>(shipStats.GetUnlockedUpgradesList());
        
        playerData.inventory.Clear();

        foreach (var item in inventory.myItems)
        {
            ResourceStack saveStack = new ResourceStack
            {
                definition = item.definition,
                amount = item.amount
            };

            playerData.inventory.Add(saveStack);
        }
        
        string json = JsonUtility.ToJson(playerData, true);
        Debug.Log(json);

        string path = Application.persistentDataPath + "/SaveData.json";
        using(StreamWriter writer = new StreamWriter(path))
        {
            writer.Write(json);
        }
        Debug.Log("Data saved");
    }

    public void LoadData()
    {
        string json = string.Empty;

        string path = Application.persistentDataPath + "/SaveData.json";
        using(StreamReader reader = new StreamReader(path))
        {
            json = reader.ReadToEnd();
        }

        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        shipStats.LoadUpgrades(data.unlockedUpgrades);

        GameObject.FindGameObjectWithTag("Player").transform.position = data.position;

        playerData.SetPlayerData(data.hp, data.credits, data.energy, data.inventory, data.position, data.speed, data.maneuverability, data.acceleration, data.cargoHold, data.durability, 
        data.shield, data.militaryScanner, data.laserTemperature, data.drillDurability, data.asteroidReport, data.sectorInformation, data.fastTravel, data.repairDrones, data.repairKits, data.unlockedUpgrades);
        shipStats.SetHP(data.hp);
        shipStats.SetEnergy(data.energy);
        shipStats.SetCargo(data.cargoHold);
        economyManager.SetCredits(data.credits);

        inventory.myItems.Clear();

        foreach (var item in data.inventory)
        {
            inventory.myItems.Add(new ResourceStack
            {
                definition = item.definition,
                amount = item.amount
            });
        }

        inventory.RefreshUI();

        Debug.Log("Data loaded");
    }
}
