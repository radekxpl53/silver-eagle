using UnityEngine;

public class RepairSupportSystem : MonoBehaviour
{
    [SerializeField] private float droneHealPerSecond = 5f;
    [SerializeField] private string repairKitResourceId = "repair_kit";

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.currentState == GameState.Fighting) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        ShipStats stats = player.GetComponent<ShipStats>();
        if (stats == null || stats.IsDestroyed) return;

        if (PlayerData.Instance.repairDrones && stats.CurrentHP < stats.GetMaxHP())
            stats.Heal(droneHealPerSecond * Time.deltaTime);
    }

    public bool UseRepairKit()
    {
        if (!PlayerData.Instance.repairKits) return false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        PlayerInventory inv = player.GetComponent<PlayerInventory>();
        ShipStats stats = player.GetComponent<ShipStats>();
        if (inv == null || stats == null) return false;

        ResourceDefinition kitDef = FindRepairKitDefinition(inv);
        if (kitDef == null || inv.GetAmount(kitDef) < 1) return false;

        inv.RemoveResource(kitDef, 1);
        stats.Heal(stats.GetMaxHP() * 0.2f);
        inv.RefreshUI();
        return true;
    }

    private ResourceDefinition FindRepairKitDefinition(PlayerInventory inv)
    {
        foreach (var stack in inv.myItems)
        {
            if (stack.definition != null &&
                (stack.definition.Name.ToLower().Contains("repair") || stack.definition.Name.ToLower().Contains("kit")))
                return stack.definition;
        }
        return null;
    }
}
