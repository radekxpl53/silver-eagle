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
        player.transform.position = target.position;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        return true;
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
