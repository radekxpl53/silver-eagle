using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SectorContentDatabase", menuName = "SilverEagle/SectorContentDatabase")]
public class SectorContentDatabase : ScriptableObject
{
    private static SectorContentDatabase _instance;
    public static SectorContentDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<SectorContentDatabase>("SectorContentDatabase");
                if (_instance == null)
                {
                    // Fallback create or load
                    SectorContentDatabase[] dbs = Resources.FindObjectsOfTypeAll<SectorContentDatabase>();
                    if (dbs.Length > 0)
                    {
                        _instance = dbs[0];
                    }
                    else
                    {
                        _instance = CreateInstance<SectorContentDatabase>();
                        _instance.name = "SectorContentDatabase (Runtime)";
                    }
                }
            }
            return _instance;
        }
    }

    [SerializeField] private List<SectorDefinition> sectors = new List<SectorDefinition>();
    private Dictionary<Vector2Int, SectorDefinition> sectorMap;

    private void InitializeMap()
    {
        if (sectorMap == null)
        {
            sectorMap = new Dictionary<Vector2Int, SectorDefinition>();
            // Add Inspector defined sectors
            foreach (var sector in sectors)
            {
                if (sector != null && !sectorMap.ContainsKey(sector.gridPosition))
                {
                    sectorMap[sector.gridPosition] = sector;
                }
            }

            // Fallback load all from Resources/Sectors
            SectorDefinition[] loadedSectors = Resources.LoadAll<SectorDefinition>("Sectors");
            foreach (var sector in loadedSectors)
            {
                if (sector != null && !sectorMap.ContainsKey(sector.gridPosition))
                {
                    sectorMap[sector.gridPosition] = sector;
                    if (!sectors.Contains(sector))
                    {
                        sectors.Add(sector);
                    }
                }
            }
        }
    }

    public SectorDefinition GetSector(Vector2Int grid)
    {
        InitializeMap();
        if (sectorMap.TryGetValue(grid, out var def))
        {
            return def;
        }
        return null;
    }
}
