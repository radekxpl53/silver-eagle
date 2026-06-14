using System.Collections.Generic;
using UnityEngine;

public class FactionMissionSystem : MonoBehaviour
{
    public static FactionMissionSystem Instance { get; private set; }

    private FactionMissionDefinition[] missions;
    private readonly HashSet<string> completed = new HashSet<string>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        missions = Resources.LoadAll<FactionMissionDefinition>("Missions");
        if (missions == null || missions.Length == 0)
            missions = CreateDefaultMissions();
    }

    public IReadOnlyList<FactionMissionDefinition> GetActiveMissions() => missions;

    public bool TryCompleteMission(FactionMissionDefinition mission, PlayerInventory inventory)
    {
        if (mission == null || inventory == null) return false;
        if (completed.Contains(mission.missionId)) return false;
        if (mission.targetResource == null) return false;

        int have = inventory.GetAmount(mission.targetResource);
        if (have < mission.requiredAmount) return false;

        inventory.RemoveResource(mission.targetResource, mission.requiredAmount);
        EconomyManager.Instance?.AddCredits(mission.creditReward);
        completed.Add(mission.missionId);
        inventory.RefreshUI();
        return true;
    }

    private static FactionMissionDefinition[] CreateDefaultMissions()
    {
        ResourceDatabase db = FindResourceDatabase();
        ResourceDefinition fallback = db != null && db.Resources.Count > 0 ? db.Resources[0] : null;

        return new[]
        {
            MakeMission("deliver_iron", "Dostawa żelaza", "Dostarcz surowiec do frakcji.", fallback, 50, 300f),
            MakeMission("deliver_crystal", "Kryształy", "Zbierz kryształy dla laboratorium.", fallback, 30, 500f),
            MakeMission("deliver_rare", "Rzadki minerał", "Dostawa rzadkiego surowca.", fallback, 20, 800f),
        };
    }

    private static FactionMissionDefinition MakeMission(string id, string name, string desc, ResourceDefinition res, int amount, float reward)
    {
        var m = ScriptableObject.CreateInstance<FactionMissionDefinition>();
        m.missionId = id;
        m.displayName = name;
        m.description = desc;
        m.targetResource = res;
        m.requiredAmount = amount;
        m.creditReward = reward;
        return m;
    }

    private static ResourceDatabase FindResourceDatabase()
    {
        var dbs = Resources.FindObjectsOfTypeAll<ResourceDatabase>();
        return dbs.Length > 0 ? dbs[0] : null;
    }
}
