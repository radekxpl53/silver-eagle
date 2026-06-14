using UnityEngine;

[CreateAssetMenu(fileName = "SectorDefinition", menuName = "SilverEagle/Sector Definition")]
public class SectorDefinition : ScriptableObject
{
    public Vector2Int gridPosition;
    public int leadingStage;
    [Range(0, 3)] public int riskLevel = 1;
    public string displayName;
    [TextArea] public string description;
}
