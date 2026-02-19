using UnityEngine;
using TMPro;

public class Sector : MonoBehaviour {
    private SectorData data;
    [SerializeField] private TextMeshPro textLabel;

    
    public void Setup(SectorData newData) {
        this.data = newData;
        Debug.Log($"Sektor {data.gridPosition}");

        // Wypisywanie sektora nad (do testów)
        if (textLabel != null) {
            textLabel.text = $"Sektor: {data.gridPosition}\nStage: {data.sectorStage}\n";
        }

        AreaSpawnerManager spawner = GetComponent<AreaSpawnerManager>();
        if (spawner != null) {
            spawner.currentSectorStage = data.sectorStage;
            
            spawner.InitialSpawn(data);
        }

        // Kolorki bo nie widzia³em róznicy ale ich nie wywalam bo wygl¹da ok
        GetComponentInChildren<Renderer>().material.color = Random.ColorHSV();
        GetComponent<AreaSpawnerManager>().currentSectorStage = data.sectorStage;
    }
}