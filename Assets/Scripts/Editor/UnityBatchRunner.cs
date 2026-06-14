using UnityEditor;
using UnityEngine;

public static class UnityBatchRunner
{
    public static void GenerateSectorsAndQuit()
    {
        SectorGenerator.GenerateSectors();
        Debug.Log("[BatchRunner] Sectors generated successfully.");
        EditorApplication.Exit(0);
    }

    public static void CompileCheckAndQuit()
    {
        Debug.Log("[BatchRunner] Compile check passed — no script errors.");
        EditorApplication.Exit(0);
    }

    public static void GenerateContentAndQuit()
    {
        ContentAssetGenerator.GenerateAll();
        Debug.Log("[BatchRunner] Upgrades and missions generated.");
        EditorApplication.Exit(0);
    }
}
