using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerInventory : MonoBehaviour
{
    public List<ResourceStack> myItems = new List<ResourceStack>();
    public ShipStats shipStats;
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

    // sprawdzenie czy jest miejsce

    public bool CanFit(ResourceDefinition def, int amount)

    {
        if (shipStats == null) return false;

        float totalWeightToAdd = def.weight * amount;
        return(shipStats.AddCargo(totalWeightToAdd));

    }
   public void AddResource(ResourceDefinition definition, int amountToAdd)
    {
        if (amountToAdd <= 0) return;
        float weightUnit = definition.weight;
        //zmienne do testu czy dziala
        float weightToAdd = definition.weight * amountToAdd;
        float totalWeight = weightUnit * amountToAdd;
        //
        Debug.Log($"[Inventory] Próba dodania: {definition.Name}. Waga jedn.: {weightUnit}, Razem: {totalWeight}");
        if (shipStats == null) {
                    Debug.LogError("BŁĄD: ShipStats nie jest podpięte do PlayerInventory!");
                    return;
                }

        if (shipStats.AddCargo(weightToAdd))
        {
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
        else
        {
            Debug.Log("BRAK MIEJSCA W ŁADOWNI!");
        }
    }
    public void RefreshUI()
    {
        if (uiDisplay != null)
        {
            uiDisplay.ShowMinedResources(myItems);
            uiDisplay.UpdateCargoUI();
        }
    }

}