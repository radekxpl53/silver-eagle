using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class InventoryDisplay : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform container;
    public Slider cargoSlider;
    public TextMeshProUGUI cargoText;
    public ShipStats shipStats;

    public void ShowMinedResources(List<ResourceStack> resources)
    {
        foreach (Transform child in container) {
            Destroy(child.gameObject);
        }

        foreach (ResourceStack stack in resources)
        {
            if (stack.definition != null && stack.amount > 0)
            {
                GameObject newSlot = Instantiate(slotPrefab, container);
                
                var slotScript = newSlot.GetComponent<UI_ResourceSlot>();
                if (slotScript != null)
                {
                    slotScript.Setup(stack.definition, stack.amount);
                }
            }
        }

        if (container is RectTransform rect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        UpdateCargoUI();
    } 

    public void UpdateCargoUI()
    {
        if (shipStats == null || cargoSlider == null) return;

        //pobieranie danych z shipStats
        float current = shipStats.CurrentCargo;
        float max = shipStats.GetMaxCargo();

        cargoSlider.maxValue = max;
        cargoSlider.value = current;

        if (cargoText != null)
        {
            cargoText.text = $"{current:F1} / {max:F0} kg";
        }

        if (cargoSlider.fillRect != null)
        {
            cargoSlider.fillRect.GetComponent<Image>().color = 
                (current / max > 0.9f) ? Color.red : Color.cyan;
        }
        
        Color barColor = (current / max > 0.9f) ? Color.red : Color.cyan;
        cargoSlider.fillRect.GetComponent<Image>().color = barColor;
    }
}