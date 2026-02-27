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
        string json = JsonUtility.ToJson(playerData);
        Debug.Log(json);

        using(StreamWriter writer = new StreamWriter(Application.persistentDataPath + Path.AltDirectorySeparatorChar+ "SaveData.json"))
        {
            writer.Write(json);
        }
        Debug.Log("Data saved");
    }

    public void LoadData()
    {
        string json = string.Empty;
        using(StreamReader reader = new StreamReader(Application.persistentDataPath + Path.AltDirectorySeparatorChar+ "SaveData.json"))
        {
            json = reader.ReadToEnd();
        }

        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        playerData.SetPlayerData(data.hp, data.credits, data.speed, data.maneuverability, data.acceleration, data.cargoHold, data.durability, 
        data.shield, data.militaryScanner, data.laserTemperature, data.drillDurability, data.asteroidReport, data.sectorInformation, data.fastTravel, data.repairDrones, data.repairKits);
        Debug.Log("Data loaded");
    }
}
