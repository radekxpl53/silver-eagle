using System;
using TMPro;
using UnityEngine;

public class BaseDropZone : MonoBehaviour
{
    [SerializeField] private ShipStats shipStats;
    [SerializeField] private GameObject healInfoCanvas;
    [SerializeField] private TextMeshProUGUI costText;

    private void Start()
    {
        healInfoCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(shipStats.CurrentEnergy != shipStats.GetMaxEnergy())
            {
                float energy = shipStats.GetMaxEnergy();
                shipStats.AddEnergy(energy);
                PlayerData.Instance.energy = energy;
                Debug.Log("Zatankowano");
            }
            if (shipStats.CurrentHP != shipStats.GetMaxHP())
            {
                // ile hp leczymy
                float hpDiff = shipStats.GetMaxHP() - shipStats.CurrentHP;

                // koszt leczenia
                int cost = Mathf.CeilToInt(hpDiff * 0.5f);

                // wydanie kaski na leczenie <- TU TRZEBA TO NAPRAWIĆ
                EconomyManager.Instance.SpendCredits(cost);

                costText.text = "Cost: " + cost;

                shipStats.Heal(hpDiff);

                if (healInfoCanvas != null)
                {
                    healInfoCanvas.SetActive(true);

                    CancelInvoke("HideHealCanvas");

                    Invoke("HideHealCanvas", 3f);
                }

            }
        }
    }

    private void HideHealCanvas()
    {
        if (healInfoCanvas != null)
            healInfoCanvas.SetActive(false);
    }
}
