using UnityEngine;
using TMPro;

public class Sector : MonoBehaviour {
    private SectorData data;
    [SerializeField] private TextMeshPro textLabel;

    
    public void Setup(SectorData newData) {
        data = newData;
        Debug.Log($"Sektor {data.gridPosition} ma w pamiêci zapisan¹ liczbê asteroid: {data.asteroidCount}");

        // Wypisywanie sektora nad (do testów)
        if (textLabel != null) {
            textLabel.text = data.gridPosition.ToString();
        }

        // Kolorki bo nie widzia³em róznicy ale ich nie wywalam bo wygl¹da ok
        GetComponentInChildren<Renderer>().material.color = Random.ColorHSV();
    }
}