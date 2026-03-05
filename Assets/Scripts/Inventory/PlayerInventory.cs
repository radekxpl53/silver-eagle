using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ResourceStack> myItems = new List<ResourceStack>();
    public InventoryDisplay uiDisplay;

    void Start()
    {
        if (uiDisplay == null)
        {
            Debug.LogError("BŁĄD: Nie podpiąłeś panelu do pola Ui Display w PlayerInventory na obiekcie: " + gameObject.name);
            return; 
        }

        if (myItems.Count == 0)
        {
            Debug.LogWarning("UWAGA: Lista My Items jest pusta. Dodaj surowce w Inspektorze!");
            uiDisplay.ShowMinedResources(myItems);
        }
        else
        {
            Debug.Log("Wysyłam listę surowców do UI. Liczba elementów: " + myItems.Count);
            uiDisplay.ShowMinedResources(myItems);
        }
    }

   public void AddResource(ResourceDefinition definition, int amountToAdd)
    {
        if (amountToAdd <= 0) return; 

        ResourceStack existingStack = myItems.Find(stack => stack.definition == definition);

        if (existingStack != null)
        {
            existingStack.amount += amountToAdd;
        }
        else
        {
            myItems.Add(new ResourceStack { definition = definition, amount = amountToAdd });
            
            myItems.Sort((a, b) => a.definition.Name.CompareTo(b.definition.Name));
        }

        
        RefreshUI();
        
        Debug.Log($"<color=green>INVENTORY:</color> Dodano {definition.Name} x{amountToAdd}");
    }
    public void RefreshUI()
    {
        if (uiDisplay != null)
        {
            uiDisplay.ShowMinedResources(myItems);
        }
    }
}