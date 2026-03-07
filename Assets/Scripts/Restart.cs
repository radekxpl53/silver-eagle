using UnityEngine;

public class Restart : MonoBehaviour
{
    [SerializeField] private ShipStats shipStats;

    public void RestartMethod()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        PlayerData.Instance.ResetData();
        shipStats.ResetData();
        player.transform.position = PlayerData.Instance.position;
    }
}