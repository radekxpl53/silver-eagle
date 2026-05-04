using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public float hp;
    public int credits;
    public float energy;
    public List<ResourceStack> inventory;
    public Vector3 position;
    //Silniki(prędkość, skrętność, przyśpieszenie)
    public float speed;
    public float maneuverability; //skrętność
    public float acceleration; //przyśpieszenie
    public float cargoHold; //ładownia
    public float durability; //wytrzymałość
    public float shield; //tarcza
    public float militaryScanner; //skaner wojskowy
    public float laserTemperature; //temperatura lasera
    public float drillDurability; //wytrzymałość wiertła
    public bool asteroidReport; //raport próbki z asteroidy
    public bool sectorInformation; //informacje o sektorze
    public bool fastTravel;
    public bool repairDrones;
    public bool repairKits;

    public List<string> unlockedUpgrades = new List<string>();

    private static PlayerData _instance = null;
    public delegate void OnPlayerDataChange(
    float hp,
    int credits,
    float energy,
    List<ResourceStack> inventory,
    Vector3 position,
    float speed,
    float maneuverability,
    float acceleration,
    float cargoHold,
    float durability,
    float shield,
    float militaryScanner,
    float laserTemperature,
    float drillDurability,
    bool asteroidReport,
    bool sectorInformation,
    bool fastTravel,
    bool repairDrones,
    bool repairKits
    );
    public static event OnPlayerDataChange OnDataChange;

    public static PlayerData Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new PlayerData(100, 0, 200, new List<ResourceStack>(), Vector3.zero, 0, 0, 0, 0, 0, 0, 0, 0, 0, false, false, false, false, false, new List<string>());
            }
            return _instance;
        }
    }
    private PlayerData(
    float hp,
    int credits,
    float energy,
    List<ResourceStack> inventory,
    Vector3 position,
    float speed,
    float maneuverability,
    float acceleration,
    float cargoHold,
    float durability,
    float shield,
    float militaryScanner,
    float laserTemperature,
    float drillDurability,
    bool asteroidReport,
    bool sectorInformation,
    bool fastTravel,
    bool repairDrones,
    bool repairKits,
    List<string> upgrades)
    {
        this.hp = hp;
        this.credits = credits;
        this.energy = energy;
        this.inventory = inventory;
        this.position = position;
        this.speed = speed;
        this.maneuverability = maneuverability;
        this.acceleration = acceleration;
        this.cargoHold = cargoHold;
        this.durability = durability;
        this.shield = shield;
        this.militaryScanner = militaryScanner;
        this.laserTemperature = laserTemperature;
        this.drillDurability = drillDurability;
        this.asteroidReport = asteroidReport;
        this.sectorInformation = sectorInformation;
        this.fastTravel = fastTravel;
        this.repairDrones = repairDrones;
        this.repairKits = repairKits;
        this.unlockedUpgrades = upgrades;
    } 

    public void SetPlayerData(
    float hp,
    int credits,
    float energy,
    List<ResourceStack> inventory,
    Vector3 position,
    float speed,
    float maneuverability,
    float acceleration,
    float cargoHold,
    float durability,
    float shield,
    float militaryScanner,
    float laserTemperature,
    float drillDurability,
    bool asteroidReport,
    bool sectorInformation,
    bool fastTravel,
    bool repairDrones,
    bool repairKits,
    List<string> upgrades)
    {
        this.hp = hp;
        this.credits = credits;
        this.energy = energy;
        this.inventory = inventory;
        this.position = position;
        this.speed = speed;
        this.maneuverability = maneuverability;
        this.acceleration = acceleration;
        this.cargoHold = cargoHold;
        this.durability = durability;
        this.shield = shield;
        this.militaryScanner = militaryScanner;
        this.laserTemperature = laserTemperature;
        this.drillDurability = drillDurability;
        this.asteroidReport = asteroidReport;
        this.sectorInformation = sectorInformation;
        this.fastTravel = fastTravel;
        this.repairDrones = repairDrones;
        this.repairKits = repairKits;
        this.unlockedUpgrades = upgrades;
        
        OnDataChange?.Invoke(hp, credits, energy, inventory, position, speed, maneuverability, acceleration, cargoHold, durability, 
        shield, militaryScanner, laserTemperature, drillDurability, asteroidReport, sectorInformation, fastTravel, repairDrones, repairKits);
    }

    public void ResetData()
    {
        SetPlayerData(
            100,
            0,
            200,
            new List<ResourceStack>(),
            Vector3.zero,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            false,
            false,
            false,
            false,
            false,
            new List<string>()
        );
    }

}
