using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionMissionSystem : MonoBehaviour
{
    public static FactionMissionSystem Instance { get; private set; }

    private FactionMissionDefinition[] missions;
    private readonly HashSet<string> completed = new HashSet<string>();
    private readonly HashSet<string> accepted = new HashSet<string>();

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
        ResolveMissionResources();
    }

    public IReadOnlyList<FactionMissionDefinition> GetActiveMissions()
    {
        if (missions == null) return System.Array.Empty<FactionMissionDefinition>();
        return missions.Where(m => m != null && !completed.Contains(m.missionId)).ToArray();
    }

    public bool IsAccepted(FactionMissionDefinition mission) =>
        mission != null && accepted.Contains(mission.missionId);

    public bool IsCompleted(FactionMissionDefinition mission) =>
        mission != null && completed.Contains(mission.missionId);

    public bool TryAcceptMission(FactionMissionDefinition mission)
    {
        if (mission == null || completed.Contains(mission.missionId)) return false;
        accepted.Add(mission.missionId);
        return true;
    }

    public int GetOwnedAmount(FactionMissionDefinition mission, PlayerInventory inventory)
    {
        if (mission == null || inventory == null || mission.targetResource == null) return 0;
        return inventory.GetAmount(mission.targetResource);
    }

    public bool TryCompleteMission(FactionMissionDefinition mission, PlayerInventory inventory)
    {
        if (mission == null || inventory == null) return false;
        if (completed.Contains(mission.missionId)) return false;
        if (!accepted.Contains(mission.missionId)) return false;
        if (mission.targetResource == null) return false;

        int have = inventory.GetAmount(mission.targetResource);
        if (have < mission.requiredAmount) return false;

        inventory.RemoveResource(mission.targetResource, mission.requiredAmount);
        EconomyManager.Instance?.AddCredits(mission.creditReward);
        completed.Add(mission.missionId);
        accepted.Remove(mission.missionId);
        inventory.RefreshUI();
        return true;
    }

    private void ResolveMissionResources()
    {
        ResourceDatabase db = FindResourceDatabase();
        if (db == null || missions == null) return;

        foreach (var mission in missions)
        {
            if (mission == null) continue;
            ResourceDefinition resolved = mission.missionId switch
            {
                "mission_copper_scan" => FindResource(db, "Miedź"),
                "mission_iron_delivery" => FindResource(db, "Ruda Żelaza"),
                "mission_rare_ore" => FindResource(db, "Kobalt") ?? FindResource(db, "Tytan"),
                _ => null
            };
            if (resolved != null)
                mission.targetResource = resolved;
        }
    }

    private static ResourceDefinition FindResource(ResourceDatabase db, string name)
    {
        foreach (ResourceDefinition res in db.Resources)
        {
            if (res != null && res.Name == name)
                return res;
        }
        return null;
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
