using System;
using TMPro;
using UnityEngine;

public class BaseDropZone : MonoBehaviour
{
    [SerializeField] private GameObject healInfoCanvas;
    [SerializeField] private TextMeshProUGUI costText;

    private void Start()
    {
        healInfoCanvas.SetActive(false);
        GameManager.Instance.allRepairStationsPosition.Add(this.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShipStats shipStats = other.GetComponent<ShipStats>();

            if (shipStats == null)
            {
                shipStats = other.GetComponentInParent<ShipStats>();
            }

            if (shipStats != null)
            {
                if (shipStats.CurrentEnergy < shipStats.GetMaxEnergy())
                {
                    float energy = shipStats.GetMaxEnergy();
                    shipStats.AddEnergy(energy);
                    PlayerData.Instance.energy = energy;
                    Debug.Log("Zatankowano");
                }

                if (shipStats.CurrentHP < shipStats.GetMaxHP() || shipStats.CurrentEnergy < shipStats.GetMaxEnergy())
                {
                    float hpDiff = shipStats.GetMaxHP() - shipStats.CurrentHP;
                    float energyDiff = shipStats.GetMaxEnergy() - shipStats.CurrentEnergy;
                    int cost = Mathf.CeilToInt((hpDiff + energyDiff) * 0.5f);

                    if (!EconomyManager.Instance.SpendCredits(cost))
                    {
                        Debug.LogWarning("Brak kredytów na naprawę.");
                        return;
                    }

                    if (costText != null)
                        costText.text = "Cost: " + cost;

                    shipStats.Heal(hpDiff);
                    shipStats.AddEnergy(energyDiff);

                    if (healInfoCanvas != null)
                    {
                        healInfoCanvas.SetActive(true);
                        CancelInvoke("HideHealCanvas");
                        Invoke("HideHealCanvas", 3f);
                    }
                }
            }
        }
    }

    private void HideHealCanvas()
    {
        if (healInfoCanvas != null)
            healInfoCanvas.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.allRepairStationsPosition.Remove(this.transform);
    }
}
