using UnityEngine;

public class Restart : MonoBehaviour
{
    [SerializeField] private ShipStats shipStats;

    public void RestartMethod()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        PlayerData.Instance.ResetData();
        shipStats.ResetData();

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        inventory.myItems.Clear();
        inventory.RefreshUI();

        player.transform.position = PlayerData.Instance.position;
        player.transform.rotation = Quaternion.identity;
    }
}