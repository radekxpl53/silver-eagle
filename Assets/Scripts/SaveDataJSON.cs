using System.IO;
using UnityEngine;

public class SaveDataJSON : MonoBehaviour
{
    private PlayerData playerData;
    [SerializeField] private ShipStats shipStats;
    private EconomyManager economyManager;

    void Start()
    {
        playerData = PlayerData.Instance;
        economyManager = EconomyManager.Instance;
    }

    public void SaveData()
    {
        playerData.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        playerData.hp = shipStats.CurrentHP;
        playerData.credits = economyManager.Credits;
        
        string json = JsonUtility.ToJson(playerData);
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

        GameObject.FindGameObjectWithTag("Player").transform.position = data.position;

        playerData.SetPlayerData(data.hp, data.credits, data.position, data.speed, data.maneuverability, data.acceleration, data.cargoHold, data.durability, 
        data.shield, data.militaryScanner, data.laserTemperature, data.drillDurability, data.asteroidReport, data.sectorInformation, data.fastTravel, data.repairDrones, data.repairKits);
        shipStats.SetHP(data.hp);
        economyManager.SetCredits(data.credits);
        Debug.Log("Data loaded");
    }
}
