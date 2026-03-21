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

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}