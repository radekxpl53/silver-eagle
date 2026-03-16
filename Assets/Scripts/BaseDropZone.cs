using UnityEngine;

public class BaseDropZone : MonoBehaviour
{
    [SerializeField] private ShipStats shipStats;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(shipStats.CurrentEnergy != shipStats.GetMaxHP())
            {
                float hp = shipStats.GetMaxHP();
                float energy = shipStats.GetMaxEnergy();
                shipStats.Heal(hp);
                shipStats.AddEnergy(energy);
                PlayerData.Instance.energy = energy;
                PlayerData.Instance.hp = hp;
                Debug.Log("Zatankowano");
            }
        }
    }
}
