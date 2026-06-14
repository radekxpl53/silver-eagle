using System.Collections.Generic;
using UnityEngine;

public static class SectorRegistry
{
    private static Dictionary<Vector2Int, SectorDefinition> _cache;
    private static bool _loaded;

    public static void EnsureLoaded()
    {
        if (_loaded) return;
        _loaded = true;
        _cache = new Dictionary<Vector2Int, SectorDefinition>();
        var defs = Resources.LoadAll<SectorDefinition>("Sectors");
        foreach (var def in defs)
        {
            if (def != null)
                _cache[def.gridPosition] = def;
        }
    }

    public static SectorDefinition GetDefinition(Vector2Int grid)
    {
        EnsureLoaded();
        return _cache != null && _cache.TryGetValue(grid, out var def) ? def : null;
    }

    public static int GetLeadingStage(SectorData data)
    {
        if (data == null) return 0;
        var def = GetDefinition(data.gridPosition);
        return def != null ? def.leadingStage : data.sectorStage;
    }

    public static int GetRiskLevel(Vector2Int grid, int fallback = 1)
    {
        var def = GetDefinition(grid);
        return def != null ? def.riskLevel : fallback;
    }
}
