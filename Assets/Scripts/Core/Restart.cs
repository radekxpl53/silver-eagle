using UnityEngine;

public class Restart : MonoBehaviour
{
    private ShipStats shipStats;
    private EconomyManager economyManager;

    public void RestartMethod()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        shipStats = player.GetComponent<ShipStats>();
        economyManager = EconomyManager.Instance;

        PlayerData.Instance.ResetData();
        shipStats.ResetData();
        economyManager.SetCredits(0);

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        inventory.myItems.Clear();
        inventory.RefreshUI();

        player.transform.position = PlayerData.Instance.position;
        player.transform.rotation = Quaternion.identity;
    }
}