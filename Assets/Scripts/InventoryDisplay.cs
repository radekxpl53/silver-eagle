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

            if (stack.definition != null)
            {
                GameObject newSlot = Instantiate(slotPrefab, container);
            
                newSlot.GetComponent<UI_ResourceSlot>().Setup(stack.definition, stack.amount);
            }
        }
    } 

}
