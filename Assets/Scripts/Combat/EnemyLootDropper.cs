using UnityEngine;

public static class EnemyLootDropper
{
    private const float PickupRange = 80f;

    public static void DropLoot(Vector3 position, int sectorStage, Transform player = null)
    {
        var db = FindResourceDatabase();
        if (db == null) return;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        PlayerInventory inventory = player != null ? player.GetComponent<PlayerInventory>() : null;
        bool inRange = player != null && Vector3.Distance(position, player.position) <= PickupRange;

        int types = Random.Range(1, 4);
        for (int i = 0; i < types; i++)
        {
            int stage = SectorStageResolver.RollLootStage(sectorStage);
            ResourceDefinition res = db.GetRandomResource(Mathf.Clamp(stage, 0, 4));
            if (res == null) continue;

            int amount = Random.Range(5, 20);
            if (inRange && inventory != null)
                inventory.AddResource(res, amount);
            else
                Debug.Log($"[Loot] {res.Name} x{amount} @ {position} (poza zasięgiem)");
        }
    }

    private static ResourceDatabase FindResourceDatabase()
    {
        var dbs = Resources.FindObjectsOfTypeAll<ResourceDatabase>();
        return dbs.Length > 0 ? dbs[0] : null;
    }
}

public static class SectorStageResolver
{
    public static int RollLootStage(int leadingStage)
    {
        int roll = Random.Range(0, 100);
        if (roll < 20) return Mathf.Clamp(leadingStage - 1, 0, 4);
        if (roll >= 90) return Mathf.Clamp(leadingStage + 1, 0, 4);
        return Mathf.Clamp(leadingStage, 0, 4);
    }
}
