using UnityEngine;

public enum UpgradeEffectType
{
    EngineThrust,
    CargoCapacity,
    MaxHP,
    Shield,
    MilitaryScanner,
    LaserMaxTemp,
    DrillDurability,
    AsteroidReport,
    SectorScanInfo,
    FastTravel,
    RepairDrones,
    RepairKits
}

[CreateAssetMenu(fileName = "Upgrade", menuName = "SilverEagle/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    public string upgradeId;
    public string displayName;
    [TextArea] public string description;
    public float creditCost;
    public int requiredSectorStage;
    public UpgradeEffectType effectType;
    public float effectValue;
}
