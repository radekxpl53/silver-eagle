using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "SilverEagle/Faction Mission")]
public class FactionMissionDefinition : ScriptableObject
{
    public string missionId;
    public string displayName;
    [TextArea] public string description;
    public ResourceDefinition targetResource;
    public int requiredAmount;
    public float creditReward;
}
