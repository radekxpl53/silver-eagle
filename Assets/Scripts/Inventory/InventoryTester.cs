using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTester : MonoBehaviour
{
    public PlayerInventory inventory;   
    public ResourceDatabase database;   
    public int testAmount = 50;          

    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (database != null && database.Resources.Count > 0)
            {
                // Losujemy surowiec
                int randomIndex = Random.Range(0, database.Resources.Count);
                ResourceDefinition randomDef = database.Resources[randomIndex];

                // Dodajemy do ekwipunku
                inventory.AddResource(randomDef, testAmount);
                
                Debug.Log($"<color=cyan>TEST:</color> Dodano {randomDef.Name} x{testAmount}");
            }
            else
            {
                Debug.LogError("Tester: Baza surowców jest pusta lub nieprzypisana!");
            }
        }
    }
}