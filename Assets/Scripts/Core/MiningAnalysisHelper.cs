using System.Collections.Generic;
using UnityEngine;

public static class MiningAnalysisHelper
{
    public static void EmitAnalysisReady(Asteroid asteroid)
    {
        if (asteroid == null) return;

        Vector2Int grid = ChunkManager.Instance != null
            ? ChunkManager.Instance.CurrentPlayerSector
            : Vector2Int.zero;

        SectorDefinition def = SectorRegistry.GetDefinition(grid);
        MiningThreatLevel threat = def != null ? def.miningThreatLevel : MiningThreatLevel.Low;
        string composition = BuildCompositionSummary(asteroid.materials);
        float avgTemp = asteroid.CalculateTemperature();

        GameEvents.TriggerMiningAnalysisReady(def, threat, composition, avgTemp);
    }

    private static string BuildCompositionSummary(List<ResourceStack> materials)
    {
        if (materials == null || materials.Count == 0)
            return "Brak danych składu.";

        var parts = new List<string>();
        foreach (var stack in materials)
        {
            if (stack?.definition == null) continue;
            parts.Add($"{stack.definition.Name} x{stack.amount}");
        }

        return parts.Count > 0 ? string.Join(", ", parts) : "Brak danych składu.";
    }
}
