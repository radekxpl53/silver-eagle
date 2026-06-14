using UnityEngine;

public class SectorTerritoryRules : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnSectorEntered += HandleSectorEntered;
    }

    private void OnDisable()
    {
        GameEvents.OnSectorEntered -= HandleSectorEntered;
    }

    private void HandleSectorEntered(Vector2Int grid, SectorDefinition sector)
    {
        if (sector == null) return;

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        ShipController controller = playerObj != null ? playerObj.GetComponent<ShipController>() : null;

        // Reset speed limit by default
        if (controller != null)
        {
            controller.SetSpeedLimit(1.0f);
        }

        // Apply rules based on territory
        if ((sector.territory == Territory.Cermandia || sector.territory == Territory.Ariandia) && sector.patrolPresence)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowSectorInfo("Patrol w pobliżu", Color.yellow);
            }
            if (controller != null)
            {
                controller.SetSpeedLimit(0.3f);
            }
            if (CockpitDisplayManager.Instance != null)
            {
                CockpitDisplayManager.Instance.ShowNotification("Speed Limit Active (30%) due to Patrol Presence.", Color.yellow);
            }
        }
        else if (sector.territory == Territory.Rubieze)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowSectorInfo("Brak ochrony prawnej. SOS niedostępne.", Color.red);
            }
            if (CockpitDisplayManager.Instance != null)
            {
                CockpitDisplayManager.Instance.ShowNotification("WARNING: Lawless Territory. SOS Systems Disabled.", Color.red);
            }
        }
        else if (sector.territory == Territory.Ariandia)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowSectorInfo("Wyłącz uzbrojenie w strefie kontrolnej", Color.yellow);
            }
            if (CockpitDisplayManager.Instance != null)
            {
                CockpitDisplayManager.Instance.ShowNotification("Notice: Disarm weapon systems in corporate control zone.", Color.yellow);
            }
        }
    }
}
