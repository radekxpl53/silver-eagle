using UnityEngine;

public enum Territory
{
    Cermandia,
    Ariandia,
    Rubieze,
    Tranzyt
}

public enum MiningThreatLevel
{
    Low,
    Mid,
    High,
    Critical
}

[CreateAssetMenu(fileName = "Sector_", menuName = "SilverEagle/Sector")]
public class SectorDefinition : ScriptableObject
{
    public Vector2Int gridPosition;
    public string sectorName;
    public Territory territory;
    [Range(0, 4)] public int leadingStage;
    [Range(0, 4)] public int riskLevel;
    
    [Header("Lore Texts")]
    public string jurisdictionText;
    public string profileText;
    public string riskAnalysisText;
    public string oreForecastText;
    
    [TextArea(3, 10)]
    public string crewNote;
    
    public string[] crtLogEntries;
    
    [Header("Mining & Economy")]
    public ResourceStack[] miningComposition;
    public MiningThreatLevel miningThreatLevel;
    public string miningThermalHint;
    public string miningSafetyMessage;
    
    public bool patrolPresence;
    public float shopTaxPercent; // e.g. 0 or 45
}
