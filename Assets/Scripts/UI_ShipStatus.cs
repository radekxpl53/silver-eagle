using UnityEngine;
using UnityEngine.UI;

public class UI_ShipStatus : MonoBehaviour
{
    private ShipStats shipStats;
    public Slider hp;
    public Slider energy;
    public Slider cargo;
    void Start()
    {
        shipStats = GetComponent<ShipStats>();

        hp.maxValue = shipStats.GetMaxHP();
        hp.value = shipStats.CurrentHP;

        energy.maxValue = shipStats.GetMaxEnergy();
        energy.value = shipStats.CurrentEnergy;

        cargo.maxValue = shipStats.GetMaxCargo();
        cargo.value = shipStats.CurrentCargo;
    }

    void Update()
    {
        hp.value = shipStats.CurrentHP;
        energy.value = shipStats.CurrentEnergy;
        cargo.value = shipStats.CurrentCargo;
    }
}
