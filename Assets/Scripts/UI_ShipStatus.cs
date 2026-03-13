using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_ShipStatus : MonoBehaviour
{
    private ShipStats shipStats;
    public Slider hp;
    public TextMeshProUGUI hpText;
    public Slider energy;
    public TextMeshProUGUI energyText;
    public Slider cargo;
    public TextMeshProUGUI cargoText;
    void Start()
    {
        shipStats = GetComponent<ShipStats>();

        hp.maxValue = shipStats.GetMaxHP();
        hp.value = shipStats.CurrentHP;
        hpText.text = shipStats.CurrentHP + "/" + shipStats.GetMaxHP();

        energy.maxValue = shipStats.GetMaxEnergy();
        energy.value = shipStats.CurrentEnergy;
        energyText.text = shipStats.CurrentEnergy + "/" + shipStats.GetMaxEnergy();

        cargo.maxValue = shipStats.GetMaxCargo();
        cargo.value = shipStats.CurrentCargo;
        cargoText.text = shipStats.CurrentCargo + "/" + shipStats.GetMaxCargo();
    }
    public void RefreshUI()
    {
        hp.value = shipStats.CurrentHP;
        hpText.text = Math.Round(shipStats.CurrentHP) + "/" + shipStats.GetMaxHP();

        energy.value = shipStats.CurrentEnergy;
        energyText.text = Math.Round(shipStats.CurrentEnergy) + "/" + shipStats.GetMaxEnergy();

        cargo.value = shipStats.CurrentCargo;
        cargoText.text = Math.Round(shipStats.CurrentCargo) + "/" + shipStats.GetMaxCargo();
    }
    void Update()
    {
        RefreshUI();
    }
}
