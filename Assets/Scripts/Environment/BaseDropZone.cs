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

                if (shipStats.CurrentHP < shipStats.GetMaxHP())
                {
                    float hpDiff = shipStats.GetMaxHP() - shipStats.CurrentHP;
                    int cost = Mathf.CeilToInt(hpDiff * 0.5f);

                    EconomyManager.Instance.SpendCredits(cost);

                    if (costText != null)
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
