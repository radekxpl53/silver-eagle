using System.IO;
using UnityEngine;

public class SaveDataJSON : MonoBehaviour
{
    private PlayerData playerData;

    void Start()
    {
        playerData = PlayerData.Instance;
    }

    public void SaveData()
    {
        playerData.position = GameObject.FindGameObjectWithTag("Player").transform.position;
        
        string json = JsonUtility.ToJson(playerData);
        Debug.Log(json);

        string path = Application.persistentDataPath + "/SaveData.json";
        if(!File.Exists(path))
        {
            Debug.Log("Save file not found");
            return;
        }

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
        if(!File.Exists(path))
        {
            Debug.Log("Save file not found");
            return;
        }

        using(StreamReader reader = new StreamReader(path))
        {
            json = reader.ReadToEnd();
        }

        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        GameObject.FindGameObjectWithTag("Player").transform.position = data.position;

        playerData.SetPlayerData(data.hp, data.credits, data.position, data.speed, data.maneuverability, data.acceleration, data.cargoHold, data.durability, 
        data.shield, data.militaryScanner, data.laserTemperature, data.drillDurability, data.asteroidReport, data.sectorInformation, data.fastTravel, data.repairDrones, data.repairKits);
        Debug.Log("Data loaded");
    }
}
