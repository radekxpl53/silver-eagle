using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SectorDataEntry
{
    public int gridX;
    public int gridY;
    public string json;
}

[Serializable]
public class SavedResourceStack
{
    public string resourceName;
    public int amount;
}

[Serializable]
public class GameSaveData
{
    public int saveVersion = 1;
    public float credits;
    public float debt;
    public Vector3 playerPosition;
    public int sectorGridX;
    public int sectorGridY;
    public float hp;
    public float energy;
    public float cargo;
    public List<string> purchasedUpgrades = new List<string>();
    public List<SavedResourceStack> inventory = new List<SavedResourceStack>();
    public List<SectorDataEntry> sectors = new List<SectorDataEntry>();
}
