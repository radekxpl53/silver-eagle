using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class InventoryDisplay : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform container;

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
    } 
}