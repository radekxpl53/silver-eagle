using System.Collections.Generic;
using UnityEngine;


public class PlayerInventory : MonoBehaviour
{
    
    public List<ResourceStack> myItems = new List<ResourceStack>();
    
    public InventoryDisplay uiDisplay;

    void Start()
    {
        uiDisplay.ShowMinedResources(myItems);
        if (uiDisplay == null)
        {
            Debug.LogError("BŁĄD: Nie podpiąłeś panelu do pola Ui Display w PlayerInventory!");
            return;
        }

        if (myItems.Count == 0)
        {
            Debug.LogWarning("UWAGA: Lista My Items jest pusta. Dodaj surowce w Inspektorze!");
        }
        else
        {
            Debug.Log("Wysyłam listę surowców do UI. Liczba elementów: " + myItems.Count);
            uiDisplay.ShowMinedResources(myItems);
        }
        
    }
}