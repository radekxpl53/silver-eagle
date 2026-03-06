using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float hp;
    public float credits;
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

    private static PlayerData _instance = null;

    public delegate void OnPlayerDataChange(
    float hp,
    float credits,
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
    bool repairKits);
    public static event OnPlayerDataChange OnDataChange;

    public static PlayerData Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new PlayerData(100, 0, new Vector3(0, 0, 0), 0, 0, 0, 0, 0, 0, 0, 0, 0, false, false, false, false, false);
            }

            return _instance;
        }
    }  

    private PlayerData(
    float hp,
    float credits,
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
    bool repairKits)
    {
        this.hp = hp;
        this.credits = credits;
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
    } 

    public void SetPlayerData(
    float hp,
    float credits,
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
    bool repairKits)
    {
        this.hp = hp;
        this.credits = credits;
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

        OnDataChange?.Invoke(hp, credits, position, speed, maneuverability, acceleration, cargoHold, durability, 
        shield, militaryScanner, laserTemperature, drillDurability, asteroidReport, sectorInformation, fastTravel, repairDrones, repairKits);
    }

}
