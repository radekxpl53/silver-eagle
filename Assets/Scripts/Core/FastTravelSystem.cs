using UnityEngine;

public class FastTravelSystem : MonoBehaviour
{
    [SerializeField] private float energyCost = 40f;

    public bool CanFastTravel()
    {
        return PlayerData.Instance.fastTravel && GameManager.Instance != null
            && GameManager.Instance.currentState == GameState.Exploration;
    }

    public bool TryFastTravelToRepairStation()
    {
        if (!CanFastTravel()) return false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        ShipStats stats = player.GetComponent<ShipStats>();
        if (stats == null || stats.CurrentEnergy < energyCost) return false;

        Transform target = FindNearestRepairStation(player.transform.position);
        if (target == null) return false;

        stats.UseEnergy(energyCost);
        TeleportPlayer(player, target.position);
        return true;
    }

    public bool TryFastTravelToSector(Vector2Int sectorGrid)
    {
        if (!CanFastTravel() || ChunkManager.Instance == null) return false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        ShipStats stats = player.GetComponent<ShipStats>();
        if (stats == null || stats.CurrentEnergy < energyCost) return false;

        Vector3 destination = ChunkManager.Instance.GetSectorWorldCenter(sectorGrid);
        stats.UseEnergy(energyCost);
        TeleportPlayer(player, destination);
        ChunkManager.Instance.ForcePlayerToSector(sectorGrid, destination);
        return true;
    }

    private static void TeleportPlayer(GameObject player, Vector3 position)
    {
        player.transform.position = position;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private Transform FindNearestRepairStation(Vector3 from)
    {
        if (GameManager.Instance == null || GameManager.Instance.allRepairStationsPosition.Count == 0)
            return null;

        Transform best = null;
        float bestDist = float.MaxValue;
        foreach (Transform t in GameManager.Instance.allRepairStationsPosition)
        {
            if (t == null) continue;
            float d = Vector3.Distance(from, t.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }
        return best;
    }
}
