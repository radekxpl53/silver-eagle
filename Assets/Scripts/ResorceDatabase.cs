using System.Collections.Generic;
using UnityEngine;

// Ta linijka pozwoli Ci stworzyć bazę kliknięciem w Unity
[CreateAssetMenu(fileName = "NewResourceDatabase", menuName = "Tutorial/Resource Database")]
public class ResourceDatabase : ScriptableObject
{
    [Header("Globalne Wagi Rzadkości")]
    public int commonWeight = 10;
    public int uncommonWeight = 6;
    public int rareWeight = 4;
    public int legendaryWeight = 2;
    public int EpicWeight = 1;

    [Header("Lista Surowców")]
    public List<ResourceDefinition> Resources = new List<ResourceDefinition>();

    public int GetWeightForRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return commonWeight;
            case Rarity.Uncommon: return uncommonWeight;
            case Rarity.Rare: return rareWeight;
            case Rarity.Legendary: return legendaryWeight;
            default: return 0;
        }
    }
}