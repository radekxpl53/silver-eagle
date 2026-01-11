using System;

[Serializable] 
public enum Rarity { Common, Uncommon,  Rare, Epic,  Legendary }

[Serializable]
public class ResourceDefinition
{
    public string Name;
    public Rarity Rarity;
    public int optimalTemp;
    public int tolerance;

}